using System;
using Godot;

namespace Betauer.Input {
    public abstract class BaseAction {
        public static bool ActionPressed(string name, InputEvent e, bool includeEchoEvents) {
            if (e is InputEventJoypadMotion joyPad) {
                var strength = Math.Abs(Godot.Input.GetActionStrength(name));
                if (Math.Abs(joyPad.AxisValue) == strength) {
                    return strength == 1f;
                }
                return false;
            }
            if (InputMap.EventIsAction(e, name)) {
                if (includeEchoEvents) {
                    return e is InputEventKey { Pressed: true } ||
                           e is InputEventJoypadButton { Pressed: true };
                }
                // No echo, removed echoed inputs
                return (e is InputEventKey { Pressed: true } key && !key.IsEcho()) ||
                       (e is InputEventJoypadButton { Pressed: true } button && !button.IsEcho());
            }
            return false;
        }
    }
}