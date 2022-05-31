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
        [Export] private string ActionText1;
        [Export] private string ActionName;
        [Export] private string ActionText2;
        private string _savedLabel1;
        private string _savedLabel2;
        private string _savedActionName;


        public override void Ready() {
            Configure(ActionText1, ActionName, ActionText2);
        }

        public void Configure(string? label1, string actionName, string? label2) {
            _label1.Text = ActionText1 = label1;
            _label2.Text = ActionText2 = label2;
            ActionName = actionName;
            _label2.Visible = !string.IsNullOrEmpty(label2);
            _label1.Visible = !string.IsNullOrEmpty(label1);

            ActionState? action = actionName != null ? _inputManager.FindActionState(actionName) : null;
            if (action != null) {
                JoystickList button = action.Buttons.First();
                _consoleButton.ShowButton(button, false);
            }
        }

        public void Save() {
            _savedLabel1 = ActionText1;
            _savedActionName = ActionName;
            _savedLabel2 = ActionText2;
        }

        public void Restore() {
            Configure(_savedLabel1, _savedActionName, _savedLabel2);
        }

    }
}