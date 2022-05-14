using Betauer.DI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.UI {
    public class ConsoleButtonDebugger : ConsoleButton {

        public override void _Input(InputEvent e) {
            if (e is InputEventJoypadButton button) {
                Change((JoystickList)button.ButtonIndex, button.Pressed);
            }
        }
    }
}