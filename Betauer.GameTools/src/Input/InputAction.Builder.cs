using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Betauer.Input;

public partial class InputAction {
    public class Builder {
        protected readonly string Name;
        protected bool IsKeepProjectSettings = false;

        private readonly ISet<JoyButton> _buttons = new HashSet<JoyButton>();
        private readonly ISet<Key> _keys = new HashSet<Key>();
        private JoyAxis _axis = JoyAxis.Invalid;
        private int _axisSign = 0;
        private float _deadZone = -1f;
        private MouseButton _mouseButton = MouseButton.None;
        private bool _ctrlPressed;
        private bool _shiftPressed;
        private bool _altPressed;
        private bool _metaPressed;
        private bool _commandOrCtrlPressed;
        private bool _pausable = false;

        internal Builder(string name) {
            Name = name;
        }

        public Builder DeadZone(float deadZone) {
            _deadZone = deadZone;
            return this;
        }

        public Builder NegativeAxis(JoyAxis axis) {
            _axis = axis;
            _axisSign = -1;
            return this;
        }

        public Builder PositiveAxis(JoyAxis axis) {
            _axis = axis;
            _axisSign = 1;
            return this;
        }

        public Builder KeepProjectSettings(bool keepProjectSettings = true) {
            IsKeepProjectSettings = keepProjectSettings;
            return this;
        }

        public Builder Keys(params Key[] keys) {
            keys.ForEach(key => _keys.Add(key));
            return this;
        }

        public Builder Buttons(params JoyButton[] buttons) {
            buttons.ForEach(button => _buttons.Add(button));
            return this;
        }

        public Builder Click(MouseButton mouseButton) {
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

        public Builder CommandOrCtrl() {
            _commandOrCtrlPressed = true;
            return this;
        }

        public Builder Pausable(bool pausable = true) {
            _pausable = pausable;
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
            inputAction.CommandOrCtrl = _commandOrCtrlPressed;
            inputAction.Pausable = _pausable;
        }
        
        public InputAction AsMock() {
            var input = CreateInputAction(InputActionBehaviour.Mock, false, false);
            ApplyConfig(input);
            return input;
        }

        public InputAction AsSimulator(bool godotInputMapToo = false) {
            var input = CreateInputAction(InputActionBehaviour.Simulate, godotInputMapToo, false);
            ApplyConfig(input);
            return input;
        }

        public InputAction AsGodotInput() {
            var input = CreateInputAction(InputActionBehaviour.GodotInput, true, false);
            ApplyConfig(input);
            return input;
        }

        public InputAction Extended(bool godotInputMapToo = false) {
            var input = CreateInputAction(InputActionBehaviour.Extended, godotInputMapToo, false);
            ApplyConfig(input);
            return input;
        }

        public InputAction ExtendedUnhandled(bool godotInputMapToo = false) {
            var input = CreateInputAction(InputActionBehaviour.Extended, godotInputMapToo, true);
            ApplyConfig(input);
            return input;
        }

        protected InputAction CreateInputAction(InputActionBehaviour behaviour, bool configureGodotInputMap, bool unhandled) {
            return new InputAction(
                Name,
                IsKeepProjectSettings,
                behaviour,
                configureGodotInputMap,
                unhandled);
        }
    }
}