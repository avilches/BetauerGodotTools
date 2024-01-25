using System;
using Betauer.Nodes;
using Godot;

namespace Betauer.Input;

public static class InputActionExtensions {
    public static Action AddOnPressed(this InputAction inputAction, Action<InputEvent> action) {
        Action<InputEvent> lambda = (InputEvent e) => {
            if (inputAction.IsEventPressed(e)) {
                action.Invoke(e);
            }
        };
        NodeManager.MainInstance.OnInput += lambda;
        return () => NodeManager.MainInstance.OnInput -= lambda;
    }

    public static Action AddOnJustPressed(this InputAction inputAction, Action<InputEvent> action) {
        Action<InputEvent> lambda = (InputEvent e) => {
            if (inputAction.IsEventJustPressed(e)) {
                action.Invoke(e);
            }
        };
        NodeManager.MainInstance.OnInput += lambda;
        return () => NodeManager.MainInstance.OnInput -= lambda;
    }

    public static Action AddOnReleased(this InputAction inputAction, Action<InputEvent> action) {
        Action<InputEvent> lambda = (InputEvent e) => {
            if (inputAction.IsEventReleased(e)) {
                action.Invoke(e);
            }
        };
        NodeManager.MainInstance.OnInput += lambda;
        return () => NodeManager.MainInstance.OnInput -= lambda;
    }
}