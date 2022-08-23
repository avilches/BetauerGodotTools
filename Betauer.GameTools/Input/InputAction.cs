using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input {

    public class AxisAction {
        [Inject] private Container Container { get; set; }

        public readonly string NegativeName;
        public readonly string PositiveName;
        public int Axis;
        public float DeadZone = 0.5f;

        public float Strength => Godot.Input.GetAxis(NegativeAction.Name, PositiveAction.Name);

        public InputAction NegativeAction { get; private set; }
        public InputAction PositiveAction { get; private set; }

        public AxisAction(string negativeName, string positiveName) {
            NegativeName = negativeName;
            PositiveName = positiveName;
        }

        public AxisAction SetDeadZone(float deadZone) {
            DeadZone = deadZone;
            return this;
        }

        public AxisAction SetAxis(int axis) {
            Axis = axis;
            return this;
        }

        [PostCreate]
        private void OnCreate() {
            NegativeAction = Container.Resolve<InputAction>(NegativeName);
            NegativeAction.SetAxis(Axis, -1, DeadZone);
            NegativeAction.Setup();
            
            PositiveAction = Container.Resolve<InputAction>(PositiveName);
            PositiveAction.SetAxis(Axis, 1, DeadZone);
            PositiveAction.Setup();
        }
        
        public bool IsRightEventPressed(InputEvent e, bool echo = false) {
            return e.IsActionPressed(PositiveName, echo);
        }

        public bool IsLeftEventPressed(InputEvent e, bool echo = false) {
            return e.IsActionPressed(NegativeName, echo);
        }
        
        public bool IsRightEventReleased(InputEvent e, bool echo = false) {
            return e.IsActionReleased(PositiveName, echo);
        }

        public bool IsLeftEventReleased(InputEvent e, bool echo = false) {
            return e.IsActionReleased(NegativeName, echo);
        }

        public bool IsDownEventPressed(InputEvent e, bool echo = false) {
            return e.IsActionPressed(PositiveName, echo);
        }

        public bool IsUpEventPressed(InputEvent e, bool echo = false) {
            return e.IsActionPressed(NegativeName, echo);
        }
        
        public bool IsDownEventReleased(InputEvent e, bool echo = false) {
            return e.IsActionReleased(PositiveName, echo);
        }

        public bool IsUpEventReleased(InputEvent e, bool echo = false) {
            return e.IsActionReleased(NegativeName, echo);
        }
        
    } 
  
    public class InputAction {
        public static NormalBuilder Create(string name) => new NormalBuilder(name);
        public static NormalBuilder Create(string inputActionsContainerName, string name) => new NormalBuilder(inputActionsContainerName, name);

        public static ConfigurableBuilder Configurable(string name) => new ConfigurableBuilder(name);
        public static ConfigurableBuilder Configurable(string inputActionsContainerName, string name) => new ConfigurableBuilder(inputActionsContainerName, name);

        public string Name { get; }
        public InputActionsContainer InputActionsContainer { get; private set; }
        public bool IsConfigurable() => _isConfigurable;
        public bool Pressed => Godot.Input.IsActionPressed(Name);
        public bool JustPressed => Godot.Input.IsActionJustPressed(Name);
        public bool Released => Godot.Input.IsActionJustReleased(Name);
        public List<JoystickList> Buttons => _buttons.ToList();
        public List<KeyList> Keys => _keys.ToList();
        public int Axis { get; private set; } = -1;
        public int AxisSign { get; private set; } = 1;
        public float DeadZone { get; private set; } = 0.5f;
        public SaveSetting<string>? ButtonSetting { get; private set; }
        public SaveSetting<string>? KeySetting { get; private set; }
        public bool IsEventAction(InputEvent e, bool echo = false) => e.IsAction(Name, echo);
        public bool IsActionPressed(InputEvent e, bool echo = false) => e.IsActionPressed(Name, echo);
        public bool IsActionReleased(InputEvent e, bool echo = false) => e.IsActionReleased(Name, echo);
        
        [Inject] private Container Container { get; set; }
        private readonly HashSet<JoystickList> _buttons = new HashSet<JoystickList>();
        private readonly HashSet<KeyList> _keys = new HashSet<KeyList>();
        private readonly string? _inputActionsContainerName;
        private readonly string? _settingsContainerName;
        private readonly string? _settingsSection;
        private bool _isConfigurable;


        private InputAction(string inputActionsContainerName, string name, bool isConfigurable, string? settingsContainerName, string? settingsSection) {
            Name = name;
            _inputActionsContainerName = inputActionsContainerName;
            _isConfigurable = isConfigurable;
            _settingsContainerName = settingsContainerName;
            _settingsSection = settingsSection;
            
        }

        public void SetAxis(int axis, int axisSign, float deadZone) {
            Axis = axis;
            AxisSign = axisSign;
            DeadZone = deadZone;
        }

        [PostCreate]
        private void ConfigureSettings() {
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
        public void Setup() {
            RemoveSetup();
            InputMap.AddAction(Name, DeadZone);
            CreateInputEvents().ForEach(e => InputMap.ActionAddEvent(Name, e));
        }

        [PostCreate]
        private void AddToInputActionsContainer() {
            var inputActionsContainer = _inputActionsContainerName != null
                ? Container.Resolve<InputActionsContainer>(_inputActionsContainerName)
                : Container.Resolve<InputActionsContainer>();
            inputActionsContainer.Add(this);
            // The Add will set the InputActionsContainer using the OnAddToInputContainer() method
        }

        public void OnAddToInputContainer(InputActionsContainer inputActionsContainer) {
            InputActionsContainer = inputActionsContainer;
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
                // e.Device = -1; // TODO: you can add a device id here
                events.Add(e);
            }

            if (Axis != -1) {
                var axisEvent = new InputEventJoypadMotion();
                axisEvent.Device = -1; // TODO: you can add a device id here
                axisEvent.Axis = Axis;
                axisEvent.AxisValue = AxisSign;
                events.Add(axisEvent);
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

        public class NormalBuilder {
            private readonly string _name;
            private readonly string _inputActionsContainerName;
            private readonly ISet<JoystickList> _buttons = new HashSet<JoystickList>();
            private readonly ISet<KeyList> _keys = new HashSet<KeyList>();

            internal NormalBuilder(string name) {
                _name = name;
            }

            internal NormalBuilder(string inputActionsContainerName, string name) {
                _inputActionsContainerName = inputActionsContainerName;
                _name = name;
            }

            public NormalBuilder Keys(params KeyList[] keys) {
                foreach (var key in keys) _keys.Add(key);
                return this;
            }

            public NormalBuilder Buttons(params JoystickList[] buttons) {
                foreach (var button in buttons) _buttons.Add(button);
                return this;
            }

            public InputAction Build() {
                return new InputAction(_inputActionsContainerName, _name, false, null, null)
                    .AddKey(_keys.ToArray())
                    .AddButton(_buttons.ToArray());
            }
        }
        
        public class ConfigurableBuilder {
            private readonly string _name;
            private readonly string _inputActionsContainerName;
            private readonly ISet<JoystickList> _buttons = new HashSet<JoystickList>();
            private readonly ISet<KeyList> _keys = new HashSet<KeyList>();
            private string? _settingsContainerName;
            private string? _settingsSection;

            internal ConfigurableBuilder(string name) {
                _name = name;
            }

            internal ConfigurableBuilder(string inputActionsContainerName, string name) {
                _inputActionsContainerName = inputActionsContainerName;
                _name = name;
            }

            public ConfigurableBuilder Keys(params KeyList[] keys) {
                foreach (var key in keys) _keys.Add(key);
                return this;
            }

            public ConfigurableBuilder Buttons(params JoystickList[] buttons) {
                foreach (var button in buttons) _buttons.Add(button);
                return this;
            }

            public ConfigurableBuilder SettingsContainer(string settingsFile) {
                _settingsContainerName = settingsFile;
                return this;
            }

            public ConfigurableBuilder SettingsSection(string settingsSection) {
                _settingsSection = settingsSection;
                return this;
            }

            public InputAction Build() {
                return new InputAction(_inputActionsContainerName, _name, true, _settingsContainerName, _settingsSection)
                    .AddKey(_keys.ToArray())
                    .AddButton(_buttons.ToArray());
            }
        }
    }
}