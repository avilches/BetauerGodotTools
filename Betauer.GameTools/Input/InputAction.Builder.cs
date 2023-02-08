using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Betauer.Input;

public partial class InputAction {
    public abstract class Builder<TBuilder> where TBuilder : class {
        protected readonly string Name;
        protected readonly string InputActionsContainerName;
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

        internal Builder(string name) {
            Name = name;
        }

        internal Builder(string inputActionsContainerName, string name) {
            InputActionsContainerName = inputActionsContainerName;
            Name = name;
        }

        public TBuilder DeadZone(float deadZone) {
            _deadZone = deadZone;
            return this as TBuilder;
        }

        public TBuilder NegativeAxis(JoyAxis axis) {
            _axis = axis;
            _axisSign = -1;
            return this as TBuilder;
        }

        public TBuilder PositiveAxis(JoyAxis axis) {
            _axis = axis;
            _axisSign = 1;
            return this as TBuilder;
        }

        public TBuilder KeepProjectSettings(bool keepProjectSettings = true) {
            IsKeepProjectSettings = keepProjectSettings;
            return this as TBuilder;
        }

        public TBuilder Keys(params Key[] keys) {
            keys.ForEach(key => _keys.Add(key));
            return this as TBuilder;
        }

        public TBuilder Buttons(params JoyButton[] buttons) {
            buttons.ForEach(button => _buttons.Add(button));
            return this as TBuilder;
        }

        public TBuilder Click(MouseButton mouseButton) {
            _mouseButton = mouseButton;
            return this as TBuilder;
        }

        public TBuilder Ctrl() {
            _ctrlPressed = true;
            return this as TBuilder;
        }

        public TBuilder Shift() {
            _shiftPressed = true;
            return this as TBuilder;
        }

        public TBuilder Alt() {
            _altPressed = true;
            return this as TBuilder;
        }

        public TBuilder Meta() {
            _metaPressed = true;
            return this as TBuilder;
        }

        public TBuilder CommandOrCtrl() {
            _commandOrCtrlPressed = true;
            return this as TBuilder;
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
        }
        
        public InputAction AsSimulator() {
            var input = CreateInputAction(InputActionBehaviour.Simulate, false, false);
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

        protected abstract InputAction CreateInputAction(InputActionBehaviour behaviour, bool configureGodotInputMap, bool unhandled);
    }

    public class ConfigurableBuilder : Builder<ConfigurableBuilder> {

        private string? _settingsContainerName;
        private string? _settingsSection;

        public ConfigurableBuilder(string name) : base(name) {
        }

        public ConfigurableBuilder(string inputActionsContainerName, string name) : base(inputActionsContainerName, name) {
        }

        public ConfigurableBuilder SettingsContainer(string settingsFile) {
            _settingsContainerName = settingsFile;
            return this;
        }

        public ConfigurableBuilder SettingsSection(string settingsSection) {
            _settingsSection = settingsSection;
            return this;
        }

        protected override InputAction CreateInputAction(InputActionBehaviour behaviour, bool configureGodotInputMap, bool unhandled) {
            return new InputAction(
                InputActionsContainerName,
                Name,
                IsKeepProjectSettings,
                true,
                _settingsContainerName,
                _settingsSection,
                behaviour,
                configureGodotInputMap,
                unhandled);
        }
    }

    public class NormalBuilder : Builder<NormalBuilder> {
        internal NormalBuilder(string name) : base(name) {
        }

        internal NormalBuilder(string inputActionsContainerName, string name) :
            base(inputActionsContainerName, name) {
        }

        protected override InputAction CreateInputAction(InputActionBehaviour behaviour, bool configureGodotInputMap, bool unhandled) {
            return new InputAction(
                InputActionsContainerName,
                Name,
                IsKeepProjectSettings,
                false,
                null,
                null,
                behaviour,
                configureGodotInputMap,
                unhandled);
        }
    }
}