using System.Collections.Generic;
using System.Linq;
using Betauer.DI;
using Betauer.Input;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.UI {
    public abstract class ActionHint : DiHBoxContainer {
        [OnReady("ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("Label1")] private Label _label1;
        [OnReady("Label2")] private Label _label2;
        [Inject] private InputManager _inputManager;
        [Export] private string ActionName;
        [Export] private string ActionText1;
        [Export] private string ActionText2;

        public ActionHint(string actionName, string actionText1, string actionText2) {
            ActionName = actionName;
            ActionText1 = actionText1;
            ActionText2 = actionText2;
        }

        public override void Ready() {
            _label1.Text = ActionText1;
            _label1.Text = ActionText2;
            ActionState? action = _inputManager.FindActionState(ActionName);
            if (action != null) {
                JoystickList button = action.Buttons.First();
                _consoleButton.ShowButton(button, false);
            }

        }
    }
}