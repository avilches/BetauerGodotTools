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
        [OnReady("Label1")] private Label _label1;
        [OnReady("Control/ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("Label2")] private Label _label2;
        [Inject] private InputActionsContainer _inputActionsContainer { get; set; }
        [Export] private string? _actionText1;
        [Export] private string? _actionName;
        [Export] private string? _actionText2;


        public ActionHint Labels(string? label1, string? label2) {
            _label1.Text = _actionText1 = label1;
            _label2.Text = _actionText2 = label2;
            _label1.Visible = !string.IsNullOrEmpty(label1);
            _label2.Visible = !string.IsNullOrEmpty(label2);
            return this;
        }

        public ActionHint InputAction(InputAction action, bool animate = false) {
            _consoleButton.InputAction(action, animate);
            return this;
        }

        public void InputAction(string? actionName, bool animate = false) {
            if (_consoleButton.HasAnimation(actionName)) {
                _consoleButton.Animate(actionName);
            } else {
                InputAction? action = actionName != null ? _inputActionsContainer.FindAction(actionName) : null;
                InputAction(action, animate);
            }
        }


    }
}