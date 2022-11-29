using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.Controller.UI.Consoles;

namespace Veronenger.Controller.UI {
    public class ActionHint : HBoxContainer {
        [OnReady("Label1")] private Label _label1;
        [OnReady("Control/ConsoleButton")] private ConsoleButton _consoleButton;
        [OnReady("KeyButton")] private Label _keyButton;
        [OnReady("Label2")] private Label _label2;
        [Inject] private InputActionsContainer _inputActionsContainer { get; set; }


        public ActionHint Labels(string? label1, string? label2) {
            _label1.Text = label1;
            _label2.Text = label2;
            _label1.Visible = !string.IsNullOrEmpty(label1);
            _label2.Visible = !string.IsNullOrEmpty(label2);
            return this;
        }

        // If using keyboard
        private string? _key;
        
        // If using gamepad
        private JoystickList _button;
        private bool _isAxis;
        private bool _animateButton;
        
        private bool _isUsingKeyboard;

        public ActionHint InputAction(InputAction action, bool isAxis, bool animate) {
            if (isAxis) {
                _button = (JoystickList)action.Axis;
                _isAxis = true;
                var keyList = new List<string>(2);
                var keys = action.Keys;
                if (keys.Count > 0) keyList.Add(keys[0].ToString());

                var axisAction = action.AxisAction;
                var otherActionName = action.Name == axisAction.PositiveName ? axisAction.NegativeName : axisAction.PositiveName;
                var otherAction = _inputActionsContainer.FindAction(otherActionName);
                var otherKeys = otherAction.Keys;
                if (otherKeys.Count > 0) keyList.Add(otherKeys[0].ToString());
                _key = $"[{string.Join(" ", keyList)}]";

            } else {
                var buttons = action.Buttons;
                _button = buttons.Count > 0 ? buttons[0] : JoystickList.InvalidOption;
                _isAxis = false;

                var keys = action.Keys;
                _key = keys.Count > 0 ? $"[{keys[0]}]" : "";
            }
            _animateButton = animate;
            
            Refresh();
            return this;
        }

        public ActionHint Refresh() {
            if (_key == null) return this;
            if (_isUsingKeyboard) {
                _keyButton.Visible = true;
                _consoleButton.Visible = false;
                _keyButton.Text = _key;
            } else {
                _keyButton.Visible = false;
                if (_button != JoystickList.InvalidOption) {
                    _consoleButton.Visible = true;
                    _consoleButton.SetButton(_isAxis, _button, _animateButton ? ConsoleButton.State.Animated : ConsoleButton.State.Normal);
                } else {
                    _consoleButton.Visible = false;
                }
            }
            return this;
        }

        public override void _Input(InputEvent e) {
            if (e.IsAnyKey() && !_isUsingKeyboard) {
                _isUsingKeyboard = true;
                Refresh();
            } else if ((e.IsAnyButton() || e.IsAnyAxis()) && _isUsingKeyboard) {
                _isUsingKeyboard = false;
                Refresh();
            }
        }

    }
}