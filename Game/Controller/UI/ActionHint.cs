using System.Collections.Generic;
using System.Linq;
using Betauer.DI;
using Betauer.Input;
using Godot;
using Veronenger.Game.Controller.UI.Consoles;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.UI {
    public abstract class ActionHint : DiHBoxContainer {
        [OnReady("Control/ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("Label1")] private Label _label1;
        [OnReady("Label2")] private Label _label2;
        [Inject] private InputManager _inputManager;
        [Export] private string ActionName;
        [Export] private string ActionText1;
        [Export] private string ActionText2;

        
        public override void Ready() {
            _label1.Text = ActionText1;
            _label2.Text = ActionText2;
            ActionState? action = ActionName != null ? _inputManager.FindActionState(ActionName) : null;
            if (action != null) {
                JoystickList button = action.Buttons.First();
                _consoleButton.ShowButton(button, false);
            }

        }
    }
}