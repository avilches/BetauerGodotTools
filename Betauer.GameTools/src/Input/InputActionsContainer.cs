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

    public IAction? FindAction(string name) {
        return ActionMap.TryGetValue(name, out var action) ? action : null;
    }

    public T? FindAction<T>(string name) where T : class, IAction {
        return ActionMap.TryGetValue(name, out var action) ? action as T: null;
    }

    // Use InputAction.SetInputActionContainer() instead
    internal void Add(AxisAction axisAction) {
        if (InputActionList.Contains(axisAction)) return; // Avoid duplicates
        InputActionList.Add(axisAction);
        ActionMap.Add(axisAction.Name, axisAction);
        LinkAxisAction(axisAction);
    }

     // Use InputAction.SetInputActionContainer() instead
    internal void Add(InputAction inputAction) {
        if (InputActionList.Contains(inputAction)) return; // Avoid duplicates
        InputActionList.Add(inputAction);
        ActionMap.Add(inputAction.Name, inputAction);
        if (inputAction.AxisActionName != null) {
            LinkAxisAction(inputAction.AxisActionName);
        }
        Enable(inputAction);
    }

    private void LinkAxisAction(string axisActionName) {
        if (InputActionList.Find(action => action is AxisAction xa && xa.Name == axisActionName) is AxisAction axisAction) {
            LinkAxisAction(axisAction);
        }
    }

    // The link is needed because the axis action could be created in random order when using [InputAction] and [AxisAction] attributes.
    // So, as soon as the actions are added in PostInject() to the container, we try to link them.
    private void LinkAxisAction(AxisAction axisAction) {
        var pairs = InputActionList.FindAll(action => action is InputAction inputAction && inputAction.AxisActionName == axisAction.Name);
        if (pairs.Count == 2 && axisAction.Negative == null && axisAction.Positive == null) {
            axisAction.SetNegativeAndPositive((InputAction)pairs[0], (InputAction)pairs[1]);
        }
    }

    internal void Remove(AxisAction inputAction) {
        InputActionList.Remove(inputAction);
        ActionMap.Remove(inputAction.Name);
    }

    internal void Remove(InputAction inputAction) {
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