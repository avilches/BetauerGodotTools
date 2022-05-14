using System.Collections.Generic;
using System.Linq;
using Betauer.DI;
using Betauer.Input;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.UI {
    public abstract class ActionHint : DiNode2D {
        [OnReady("ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("Label")] private Label _label;
        [Inject] private InputManager _inputManager;
        [Export] private string ActionName;
        [Export] private string ActionText;

        public override void Ready() {
            _label.Text = ActionText;
            ActionState action = _inputManager.FindActionState(ActionName);
            JoystickList button = action.Buttons.First();
            _consoleButton.ShowButton(button, false);

        }
    }
}