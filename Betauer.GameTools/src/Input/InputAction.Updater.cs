using System;
using Godot;

namespace Betauer.Input;

public partial class InputAction {
    public class Updater {
        private readonly InputAction _inputAction;

        internal Updater(InputAction inputAction) {
            _inputAction = inputAction;
        }

        public Updater ClearAll() {
            // Mouse
            ClearMouse();
            
            // Joypad
            SetDeadZone(DefaultDeadZone);
            ClearAxis();
            ClearButtons();
            
            // Keys
            ClearModifiers();
            ClearKeys();
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
            SetJoypadId(inputAction.JoypadId);
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

        public Updater SetJoypadId(int joypadId) {
            _inputAction.JoypadId = joypadId;
            return this;
        }

        public Updater ClearJoypadId() {
            _inputAction.JoypadId = -1;
            return this;
        }

        public Updater SetAxisSign(int defaultAxisSign) {
            if (Math.Abs(defaultAxisSign) != 1) throw new Exception("Invalid axis sign. Value  must be 1 or -1, but is "+defaultAxisSign);
            _inputAction.AxisSign = defaultAxisSign;
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
            WithCommandOrCtrlAutoremap(false);
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
    }
}