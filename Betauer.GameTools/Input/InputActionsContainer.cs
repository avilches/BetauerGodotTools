using System.Collections.Generic;
using System.Runtime.InteropServices;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input; 

public partial class InputActionsContainer : Node, IInjectable {
    [Inject(Nullable = true)] protected DebugOverlayManager? DebugOverlayManager { get; set; }

    public readonly List<IAction> InputActionList = new();
    public readonly Dictionary<string, IAction> ActionMap = new();
    private readonly List<InputAction> _onInputActions = new(); 
    private readonly List<InputAction> _onUnhandledInputActions = new();
    private bool _running = false;

    private NodeHandler _nodeHandler;

    public InputActionsContainer() {
        _nodeHandler = DefaultNodeHandler.Instance;
        _nodeHandler.AddChild(this);
    }

    public InputActionsContainer(NodeHandler nodeHandler) {
        _nodeHandler = nodeHandler;
        _nodeHandler.AddChild(this);
    }

    public void SetNodeHandler(NodeHandler nodeHandler) {
        _nodeHandler.RemoveChild(this);
        _nodeHandler = nodeHandler;
        _nodeHandler.AddChild(this);
    }

    public void PostInject() {
        // TODO: What if there are more than one InputActionsContainer? Only the last one will have the command linked
        DebugOverlayManager?.DebugConsole.AddInputEventCommand(this);
        DebugOverlayManager?.DebugConsole.AddInputMapCommand(this);
    }

    internal void Start() {
        if (_running) return;
        _running = true;
    }

    public IAction? FindAction(string name) {
        return ActionMap.TryGetValue(name, out var action) ? action : null;
    }

    public T? FindAction<T>(string name) where T : class, IAction {
        return ActionMap.TryGetValue(name, out var action) ? action as T: null;
    }

    public IAction? FindAction(InputEvent inputEvent) {
        return InputActionList.Find(action => action.IsEvent(inputEvent));
    }

    public List<IAction> FindActions(InputEvent inputEvent) {
        var list = new List<IAction>();
        for (var i = 0; i < InputActionList.Count; i++) {
            if (InputActionList[i].IsEvent(inputEvent)) list.Add(InputActionList[i]);
        }
        return list;
    }

    public AxisAction? FindAction(JoyAxis axis) {
        return InputActionList.Find(action => action is AxisAction && action.Axis == axis) as AxisAction;
    }

    public List<IAction> FindActions(JoyAxis axis) {
        return InputActionList.FindAll(action => action.Axis == axis);
    }

    public InputAction? FindAction(JoyButton button) {
        return InputActionList.Find(action => action is InputAction a && a.HasButton(button)) as InputAction;
    }

    public List<InputAction> FindActions(JoyButton button) {
        var list = new List<InputAction>();
        for (var i = 0; i < InputActionList.Count; i++) {
            if (InputActionList[i] is InputAction a && a.HasButton(button)) list.Add(a);
        }
        return list;
    }

    public InputAction? FindAction(Key key) {
        return InputActionList.Find(action => action is InputAction a && a.HasKey(key)) as InputAction;
    }

    public List<InputAction> FindActions(Key key) {
        var list = new List<InputAction>();
        for (var i = 0; i < InputActionList.Count; i++) {
            if (InputActionList[i] is InputAction a && a.HasKey(key)) list.Add(a);
        }
        return list;
    }

    public void Add(AxisAction axisAction) {
        if (InputActionList.Contains(axisAction)) return; // Avoid duplicates
        InputActionList.Add(axisAction);
        ActionMap.Add(axisAction.Name, axisAction);
        axisAction.OnAddToInputActionsContainer(this);
    }

    public void Add(InputAction inputAction) {
        if (InputActionList.Contains(inputAction)) return; // Avoid duplicates
        InputActionList.Add(inputAction);
        ActionMap.Add(inputAction.Name, inputAction);
        inputAction.OnAddToInputActionsContainer(this);
        Enable(inputAction);
    }

    public void Remove(AxisAction inputAction) {
        InputActionList.Remove(inputAction);
        ActionMap.Remove(inputAction.Name);
        inputAction.OnRemoveFromInputActionsContainer();
    }

    public void Remove(InputAction inputAction) {
        InputActionList.Remove(inputAction);
        ActionMap.Remove(inputAction.Name);
        inputAction.OnRemoveFromInputActionsContainer(this);
        Disable(inputAction);
    }

    public void Disable() {
        InputActionList.ForEach(action => action.Enable(false));
    }

    public void Enable(bool enabled = true) {
        InputActionList.ForEach(action => action.Enable(enabled));
    }

    internal void Enable(InputAction inputAction) {
        if (inputAction.Behaviour == InputActionBehaviour.Extended) {
            SetProcess(true);
            if (inputAction.IsUnhandledInput) {
                SetProcessUnhandledInput(true);
                _onUnhandledInputActions.Add(inputAction);
            } else {
                SetProcessInput(true);
                _onInputActions.Add(inputAction);
            }
        }
    }

    internal void Disable(InputAction inputAction) {
        if (inputAction.Behaviour == InputActionBehaviour.Extended) {
            if (inputAction.IsUnhandledInput) _onUnhandledInputActions.Remove(inputAction);
            else _onInputActions.Remove(inputAction);
        }
    }

    public override void _Input(InputEvent e) {
        if (_onInputActions.Count == 0) {
            SetProcessInput(false);
            return;
        }
        var paused = GetTree().Paused;
        var span = CollectionsMarshal.AsSpan(_onInputActions);
        for (var i = 0; i < span.Length; i++) {
            var inputAction = span[i];
            if (inputAction.IsEvent(e)) ((ExtendedInputFrameBasedStateHandler)inputAction.Handler).Update(paused, e);
        }
    }

    public override void _UnhandledInput(InputEvent e) {
        if (_onUnhandledInputActions.Count == 0) {
            SetProcessUnhandledInput(false);
            return;
        }
        var paused = GetTree().Paused;
        var span = CollectionsMarshal.AsSpan(_onUnhandledInputActions);
        for (var i = 0; i < span.Length; i++) {
            var inputAction = span[i];
            if (inputAction.IsEvent(e)) ((ExtendedInputFrameBasedStateHandler)inputAction.Handler).Update(paused, e);
        }
    }

    public override void _Process(double d) {
        if (_onInputActions.Count == 0 && 
            _onUnhandledInputActions.Count == 0) {
            SetProcess(false);
            return;
        }
        var delta = (float)d;
        var paused = GetTree().Paused;
        var handledSpan = CollectionsMarshal.AsSpan(_onInputActions);
        for (var i = 0; i < handledSpan.Length; i++) {
            var inputAction = handledSpan[i];
            ((ExtendedInputFrameBasedStateHandler)inputAction.Handler).AddTime(paused, delta);
        }
        var unhandledSpan = CollectionsMarshal.AsSpan(_onUnhandledInputActions);
        for (var i = 0; i < unhandledSpan.Length; i++) {
            var inputAction = unhandledSpan[i];
            ((ExtendedInputFrameBasedStateHandler)inputAction.Handler).AddTime(paused, delta);
        }
    }
}