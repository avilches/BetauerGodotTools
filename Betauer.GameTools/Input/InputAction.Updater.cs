using System;
using Godot;

namespace Betauer.Input;

public partial class InputAction {
    public class Updater {
        private readonly InputAction _inputAction;

        internal Updater(InputAction inputAction) {
            _inputAction = inputAction;
        }

        public Updater SetMouse(MouseButton mouseButton) {
            _inputAction.MouseButton = mouseButton;
            return this;
        }

        public Updater ClearMouse() {
            _inputAction.MouseButton = MouseButton.None;
            return this;
        }

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

        public Updater SetAxisSign(int defaultAxisSign) {
            if (Math.Abs(defaultAxisSign) != 1) throw new Exception("Invalid axis sign. Value  must be 1 or -1, but is "+defaultAxisSign);
            _inputAction.AxisSign = defaultAxisSign;
            return this;
        }

        public Updater WithCommandOrCtrl(bool enable = true) {
            _inputAction.CommandOrCtrl = enable;
            if (enable) {
                // Disable meta and control
                _inputAction.Meta = false;
                _inputAction.Ctrl = false;
            }
            return this;
        }

        public Updater WithCtrl(bool enable = true) {
            if (_inputAction.CommandOrCtrl) return this; // Ignore if CommandOrCtrl is enabled
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
            if (_inputAction.CommandOrCtrl) return this; // Ignore if CommandOrCtrl is enabled
            _inputAction.Meta = enable;
            return this;
        }

        // Key

        public Updater ClearKeys() {
            _inputAction._keys.Clear();
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
            _inputAction._keys.Add(key);
            return this;
        }

        public Updater AddKeys(params Key[] keys) {
            Array.ForEach(keys, key => AddKey(key));
            return this;
        }

        public Updater RemoveKey(Key key) {
            _inputAction._keys.Remove(key);
            return this;
        }
        
        // Buttons

        public Updater ClearButtons() {
            _inputAction._buttons.Clear();
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
            _inputAction._buttons.Add(button);
            return this;
        }

        public Updater AddButtons(params JoyButton[] buttons) {
            Array.ForEach(buttons, button => AddButton(button));
            return this;
        }

        public Updater RemoveButton(JoyButton button) {
            _inputAction._buttons.Remove(button);
            return this;
        }
    }
}