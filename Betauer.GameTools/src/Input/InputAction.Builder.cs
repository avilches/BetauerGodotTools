using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Betauer.Input;

public partial class InputAction {
    public class Builder {
        private readonly string _name;
        private string _axisName;
        private string _saveAs;
        private bool _isKeepProjectSettings = false;

        private readonly ISet<JoyButton> _buttons = new HashSet<JoyButton>();
        private readonly ISet<Key> _keys = new HashSet<Key>();
        private JoyAxis _axis = JoyAxis.Invalid;
        private AxisSignEnum _axisSign = AxisSignEnum.Positive;
        private float _deadZone = -1f;
        private MouseButton _mouseButton = MouseButton.None;
        private bool _ctrlPressed;
        private bool _shiftPressed;
        private bool _altPressed;
        private bool _metaPressed;
        private bool _commandOrCtrlAutoremap;
                      
        internal Builder(string name) {
            _name = name;
        }

        public Builder AxisName(string axisName) {
            _axisName = axisName;
            return this;
        }

        public Builder DeadZone(float deadZone) {
            _deadZone = deadZone;
            return this;
        }

        public Builder NegativeAxis(JoyAxis axis) {
            _axis = axis;
            _axisSign = AxisSignEnum.Negative;
            return this;
        }

        public Builder PositiveAxis(JoyAxis axis) {
            _axis = axis;
            _axisSign = AxisSignEnum.Positive;
            return this;
        }

        public Builder KeepProjectSettings(bool keepProjectSettings = true) {
            _isKeepProjectSettings = keepProjectSettings;
            return this;
        }

        public Builder  Keys(params Key[] keys) {
            keys.ForEach(key => _keys.Add(key));
            return this;
        }

        public Builder Buttons(params JoyButton[] buttons) {
            buttons.ForEach(button => _buttons.Add(button));
            return this;
        }

        public Builder Mouse(MouseButton mouseButton) {
            _mouseButton = mouseButton;
            return this;
        }

        public Builder Ctrl() {
            _ctrlPressed = true;
            return this;
        }

        public Builder Shift() {
            _shiftPressed = true;
            return this;
        }

        public Builder Alt() {
            _altPressed = true;
            return this;
        }

        public Builder Meta() {
            _metaPressed = true;
            return this;
        }

        public Builder CommandOrCtrlAutoremap() {
            _commandOrCtrlAutoremap = true;
            return this;
        }

        public Builder SaveAs(string saveAs) {
            _saveAs = saveAs;
            return this;
        }

        private void ApplyConfig(InputAction inputAction) {
            if (_axis != JoyAxis.Invalid) {
                inputAction.Axis = _axis;
                inputAction.AxisSign = _axisSign;
            }
            if (_deadZone >= 0f) {
                inputAction.DeadZone = _deadZone;
            }
            if (_mouseButton != MouseButton.None) {
                inputAction.MouseButton = _mouseButton;
            }
            if (_keys.Count > 0) {
                _keys.ForEach(k => inputAction.Keys.Add(k));
            }
            if (_buttons.Count > 0) {
                _buttons.ForEach(b => inputAction.Buttons.Add(b));
            }
            inputAction.Ctrl = _ctrlPressed;
            inputAction.Shift = _shiftPressed;
            inputAction.Alt = _altPressed;
            inputAction.Meta = _metaPressed;
            inputAction.CommandOrCtrlAutoremap = _commandOrCtrlAutoremap;
        }
        
        public InputAction Simulator() {
            var input = CreateInputAction(InputActionBehaviour.Simulator);
            ApplyConfig(input);
            return input;
        }

        public InputAction Build(bool addWasPressed = false) {
            var input = CreateInputAction(addWasPressed ? InputActionBehaviour.Extended : InputActionBehaviour.GodotInput);
            ApplyConfig(input);
            return input;
        }

        protected InputAction CreateInputAction(InputActionBehaviour behaviour) {
            var inputAction = new InputAction(
                _name,
                _axisName,
                behaviour,
                _saveAs);
            if (_isKeepProjectSettings) {
                inputAction.LoadFromGodotProjectSettings();
            }
            return inputAction;
        }
    }
}