using System;
using System.Collections.Generic;
using System.Linq;
using Betauer;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.Game.Controller.UI.Consoles;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.UI {
    public abstract class ActionHint : HBoxContainer {
        [OnReady("Control/ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("Label1")] private Label _label1;
        [OnReady("Label2")] private Label _label2;
        [Inject] private InputActionsContainer _inputActionsContainer;
        [Export] private string ActionText1;
        [Export] private string ActionName;
        [Export] private string ActionText2;
        private string _savedLabel1;
        private string _savedLabel2;
        private string _savedActionName;


        public ActionHint Labels(string? label1, string? label2) {
            _label1.Text = ActionText1 = label1;
            _label2.Text = ActionText2 = label2;
            _label2.Visible = !string.IsNullOrEmpty(label2);
            _label1.Visible = !string.IsNullOrEmpty(label1);
            return this;
        }

        public ActionHint Button(ActionState actionState, bool animate = false) {
            JoystickList button = actionState.Buttons.First();
            if (animate) {
                _consoleButton.Animate(button);
            } else {
                _consoleButton.ShowButton(button);
            }
            return this;
        }

        public void InputAction(string? actionName, bool animate = false) {
            if (_consoleButton.HasAnimation(actionName)) {
                _consoleButton.Animate(actionName);
            } else {
                ActionState? action = actionName != null ? _inputManager.FindActionState(actionName) : null;
                if (action != null) {
                    Button(action, animate);
                }
            }
        }


    }
}