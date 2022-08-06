using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input {
  
    public class InputAction : IInputAction {
        public static Builder Create(string name) => new Builder(name);

        public string Name { get; }
        public bool Pressed => Godot.Input.IsActionPressed(Name);
        public bool JustPressed => Godot.Input.IsActionJustPressed(Name);
        public bool Released => Godot.Input.IsActionJustReleased(Name);
        public List<JoystickList> Buttons => _buttons.ToList();
        public List<KeyList> Keys => _keys.ToList();
        public bool IsEventAction(InputEvent e, bool echo = false) => e.IsAction(Name, echo);
        public bool IsActionPressed(InputEvent e, bool echo = false) => e.IsActionPressed(Name, echo);
        public bool IsActionReleased(InputEvent e, bool echo = false) => e.IsActionReleased(Name, echo);
        public bool IsConfigurable() => _isConfigurable;

        [Inject] protected InputActionsContainer InputActionsContainer;
        [Inject] protected Container Container;
        private readonly string? _settingsContainerName;
        private readonly bool _isConfigurable;
        private readonly HashSet<JoystickList> _buttons = new HashSet<JoystickList>();
        private readonly HashSet<KeyList> _keys = new HashSet<KeyList>();
        private Setting<string> _buttonSetting;
        private Setting<string> _keySetting;

        private InputAction(string name, bool configurable, string? settingsContainerName) {
            Name = name;
            _isConfigurable = configurable;
            _settingsContainerName = settingsContainerName;
        }

        [PostCreate]
        internal void ConfigureAndAddToActionContainer() {
            if (_isConfigurable) {
                _buttonSetting = new Setting<string>(_settingsContainerName, "Controls", Name + ".Buttons", ExportButtons());
                _keySetting = new Setting<string>(_settingsContainerName, "Controls", Name + ".Keys", ExportKeys());
                Container.InjectAllFields(_buttonSetting);
                Container.InjectAllFields(_keySetting);
                _buttonSetting.AddToSettingContainer();
                _keySetting.AddToSettingContainer();
                Load();
            }
            InputActionsContainer.Add(this);
            Setup();
        }

        private void Setup() {
            if (InputMap.HasAction(Name)) InputMap.EraseAction(Name);
            InputMap.AddAction(Name);
            CreateInputEvents().ForEach((e) => InputMap.ActionAddEvent(Name, e));
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
            if (_isConfigurable) {
                ClearKeys().ImportKeys(_keySetting.Value);
                ClearButtons().ImportButtons(_buttonSetting.Value);
            }
            return this;
        }
        
        public InputAction Save() {
            if (_isConfigurable) {
                _buttonSetting.Value = ExportButtons();
                _keySetting.Value = ExportKeys();
            }
            Setup();
            return this;
        }

        private string ExportKeys() => string.Join(",", _keys);
        private string ExportButtons() => string.Join(",", Buttons.Select(button => (int)button));

        private void ImportKeys(string keys) {
            keys.Split(",").ToList().ForEach(key => AddKey(Parse<KeyList>(key)));
        }

        private void ImportButtons(string buttons) {
            buttons.Split(",").ToList().ForEach(button => AddButton((JoystickList)button.ToInt()));
        }

        private static T Parse<T>(string key) => (T)Enum.Parse(typeof(T), key);

        public class Builder {
            private readonly ISet<JoystickList> _buttons = new HashSet<JoystickList>();
            private readonly ISet<KeyList> _keys = new HashSet<KeyList>();

            private bool isConfigurable = false;
            private string? _settingsContainerName;
            private readonly string _name;

            internal Builder(string name) {
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

            public Builder Configurable(string? settingsFile = null) {
                isConfigurable = true;
                _settingsContainerName = settingsFile;
                return this;
            }

            public InputAction Build() {
                return new InputAction(_name, isConfigurable, _settingsContainerName)
                    .AddKey(_keys.ToArray())
                    .AddButton(_buttons.ToArray());
            }
        }
    }
}