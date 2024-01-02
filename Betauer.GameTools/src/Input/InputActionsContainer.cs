using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Input.Handler;
using Betauer.Input.Joypad;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input; 

public partial class InputActionsContainer : Node, IInjectable {
    [Inject(Nullable = true)] protected DebugOverlayManager? DebugOverlayManager { get; set; }

    public readonly List<IAction> InputActionList = new();
    public readonly Dictionary<string, IAction> ActionMap = new();
    private readonly List<GodotInputHandler> _onInputActions = new(); 

    public bool Enabled { get; private set; } = true;

    /// <summary>
    /// The new cloned inputs won't have SaveSetting
    /// </summary>
    /// <param name="suffix"></param>
    /// <param name="updater"></param>
    /// <returns></returns>
    public InputActionsContainer Clone(string suffix, Action<InputAction, InputAction.Updater> updater) {
        var newIac = new InputActionsContainer();

        InputActionList.ForEach(i => {
            IAction? newInputAction = i switch {
                InputAction inputAction => inputAction.Clone(suffix).Update(u => updater(inputAction, u)),
                AxisAction axisAction => axisAction.Clone(suffix),
                _ => null
            };
            newInputAction?.SetInputActionsContainer(newIac);
        });
        return newIac;
    }

    public InputActionsContainer(bool enable = true) {
        if (enable) NodeManager.MainInstance.AddChild(this);
    }

    public void PostInject() {
        // TODO: What if there are more than one InputActionsContainer? Only the last one will have the command linked
        DebugOverlayManager?.DebugConsole.AddInputEventCommand(this);
        DebugOverlayManager?.DebugConsole.AddInputMapCommand(this);
    }

    public IAction? FindAction(string name) {
        return FindAction<IAction>(name);
    }

    public T? FindAction<T>(string name) where T : class, IAction {
        return ActionMap.TryGetValue(name, out var action) ? action as T: null;
    }

    // This method is only used by InputAction.SetInputActionContainer(). Don't use it directly, use InputAction.SetInputActionContainer() instead
    internal void Add(AxisAction axisAction) {
        if (InputActionList.Contains(axisAction)) return; // Avoid duplicates
        InputActionList.Add(axisAction);
        ActionMap.Add(axisAction.Name, axisAction);
        LinkAxisAction(axisAction);
    }

     // This method is only used by InputAction.SetInputActionContainer(). Don't use it directly, use InputAction.SetInputActionContainer() instead
    internal void Add(InputAction inputAction) {
        if (InputActionList.Contains(inputAction)) return; // Avoid duplicates
        InputActionList.Add(inputAction);
        ActionMap.Add(inputAction.Name, inputAction);
        if (inputAction.AxisName != null) {
            LinkAxisAction(inputAction.AxisName);
        }
        EnableAction(inputAction);
    }

    private void LinkAxisAction(string axisActionName) {
        if (InputActionList.Find(action => action is AxisAction xa && xa.Name == axisActionName) is AxisAction axisAction) {
            LinkAxisAction(axisAction);
        }
    }

    // The link is needed because the axis action could be created in random order when using [InputAction] and [AxisAction] attributes.
    // So, as soon as the actions are added in PostInject() to the container, we try to link them.
    private void LinkAxisAction(AxisAction axisAction) {
        var pairs = InputActionList.FindAll(action => action is InputAction inputAction && inputAction.AxisName == axisAction.Name);
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
        DisableAction(inputAction);
    }

    public void Disable() {
        Enabled = false;
        InputActionList.ForEach(action => action.Enable(false));
    }

    public void Enable(bool enable = true) {
        Enabled = true;
        InputActionList.ForEach(action => action.Enable(enable));
    }

    internal void EnableAction(InputAction inputAction) {
        if (Enabled && inputAction is { Enabled: true, Handler: GodotInputHandler { HasJustTimers: true } handler }) {
            SetProcessInput(true);
            _onInputActions.Add(handler);
        }
    }

    internal void DisableAction(InputAction inputAction) {
        if (inputAction.Handler is GodotInputHandler handler) {
            _onInputActions.Remove(handler);
        }
    }

    public override void _Input(InputEvent e) {
        if (_onInputActions.Count == 0) {
            SetProcessInput(false);
            return;
        }
        var span = CollectionsMarshal.AsSpan(_onInputActions);
        for (var i = 0; i < span.Length; i++) {
            var handler = span[i];
            handler.UpdateJustTimers();
        }
    }

    public T CreateJoypadController<T>(PlayerMapping playerMapping) where T : JoypadController, new() {
        var joypadController = new T();
        joypadController.Configure(playerMapping, this);
        return joypadController;
    }
}