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
        private bool _includeImportExportDeadzone = false;
        private bool _includeImportExportAxisSign = false;
        private bool _includeModifiers = false;
        private bool _enableJustTimer = false;
        private bool _allowMultipleButtons = true;
        private bool _allowMultipleKeys = true;
                      
        internal Builder(string name) {
            _name = name;
        }

        public Builder AxisName(string axisName) {
            _axisName = axisName;
            return this;
        }

        /// <summary>
        /// If includeImportExport is true, the deadZone value will be included in the export/import string, used by the SaveSetting created when you add
        /// a SettingContainer to the InputActionContainer where this InputAction is located.
        /// </summary>
        /// <param name="deadZone"></param>
        /// <param name="includeImportExport"></param>
        /// <returns></returns>
        public Builder DeadZone(float deadZone, bool includeImportExport = false) {
            _deadZone = deadZone;
            _includeImportExportDeadzone = includeImportExport;
            return this;
        }

        public Builder NegativeAxis(JoyAxis axis, bool includeImportExport = false) {
            _axis = axis;
            _axisSign = AxisSignEnum.Negative;
            _includeImportExportAxisSign = includeImportExport;
            return this;
        }

        public Builder PositiveAxis(JoyAxis axis, bool includeImportExport = false) {
            _axis = axis;
            _axisSign = AxisSignEnum.Positive;
            _includeImportExportAxisSign = includeImportExport;
            return this;
        }

        public Builder KeepProjectSettings(bool keepProjectSettings = true) {
            _isKeepProjectSettings = keepProjectSettings;
            return this;
        }

        public Builder Key(Key key, bool allowMultipleKeys = true) {
            _allowMultipleKeys = allowMultipleKeys;
            if (!allowMultipleKeys) _keys.Clear();
            _keys.Add(key);
            return this;
        }

        public Builder Keys(params Key[] keys) {
            _allowMultipleKeys = true;
            keys.ForEach(key => _keys.Add(key));
            return this;
        }

        public Builder Button(JoyButton button, bool allowMultipleButtons = true) {
            _allowMultipleButtons = allowMultipleButtons;
            if (!allowMultipleButtons) _buttons.Clear();
            _buttons.Add(button);
            return this;
        }

        public Builder Buttons(params JoyButton[] buttons) {
            _allowMultipleButtons = true;
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

        /// <summary>
        /// If true, the modifiers (Ctrl, Shift, Alt, Meta) will be included in the export/import string, used by the SaveSetting created when you add
        /// a SettingContainer to the InputActionContainer where this InputAction is located.
        /// </summary>
        /// <param name="includeModifiers"></param>
        /// <returns></returns>
        public Builder IncludeModifiers(bool includeModifiers = true) {
            _includeModifiers = includeModifiers;
            return this;
        }

        /// <summary>
        /// If true, the PressedTime and ReleasedTime attributes will return the time, in seconds, since the last press or release. And WasPressed() and
        /// WasReleased() will work properly.
        /// 
        /// If false, the PressedTime and ReleasedTime attributes will return a value of 0, and WasPressed() and WasReleased() will always return false.
        /// </summary>
        /// <param name="enableJustTimer"></param>
        /// <returns></returns>
        public Builder EnableJustTimers(bool enableJustTimer = true) {
            _enableJustTimer = enableJustTimer;
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

        public InputAction Build() {
            var input = CreateInputAction(_enableJustTimer ? InputActionBehaviour.Extended : InputActionBehaviour.GodotInput);
            ApplyConfig(input);
            input.IncludeModifiers = _includeModifiers;
            input.IncludeDeadZone = _includeImportExportDeadzone;
            input.IncludeAxisSign = _includeImportExportAxisSign;
            input.AllowMultipleKeys = _allowMultipleKeys;
            input.AllowMultipleButtons = _allowMultipleButtons;
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