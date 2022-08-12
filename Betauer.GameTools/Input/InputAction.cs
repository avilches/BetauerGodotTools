using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input {
  
    public class InputAction : BaseAction, IInputAction {
        public static Builder Create(string name) => new Builder(name);
        public static Builder Create(string inputActionsContainerName, string name) => new Builder(inputActionsContainerName, name);

        public bool Pressed => Godot.Input.IsActionPressed(Name);
        public bool JustPressed => Godot.Input.IsActionJustPressed(Name);
        public bool Released => Godot.Input.IsActionJustReleased(Name);
        public List<JoystickList> Buttons => _buttons.ToList();
        public List<KeyList> Keys => _keys.ToList();
        public bool IsEventAction(InputEvent e, bool echo = false) => e.IsAction(Name, echo);
        public bool IsActionPressed(InputEvent e, bool echo = false) => e.IsActionPressed(Name, echo);
        public bool IsActionReleased(InputEvent e, bool echo = false) => e.IsActionReleased(Name, echo);
        public bool IsConfigurable() => _isConfigurable;

        private readonly string? _inputActionsContainerName;
        private readonly string? _settingsContainerName;
        private bool _isConfigurable;
        private readonly HashSet<JoystickList> _buttons = new HashSet<JoystickList>();
        private readonly HashSet<KeyList> _keys = new HashSet<KeyList>();
        public SaveSetting<string>? ButtonSetting { get; private set; }
        public SaveSetting<string>? KeySetting { get; private set; }

        private readonly string? _settingsSection;

        private InputAction(string inputActionsContainerName, string name, bool isConfigurable, string? settingsContainerName, string? settingsSection) : base(name) {
            _inputActionsContainerName = inputActionsContainerName;
            _isConfigurable = isConfigurable;
            _settingsContainerName = settingsContainerName;
            _settingsSection = settingsSection;
        }

        [PostCreate]
        private void ConfigureAndAddToActionContainer() {
            if (_isConfigurable) {
                var section = _settingsSection ?? "Controls";
                var buttonSetting =
                    Setting<string>.Save(_settingsContainerName, section, Name + ".Buttons", ExportButtons());
                var keySetting = Setting<string>.Save(_settingsContainerName, section, Name + ".Keys", ExportKeys());
                Container.InjectServices(buttonSetting);
                Container.InjectServices(keySetting);
                buttonSetting.ConfigureAndAddToSettingContainer();
                keySetting.ConfigureAndAddToSettingContainer();
                SetSettings(keySetting, buttonSetting);
            }
        }
        
        [PostCreate]
        private void AddToInputActionsContainer() {
            var inputActionsContainer = _inputActionsContainerName != null
                ? Container.Resolve<InputActionsContainer>(_inputActionsContainerName)
                : Container.Resolve<InputActionsContainer>();
            inputActionsContainer.Add(this);
            // The Add will set the InputActionsContainer using the OnAddToInputContainer
        }

        [PostCreate]
        public void Setup() {
            RemoveSetup();
            InputMap.AddAction(Name);
            CreateInputEvents().ForEach((e) => InputMap.ActionAddEvent(Name, e));
        }

        public void RemoveSetup() {
            if (InputMap.HasAction(Name)) InputMap.EraseAction(Name);            
        }

        public InputAction SetSettings(SaveSetting<string> keySetting, SaveSetting<string> buttonSetting) {
            KeySetting = keySetting;
            ButtonSetting = buttonSetting;
            _isConfigurable = true;
            return this;
        }

        
        private List<InputEvent> CreateInputEvents() {
            List<InputEvent> events = new List<InputEvent>();
            foreach (var key in _keys) {
                var e = new InputEventKey();
                e.Scancode = (uint)key;
                events.Add(e);
            }
            foreach (var button in _buttons) {
                var e = new InputEventJoypadButton();
                e.ButtonIndex = (int)button;
                events.Add(e);
            }
            return events;
        }

        public InputAction ClearButtons() {
            _buttons.Clear();
            return this;
        }

        public InputAction ClearKeys() {
            _keys.Clear();
            return this;
        }

        public InputAction AddKey(params KeyList[] keys) {
            foreach (var key in keys) _keys.Add(key);
            return this;
        }

        public InputAction AddButton(params JoystickList[] buttons) {
            foreach (var button in buttons) _buttons.Add(button);
            return this;
        }

        public InputAction Load() {
            if (KeySetting != null) ClearKeys().ImportKeys(KeySetting.Value);
            if (ButtonSetting != null) ClearButtons().ImportButtons(ButtonSetting.Value);
            return this;
        }
        
        public InputAction Save() {
            if (KeySetting != null) ButtonSetting.Value = ExportButtons();
            if (ButtonSetting != null) KeySetting.Value = ExportKeys();
            Setup();
            return this;
        }

        private string ExportKeys() => string.Join(",", _keys);
        private string ExportButtons() => string.Join(",", Buttons.Select(button => (int)button));

        private void ImportKeys(string keys) {
            if (string.IsNullOrWhiteSpace(keys)) return;
            keys.Split(",").ToList().ForEach(key => {
                try {
                    AddKey(Parse<KeyList>(key.Trim()));
                } catch (Exception) {}
            });
        }

        private void ImportButtons(string buttons) {
            if (string.IsNullOrWhiteSpace(buttons)) return;
            buttons.Split(",").ToList().ForEach(button => AddButton((JoystickList)button.Trim().ToInt()));
        }

        private static T Parse<T>(string key) => (T)Enum.Parse(typeof(T), key);

        public class Builder {
            private readonly string _name;
            private readonly string _inputActionsContainerName;
            private readonly ISet<JoystickList> _buttons = new HashSet<JoystickList>();
            private readonly ISet<KeyList> _keys = new HashSet<KeyList>();
            private bool _isConfigurable = false;
            private string? _settingsContainerName;
            private string? _settingsSection;

            internal Builder(string name) {
                _name = name;
            }

            internal Builder(string inputActionsContainerName, string name) {
                _inputActionsContainerName = inputActionsContainerName;
                _name = name;
            }

            public Builder Keys(params KeyList[] keys) {
                foreach (var key in keys) _keys.Add(key);
                return this;
            }

            public Builder Buttons(params JoystickList[] buttons) {
                foreach (var button in buttons) _buttons.Add(button);
                return this;
            }

            public Builder Configurable() {
                _isConfigurable = true;
                return this;
            }

            public Builder SettingsContainer(string settingsFile) {
                _isConfigurable = true;
                _settingsContainerName = settingsFile;
                return this;
            }

            public Builder SettingsSection(string settingsSection) {
                _isConfigurable = true;
                _settingsSection = settingsSection;
                return this;
            }

            public InputAction Build() {
                return new InputAction(_inputActionsContainerName, _name, _isConfigurable, _settingsContainerName, _settingsSection)
                    .AddKey(_keys.ToArray())
                    .AddButton(_buttons.ToArray());
            }
        }
    }
}