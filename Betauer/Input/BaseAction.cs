using System;
using Godot;

namespace Betauer.Input {
    public abstract class BaseAction {
        public abstract bool IsEventAction(InputEvent e, bool includeEchoEvents = false);
    }
    
    public static class InputTools {  
        public static bool EventIsAction(string name, InputEvent e, bool includeEchoEvents) {
            if (e is InputEventJoypadMotion joyPad) {
                var strength = Math.Abs(Godot.Input.GetActionStrength(name));
                if (Math.Abs(joyPad.AxisValue) == strength) {
                    return strength == 1f;
                }
                return false;
            }
            if (!InputMap.EventIsAction(e, name)) return false;
            return e.IsActionPressed(name, includeEchoEvents) || e.IsActionReleased(name);
        }
    }
}