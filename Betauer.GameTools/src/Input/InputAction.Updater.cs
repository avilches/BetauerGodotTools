using System;
using System.Linq;
using Godot;

namespace Betauer.Input;

public partial class InputAction {
    public class Updater {
        private readonly InputAction _inputAction;

        internal Updater(InputAction inputAction) {
            _inputAction = inputAction;
        }

        public Updater Reset() {
            // Keyboard and mouse
            ClearModifiers();
            ClearKeys();

            // Joypad
            SetDeadZone(DefaultDeadZone);
            ClearAxis();
            ClearButtons();
            return this;
        }

        public Updater CopyAll(InputAction inputAction) {
            CopyModifiers(inputAction);
            CopyMouse(inputAction);
            CopyJoypad(inputAction);
            CopyKeys(inputAction);
            return this;
        }

        public Updater CopyModifiers(InputAction inputAction) {
            WithCommandOrCtrlAutoremap(inputAction.CommandOrCtrlAutoremap);
            WithCtrl(inputAction.Ctrl);
            WithShift(inputAction.Shift);
            WithAlt(inputAction.Alt);
            WithMeta(inputAction.Meta);
            return this;
        }

        public Updater CopyMouse(InputAction inputAction) {
            SetMouse(inputAction.MouseButton);
            return this;
        }

        public Updater CopyJoypad(InputAction inputAction) {
            SetDeadZone(inputAction.DeadZone);
            SetAxis(inputAction.Axis);
            SetAxisSign(inputAction.AxisSign);
            SetButtons(inputAction.Buttons.ToArray());
            return this;
        }

        public Updater CopyKeys(InputAction inputAction) {
            SetKeys(inputAction.Keys.ToArray());
            return this;
        }

        // Mouse
        public Updater SetMouse(MouseButton mouseButton) {
            _inputAction.MouseButton = mouseButton;
            return this;
        }

        public Updater ClearMouse() {
            _inputAction.MouseButton = MouseButton.None;
            return this;
        }

        // Joypad
        public Updater SetDeadZone(float deadZone) {
            _inputAction.DeadZone = deadZone;
            return this;
        }

        public Updater ClearAxis() {
            _inputAction.Axis = JoyAxis.Invalid;
            return this;
        }

        public Updater SetAxis(JoyAxis axis) {
            _inputAction.Axis = axis;
            return this;
        }

        public Updater SetAxisSign(AxisSignEnum axisSign) {
            _inputAction.AxisSign = axisSign;
            return this;
        }

        // Keys
        public Updater WithCommandOrCtrlAutoremap(bool enable = true) {
            _inputAction.CommandOrCtrlAutoremap = enable;
            if (enable) {
                // Disable meta and control
                _inputAction.Meta = false;
                _inputAction.Ctrl = false;
            }
            return this;
        }

        public Updater ClearModifiers() {
            WithCtrl(false);
            WithShift(false);
            WithAlt(false);
            WithMeta(false);
            return this;
        }

        public Updater WithCtrl(bool enable = true) {
            if (_inputAction.CommandOrCtrlAutoremap) return this; // Ignore if CommandOrCtrl is enabled
            _inputAction.Ctrl = enable;
            return this;
        }

        public Updater WithShift(bool enable = true) {
            _inputAction.Shift = enable;
            return this;
        }

        public Updater WithAlt(bool enable = true) {
            _inputAction.Alt = enable;
            return this;
        }

        public Updater WithMeta(bool enable = true) {
            if (_inputAction.CommandOrCtrlAutoremap) return this; // Ignore if CommandOrCtrl is enabled
            _inputAction.Meta = enable;
            return this;
        }

        // Key

        public Updater ClearKeys() {
            _inputAction.Keys.Clear();
            return this;
        }

        public Updater SetKey(Key key) {
            ClearKeys().AddKey(key);
            return this;
        }

        public Updater SetKeys(params Key[] keys) {
            ClearKeys().AddKeys(keys);
            return this;
        }

        public Updater AddKey(Key key) {
            if (!_inputAction.AllowMultipleKeys) ClearKeys();
            _inputAction.Keys.Add(key);
            return this;
        }

        public Updater AddKeys(params Key[] keys) {
            Array.ForEach(keys, key => AddKey(key));
            return this;
        }

        public Updater RemoveKey(Key key) {
            _inputAction.Keys.Remove(key);
            return this;
        }

        // Buttons

        public Updater ClearButtons() {
            _inputAction.Buttons.Clear();
            return this;
        }

        public Updater SetButton(JoyButton button) {
            ClearButtons().AddButton(button);
            return this;
        }

        public Updater SetButtons(params JoyButton[] buttons) {
            ClearButtons().AddButtons(buttons);
            return this;
        }

        public Updater AddButton(JoyButton button) {
            if (!_inputAction.AllowMultipleButtons) ClearButtons();
            _inputAction.Buttons.Add(button);
            return this;
        }

        public Updater AddButtons(params JoyButton[] buttons) {
            Array.ForEach(buttons, button => AddButton(button));
            return this;
        }

        public Updater RemoveButton(JoyButton button) {
            _inputAction.Buttons.Remove(button);
            return this;
        }

        /// <summary>
        /// If error, the key is not added. So, check if the Keys list is empty.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Updater ImportKeys(string input) {
            ClearKeys();
            Parser(input, (key, value) => {
                if (key != "key" || value.Length == 0 || (!_inputAction.AllowMultipleKeys && _inputAction.HasKeys())) return;
                var keyValue = ParseEnum(value, Key.Unknown);
                if (keyValue != Key.Unknown) AddKey(keyValue);
            });
            if (_inputAction.IncludeModifiers) ImportModifiers(input);
            return this;
        }

        /// <summary>
        /// If error, then the mouse is set to MouseButton.None
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Updater ImportMouse(string input) {
            ClearMouse();
            Parser(input, (key, value) => {
                if (key != "mouse" || value.Length == 0) return;
                _inputAction.MouseButton = ParseEnum(value, MouseButton.None);
            });
            if (_inputAction.IncludeModifiers) ImportModifiers(input);
            return this;
        }

        public Updater ImportModifiers(string input) {
            if (!_inputAction.IncludeModifiers) return this;
            ClearModifiers();
            Parser(input, (key, value) => {
                if (key == "ctrl") {
                    _inputAction.Ctrl = ParseModifierValue(value);
                } else if (key == "shift") {
                    _inputAction.Shift = ParseModifierValue(value);
                } else if (key == "alt") {
                    _inputAction.Alt = ParseModifierValue(value);
                } else if (key == "meta") {
                    _inputAction.Meta = ParseModifierValue(value);
                }
            });
            return this;
        }

        /// <summary>
        /// In case of button error, the button won't be added. Check if the Buttons list is empty.
        /// Wrong axis will be set as JoyAxis.Invalid
        /// A wrong axisSign will be ignored
        /// A wrong deadZone will be ignored
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Updater ImportJoypad(string input) {
            ClearButtons();
            ClearAxis();
            Parser(input, (key, value) => {
                if (value.Length == 0) return;
                if (key == "button") {
                    if (!_inputAction.AllowMultipleButtons && _inputAction.HasButtons()) return;
                    var joyButtonValue = ParseEnum(value, JoyButton.Invalid);
                    if (joyButtonValue >= 0 && joyButtonValue <= JoyButton.Max) AddButtons(joyButtonValue);
                } else if (key == "joyaxis") {
                    var axisValue = ParseEnum(value, JoyAxis.Invalid);
                    _inputAction.Axis = axisValue;
                } else if (_inputAction.IncludeAxisSign && key == "axissign") {
                    try {
                        _inputAction.AxisSign = Enum.Parse<AxisSignEnum>(value, true);
                    } catch (Exception) {
                    }
                } else if (_inputAction.IncludeDeadZone && key == "deadzone") {
                    if (float.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out var deadZoneValue)) {
                        if (deadZoneValue >= 0f && deadZoneValue <= 1f) _inputAction.DeadZone = deadZoneValue;
                    }
                }
            });
            return this;
        }

        private bool ParseModifierValue(string value) {
            // If there is no value, like just "Ctrl" or "Ctrl:", the the value is true
            if (string.IsNullOrWhiteSpace(value)) return true;
            // Return true or false if the value is true or false
            if (bool.TryParse(value, out var result)) return result;
            // If there is an error, lik "Ctrl:asdas", then return false
            return false;
        }

        private void Parser(string input, Action<string, string> action) {
            if (string.IsNullOrWhiteSpace(input)) return;
            input.Split(",")
                .ToList()
                .ForEach((item) => {
                    var parts = item.Split(":");
                    var key = parts[0].ToLower().Trim();
                    if (key.Length == 0) return;
                    // Empty values are ok
                    var value = parts.Length > 1 ? parts[1].Trim() : "";
                    action(key, value);
                });
        }

        private static T ParseEnum<T>(string input, T defaultValue) where T : struct {
            if (Enum.TryParse<T>(input, true, out var result)) {
                // The parsing was ok, but if the value name was a number like "2" the value could be wrong (2 couldn't be a valid value)
                var underlyingType = Enum.GetUnderlyingType(typeof(Key));
                if (Enum.IsDefined(typeof(T), Convert.ChangeType(result, underlyingType))) {
                    return result;
                }
            } 
            return defaultValue;
        }
    }
}