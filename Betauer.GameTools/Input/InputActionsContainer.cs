using System.Collections.Generic;
using System.Runtime.InteropServices;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input; 

public partial class InputActionsContainer : Node {
    [Inject(Nullable = true)] protected DebugOverlayManager? DebugOverlayManager { get; set; }

    public readonly List<IAction> InputActionList = new();
    public readonly Dictionary<string, IAction> ActionMap = new();
    private readonly List<InputAction> _extendedInputStateHandlers = new(); 
    private readonly List<InputAction> _extendedUnhandledInputActions = new();
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

    [PostInject]
    public void ConfigureCommands() {
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
        InputActionList.Add(axisAction);
        ActionMap.Add(axisAction.Name, axisAction);
    }

    public void Add(InputAction inputAction) {
        InputActionList.Add(inputAction);
        ActionMap.Add(inputAction.Name, inputAction);
        Enable(inputAction);
    }

    public void Remove(InputAction inputAction) {
        InputActionList.Remove(inputAction);
        ActionMap.Remove(inputAction.Name);
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
            if (inputAction.IsUnhandledInput) _extendedUnhandledInputActions.Add(inputAction);
            else _extendedInputStateHandlers.Add(inputAction);
        }
    }

    internal void Disable(InputAction inputAction) {
        if (inputAction.Behaviour == InputActionBehaviour.Extended) {
            if (inputAction.IsUnhandledInput) _extendedUnhandledInputActions.Add(inputAction);
            else _extendedInputStateHandlers.Add(inputAction);
        }
    }

    public override void _Input(InputEvent e) {
        var span = CollectionsMarshal.AsSpan(_extendedInputStateHandlers);
        var viewport = _nodeHandler.GetViewport();
        for (var i = 0; i < span.Length; i++) {
            if (viewport.IsInputHandled()) return;
            var inputAction = span[i];
            if (inputAction.IsEvent(e)) ((ExtendedInputActionStateHandler)inputAction.Handler).Update(e);
        }
    }

    public override void _UnhandledInput(InputEvent e) {
        var span = CollectionsMarshal.AsSpan(_extendedUnhandledInputActions);
        var viewport = _nodeHandler.GetViewport();
        for (var i = 0; i < span.Length; i++) {
            if (viewport.IsInputHandled()) return;
            var inputAction = span[i];
            if (inputAction.IsEvent(e)) ((ExtendedInputActionStateHandler)inputAction.Handler).Update(e);
        }
    }

    public override void _Process(double d) {
        var delta = (float)d;
        var handledSpan = CollectionsMarshal.AsSpan(_extendedInputStateHandlers);
        for (var i = 0; i < handledSpan.Length; i++) {
            var inputAction = handledSpan[i];
            ((ExtendedInputActionStateHandler)inputAction.Handler).AddTime(delta);
        }
        var unhandledSpan = CollectionsMarshal.AsSpan(_extendedUnhandledInputActions);
        for (var i = 0; i < unhandledSpan.Length; i++) {
            var inputAction = unhandledSpan[i];
            ((ExtendedInputActionStateHandler)inputAction.Handler).AddTime(delta);
        }
    }
}