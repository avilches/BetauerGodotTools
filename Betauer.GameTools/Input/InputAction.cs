using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input {
    public class InputAction {
        public static NormalBuilder Create(string name) => new NormalBuilder(name);
        public static NormalBuilder Create(string inputActionsContainerName, string name) => new NormalBuilder(inputActionsContainerName, name);

        public static ConfigurableBuilder Configurable(string name) => new ConfigurableBuilder(name);
        public static ConfigurableBuilder Configurable(string inputActionsContainerName, string name) => new ConfigurableBuilder(inputActionsContainerName, name);

        public string Name { get; }
        public InputActionsContainer InputActionsContainer { get; private set; }
        public bool IsConfigurable() => _isConfigurable;
        
        public bool Pressed(bool exact = false) => Godot.Input.IsActionPressed(Name, exact);
        public bool JustPressed(bool exact = false) => Godot.Input.IsActionJustPressed(Name, exact);
        public bool Released(bool exact = false) => Godot.Input.IsActionJustReleased(Name, exact);
        public float Strength(bool exact = false) => Godot.Input.GetActionStrength(Name, exact);
        public float RawStrength(bool exact = false) => Godot.Input.GetActionRawStrength(Name, exact);

        public AxisAction? AxisAction {
            get {
                if (_axisAction == null && _oppositeActionName != null) {
                    _axisAction = AxisValue > 0 ?
                        // AxisValue > 0 means current action is positive, so _oppositeActionName is negative
                        new AxisAction(_oppositeActionName, Name) : 
                        new AxisAction(Name, _oppositeActionName);
                }
                return _axisAction;
            }
        }
        
        public void SimulatePress(float strength = 1f) => Godot.Input.ActionPress(Name, strength);
        public void SimulateRelease() => Godot.Input.ActionRelease(Name);
        
        public List<JoystickList> Buttons => _buttons.ToList();
        public List<KeyList> Keys => _keys.ToList();
        public int Axis { get; private set; } = -1;
        public int AxisValue { get; private set; } = 1;
        public float DeadZone { get; private set; } = 0.5f;
        public SaveSetting<string>? SaveSetting { get; private set; }
        public bool IsAction(InputEvent e, bool exact = false) => e.IsAction(Name, exact);
        public bool IsActionPressed(InputEvent e, bool exact = false) => e.IsActionPressed(Name, exact);
        public bool IsActionReleased(InputEvent e, bool exact = false) => e.IsActionReleased(Name, exact);
        
        [Inject] private Container Container { get; set; }
        private readonly HashSet<JoystickList> _buttons = new HashSet<JoystickList>();
        private readonly HashSet<KeyList> _keys = new HashSet<KeyList>();
        private readonly string? _inputActionsContainerName;
        private readonly string? _settingsContainerName;
        private readonly string? _settingsSection;
        private readonly string? _oppositeActionName;
        private AxisAction? _axisAction;
        private bool _isConfigurable;

        private InputAction(string inputActionsContainerName, string name, string? oppositeActionName, bool keepProjectSettings,
            bool isConfigurable, string? settingsContainerName, string? settingsSection) {
            _inputActionsContainerName = inputActionsContainerName;
            Name = name;
            _oppositeActionName = oppositeActionName;
            _isConfigurable = isConfigurable;
            _settingsContainerName = settingsContainerName;
            _settingsSection = settingsSection;
            if (keepProjectSettings) LoadFromProjectSettings();
        }
        
        public void LoadFromProjectSettings() {
            if (!InputMap.HasAction(Name)) return;
            foreach (var inputEvent in InputMap.GetActionList(Name))
                if (inputEvent is InputEventKey key) AddKey((KeyList)key.Scancode);
                else if (inputEvent is InputEventJoypadButton button) AddButton((JoystickList)button.ButtonIndex);
            // TODO: mouse, axis missing
        }

        [PostCreate]
        private void ConfigureSaveSettings() {
            if (_isConfigurable) {
                var section = _settingsSection ?? "Controls";
                var setting = Setting<string>.Save(_settingsContainerName, section, Name, Export());
                Container.InjectServices(setting);
                setting.ConfigureAndAddToSettingContainer();
                SetSaveSettings(setting);
            }
        }
        
        [PostCreate]
        public void Setup() {
            RemoveSetup();
            if (DeadZone > 0) InputMap.AddAction(Name, DeadZone);
            else InputMap.AddAction(Name);
            CreateInputEvents()
                .ForEach(e => InputMap.ActionAddEvent(Name, e));
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

        public InputAction SetSaveSettings(SaveSetting<string> saveSetting) {
            SaveSetting = saveSetting;
            _isConfigurable = true;
            return this;
        }
        
        private List<InputEvent> CreateInputEvents() {
            List<InputEvent> events =
                new List<InputEvent>(_keys.Count + _buttons.Count + 1);
            foreach (var key in _keys) {
                var e = new InputEventKey();
                e.Scancode = (uint)key;
                events.Add(e);
            }
            foreach (var button in _buttons) {
                var e = new InputEventJoypadButton();
                // e.Device = -1; // TODO: you can add a device id here
                e.ButtonIndex = (int)button;
                events.Add(e);
            }

            if (Axis > -1 && AxisValue != 0) {
                var axisEvent = new InputEventJoypadMotion();
                axisEvent.Device = -1; // TODO: you can add a device id here
                axisEvent.Axis = Axis;
                axisEvent.AxisValue = AxisValue > 0 ? 1 : -1;
                events.Add(axisEvent);
            }
            return events;
        }

        public InputAction SetDeadZone(float deadZone) {
            DeadZone = deadZone;
            return this;
        }

        public InputAction SetAxis(int axis) {
            Axis = axis;
            return this;
        }

        public InputAction SetAxisValue(int axisValue) {
            AxisValue = axisValue;
            return this;
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
            if (SaveSetting != null) Import(SaveSetting.Value);
            return this;
        }
        
        public InputAction Save() {
            if (SaveSetting != null) {
                SaveSetting.Value = Export();
            }
            return this;
        }

        public string Export() {
            var export = new List<string>(_keys.Count + _buttons.Count + 1);
            export.AddRange(_keys.Select(key => $"Key:{key}"));
            export.AddRange(_buttons.Select(button => $"Button:{(int)button}"));
            if (Axis > -1) {
                export.Add("Axis:"+Axis);
            }
            return string.Join(",", export);
        }

        public void Import(string export) {
            if (string.IsNullOrWhiteSpace(export)) return;
            ClearButtons().ClearKeys();
            export.Split(",").ToList().ForEach(ImportItem);
        }

        private void ImportItem(string item) {
            if (!item.Contains(":")) return;
            var parts = item.Split(":");
            var key = parts[0].ToLower().Trim();
            var value = parts[1].Trim();
            if (key == "key") {
                try {
                    var keyList = Parse<KeyList>(value);
                    AddKey(keyList);
                } catch (Exception) {
                }
            } else if (key == "button") {
                if (value.IsValidInteger()) AddButton((JoystickList)value.ToInt());
            } else if (key == "axis") {
                if (value.IsValidInteger()) SetAxis(value.ToInt());
            }
        }

        private static T Parse<T>(string key) => (T)Enum.Parse(typeof(T), key);

        public class NormalBuilder {
            private readonly string _name;
            private readonly string _inputActionsContainerName;
            private readonly ISet<JoystickList> _buttons = new HashSet<JoystickList>();
            private readonly ISet<KeyList> _keys = new HashSet<KeyList>();
            private int _axis = -1;
            private int _axisValue = 0;
            private string? _oppositeActionName;
            private float _deadZone = -1f;
            private bool _keepProjectSettings = true;

            internal NormalBuilder(string name) {
                _name = name;
            }

            internal NormalBuilder(string inputActionsContainerName, string name) {
                _inputActionsContainerName = inputActionsContainerName;
                _name = name;
            }

            public NormalBuilder DeadZone(float deadZone) {
                _deadZone = deadZone;
                return this;
            }

            public NormalBuilder NegativeAxis(int axis, string positiveActionName) {
                _axis = axis;
                _axisValue = -1;
                _oppositeActionName = positiveActionName;
                return this;
            }

            public NormalBuilder PositiveAxis(int axis, string negativeActionName) {
                _axis = axis;
                _axisValue = 1;
                _oppositeActionName = negativeActionName;
                return this;
            }

            public NormalBuilder KeepProjectSettings(bool keepProjectSettings = true) {
                _keepProjectSettings = keepProjectSettings;
                return this;
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
                return new InputAction(_inputActionsContainerName, _name, _oppositeActionName, _keepProjectSettings,
                        false, null, null)
                    .SetAxis(_axis)
                    .SetAxisValue(_axisValue)
                    .SetDeadZone(_deadZone)
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
            private int _axis = -1;
            private int _axisValue = 0;
            private string? _oppositeActionName;
            private float _deadZone = 0.5f;
            private bool _keepProjectSettings = true;

            internal ConfigurableBuilder(string name) {
                _name = name;
            }

            internal ConfigurableBuilder(string inputActionsContainerName, string name) {
                _inputActionsContainerName = inputActionsContainerName;
                _name = name;
            }

            public ConfigurableBuilder DeadZone(float deadZone) {
                _deadZone = deadZone;
                return this;
            }

            public ConfigurableBuilder NegativeAxis(int axis, string positiveActionName) {
                _axis = axis;
                _axisValue = -1;
                _oppositeActionName = positiveActionName;
                return this;
            }

            public ConfigurableBuilder PositiveAxis(int axis, string negativeActionName) {
                _axis = axis;
                _axisValue = 1;
                _oppositeActionName = negativeActionName;
                return this;
            }

            public ConfigurableBuilder KeepProjectSettings(bool keepProjectSettings = true) {
                _keepProjectSettings = keepProjectSettings;
                return this;
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
                return new InputAction(_inputActionsContainerName, _name, _oppositeActionName, _keepProjectSettings, 
                        true, _settingsContainerName, _settingsSection)
                    .SetAxis(_axis)
                    .SetAxisValue(_axisValue)
                    .SetDeadZone(_deadZone)
                    .AddKey(_keys.ToArray())
                    .AddButton(_buttons.ToArray());
            }
        }
    }

    public class AxisAction {
        public readonly string NegativeName;
        public readonly string PositiveName;

        public float Strength => Godot.Input.GetAxis(NegativeName, PositiveName);

        internal AxisAction(string negativeName, string positiveName) {
            NegativeName = negativeName;
            PositiveName = positiveName;
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
}