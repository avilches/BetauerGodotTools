using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input {
    public class InputAction {
        private const MouseButton InvalidMouseButton = MouseButton.MaskXbutton2;

        public static NormalBuilder Create(string name) => new NormalBuilder(name);
        public static NormalBuilder Create(string inputActionsContainerName, string name) => new NormalBuilder(inputActionsContainerName, name);

        public static ConfigurableBuilder Configurable(string name) => new ConfigurableBuilder(name);
        public static ConfigurableBuilder Configurable(string inputActionsContainerName, string name) => new ConfigurableBuilder(inputActionsContainerName, name);

        public string Name { get; }
        public InputActionsContainer InputActionsContainer { get; private set; }
        
        public bool IsPressed(bool exact = false) => Godot.Input.IsActionPressed(Name, exact);
        public bool IsJustPressed(bool exact = false) => Godot.Input.IsActionJustPressed(Name, exact);
        public bool IsReleased(bool exact = false) => Godot.Input.IsActionJustReleased(Name, exact);

        public bool IsEvent(InputEvent e, bool exact = false) => e.IsAction(Name, exact);
        public bool IsEventPressed(InputEvent e, bool exact = false) => e.IsActionPressed(Name, exact);
        public bool IsEventJustPressed(InputEvent e, bool exact = false) => e.IsActionPressed(Name, exact) && e.IsJustPressed();
        public bool IsEventReleased(InputEvent e, bool exact = false) => e.IsActionReleased(Name, exact);

        public float GetStrength(bool exact = false) => Godot.Input.GetActionStrength(Name, exact);
        public float GetRawStrength(bool exact = false) => Godot.Input.GetActionRawStrength(Name, exact);

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
        
        public List<JoyButton> Buttons => _buttons.ToList();
        public List<Key> Keys => _keys.ToList();
        public JoyAxis Axis { get; private set; } = JoyAxis.Invalid;
        public float AxisValue { get; private set; } = 1;
        public float DeadZone { get; private set; } = 0.5f;
        public SaveSetting<string>? SaveSetting { get; private set; }
        
        [Inject] private Container Container { get; set; }
        private readonly HashSet<JoyButton> _buttons = new();
        private readonly HashSet<Key> _keys = new();
        private MouseButton _mouseButton = InvalidMouseButton;
        private readonly string? _inputActionsContainerName;
        private readonly string? _settingsContainerName;
        private readonly string? _settingsSection;
        private readonly string? _oppositeActionName;
        private AxisAction? _axisAction;
        private readonly bool _isConfigurable;

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
            foreach (var inputEvent in InputMap.ActionGetEvents(Name))
                if (inputEvent is InputEventKey key) AddKey(key.Keycode);
                else if (inputEvent is InputEventJoypadButton button) AddButton((JoyButton)button.ButtonIndex);
                else if (inputEvent is InputEventJoypadMotion motion) {
                    // TODO: feature missing, not tested!!!
                    SetAxis(motion.Axis);
                    SetAxisValue(motion.AxisValue);
                }
                else if (inputEvent is InputEventMouseButton mouseButton) SetMouse((MouseButton)mouseButton.ButtonIndex);
        }

        [PostCreate]
        private void PostCreate() {
            // Configure and load settings
            if (_isConfigurable) {
                var section = _settingsSection ?? "Controls";
                var setting = Setting<string>.Save(_settingsContainerName, section, Name, Export());
                Container.InjectServices(setting);
                setting.ConfigureAndAddToSettingContainer();
                SetSaveSettings(setting);
                
                // Load settings from file
                Load();
            }
            
            // Add to InputContainer
            var inputActionsContainer = _inputActionsContainerName != null
                ? Container.Resolve<InputActionsContainer>(_inputActionsContainerName)
                : Container.Resolve<InputActionsContainer>();
            inputActionsContainer.Add(this);
            
            // Configure the Godot InputMap
            Setup();
        }
        
        public void Setup() {
            RemoveSetup();
            if (DeadZone > 0) InputMap.AddAction(Name, DeadZone);
            else InputMap.AddAction(Name);
            CreateInputEvents()
                .ForEach(e => InputMap.ActionAddEvent(Name, e));
        }

        public void OnAddToInputContainer(InputActionsContainer inputActionsContainer) {
            InputActionsContainer = inputActionsContainer;
        }

        public void RemoveSetup() {
            if (InputMap.HasAction(Name)) InputMap.EraseAction(Name);            
        }

        public InputAction SetSaveSettings(SaveSetting<string> saveSetting) {
            SaveSetting = saveSetting;
            return this;
        }
        
        private List<InputEvent> CreateInputEvents() {
            List<InputEvent> events =
                new List<InputEvent>(_keys.Count + _buttons.Count + 1);
            foreach (var key in _keys) {
                var e = new InputEventKey();
                e.Keycode = key;
                events.Add(e);
            }
            if (_mouseButton != InvalidMouseButton) {
                var e = new InputEventMouseButton();
                e.ButtonIndex = _mouseButton;
                events.Add(e);
            }
            foreach (var button in _buttons) {
                var e = new InputEventJoypadButton();
                // e.Device = -1; // TODO: you can add a device id here
                e.ButtonIndex = button;
                events.Add(e);
            }

            if (Axis != JoyAxis.Invalid && AxisValue != 0) {
                var axisEvent = new InputEventJoypadMotion();
                axisEvent.Device = -1; // TODO: you can add a device id here
                axisEvent.Axis = Axis;
                axisEvent.AxisValue = AxisValue > 0 ? 1 : -1;
                events.Add(axisEvent);
            }
            return events;
        }

        public InputAction SetMouse(MouseButton mouseButton) {
            _mouseButton = mouseButton;
            return this;
        }

        public InputAction ClearMouse() {
            _mouseButton = InvalidMouseButton;
            return this;
        }

        public InputAction SetDeadZone(float deadZone) {
            DeadZone = deadZone;
            return this;
        }

        public InputAction SetAxis(JoyAxis axis) {
            Axis = axis;
            return this;
        }

        public InputAction SetAxisValue(float axisValue) {
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

        public InputAction ClearAxis() {
            SetAxis(JoyAxis.Invalid);
            return this;
        }

        public bool HasKey(Key key) {
            return _keys.Contains(key);
        }

        public bool HasButton(JoyButton button) {
            return _buttons.Contains(button);
        }

        public InputAction RemoveKey(Key key) {
            _keys.Remove(key);
            return this;
        }

        public InputAction RemoveButton(JoyButton button) {
            _buttons.Remove(button);
            return this;
        }

        public InputAction AddKey(Key key) {
            _keys.Add(key);
            return this;
        }

        public InputAction AddButton(JoyButton button) {
            _buttons.Add(button);
            return this;
        }

        public InputAction AddKeys(params Key[] keys) {
            Array.ForEach(keys, key => AddKey(key));
            return this;
        }

        public InputAction AddButtons(params JoyButton[] buttons) {
            Array.ForEach(buttons, button => AddButton(button));
            return this;
        }

        public InputAction ResetToDefaults() {
            if (SaveSetting == null) throw new Exception("InputAction does not have a Setting");
            Import(SaveSetting.DefaultValue);
            return this;
        }
        
        public InputAction Load() {
            if (SaveSetting == null) throw new Exception("InputAction does not have a Setting");
            Import(SaveSetting.Value);
            return this;
        }
        
        public InputAction Save() {
            if (SaveSetting == null) throw new Exception("InputAction does not have a Setting");
            SaveSetting.Value = Export();
            return this;
        }

        public string Export() {
            var export = new List<string>(_keys.Count + _buttons.Count + 1);
            export.AddRange(_keys.Select(key => $"Key:{(int)key}"));
            export.AddRange(_buttons.Select(button => $"Button:{(int)button}"));
            if (Axis != JoyAxis.Invalid) {
                export.Add($"Axis:{(int)Axis}");
            }
            return string.Join(",", export);
        }

        public InputAction Import(string export) {
            if (string.IsNullOrWhiteSpace(export)) return this;
            ClearButtons().ClearKeys().ClearAxis();
            export.Split(",").ToList().ForEach(ImportItem);
            return this;
        }

        private void ImportItem(string item) {
            if (!item.Contains(':')) return;
            var parts = item.Split(":");
            var key = parts[0].ToLower().Trim();
            var value = parts[1].Trim();
            if (key == "key") {
                if (value.IsValidInteger()) {
                    AddKey((Key)value.ToInt());
                } else {
                    try {
                        AddKey(Parse<Key>(value));
                    } catch (Exception) {
                    }
                }
            } else if (key == "button") {
                if (value.IsValidInteger()) {
                    AddButton((JoyButton)value.ToInt());
                } else {
                    try {
                        AddButton(Parse<JoyButton>(value));
                    } catch (Exception) {
                    }
                }
            } else if (key == "axis") {
                if (value.IsValidInteger()) {
                    SetAxis((JoyAxis)value.ToInt());
                } else {
                    try {
                        SetAxis(Parse<JoyAxis>(value));
                    } catch (Exception) {
                    }
                }
            }
        }

        private static T Parse<T>(string key) => (T)Enum.Parse(typeof(T), key);

        public abstract class Builder<TBuilder> where TBuilder : class {
            protected readonly string _name;
            protected readonly string _inputActionsContainerName;
            protected readonly ISet<JoyButton> _buttons = new HashSet<JoyButton>();
            protected readonly ISet<Key> _keys = new HashSet<Key>();
            protected JoyAxis _axis = JoyAxis.Invalid;
            protected float _axisValue = 0;
            protected string? _oppositeActionName;
            protected float _deadZone = -1f;
            protected bool _keepProjectSettings = true;
            protected MouseButton _mouseButton = InvalidMouseButton;

            internal Builder(string name) {
                _name = name;
            }

            internal Builder(string inputActionsContainerName, string name) {
                _inputActionsContainerName = inputActionsContainerName;
                _name = name;
            }

            public TBuilder DeadZone(float deadZone) {
                _deadZone = deadZone;
                return this as TBuilder;
            }

            public TBuilder NegativeAxis(JoyAxis axis, string positiveActionName) {
                _axis = axis;
                _axisValue = -1;
                _oppositeActionName = positiveActionName;
                return this as TBuilder;
            }

            public TBuilder PositiveAxis(JoyAxis axis, string negativeActionName) {
                _axis = axis;
                _axisValue = 1;
                _oppositeActionName = negativeActionName;
                return this as TBuilder;
            }

            public TBuilder KeepProjectSettings(bool keepProjectSettings = true) {
                _keepProjectSettings = keepProjectSettings;
                return this as TBuilder;
            } 

            public TBuilder Keys(params Key[] keys) {
                Array.ForEach(keys, key => _keys.Add(key));
                return this as TBuilder;
            }

            public TBuilder Buttons(params JoyButton[] buttons) {
                Array.ForEach(buttons, button => _buttons.Add(button));
                return this as TBuilder;
            }

            public TBuilder Mouse(MouseButton mouseButton) {
                _mouseButton = mouseButton;
                return this as TBuilder;
            }
        }

        public class NormalBuilder : Builder<NormalBuilder> {

            internal NormalBuilder(string name): base(name) {
            }

            internal NormalBuilder(string inputActionsContainerName, string name) : 
                base(inputActionsContainerName, name) {
            }

            public InputAction Build() {
                return new InputAction(_inputActionsContainerName, _name, _oppositeActionName, _keepProjectSettings,
                        false, null, null)
                    .SetAxis(_axis)
                    .SetAxisValue(_axisValue)
                    .SetDeadZone(_deadZone)
                    .SetMouse(_mouseButton)
                    .AddKeys(_keys.ToArray())
                    .AddButtons(_buttons.ToArray());
            }
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

            public InputAction Build() {
                return new InputAction(_inputActionsContainerName, _name, _oppositeActionName, _keepProjectSettings, 
                        true, _settingsContainerName, _settingsSection)
                    .SetAxis(_axis)
                    .SetAxisValue(_axisValue)
                    .SetDeadZone(_deadZone)
                    .SetMouse(_mouseButton)
                    .AddKeys(_keys.ToArray())
                    .AddButtons(_buttons.ToArray());
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