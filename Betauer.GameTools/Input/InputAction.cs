using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input;
public class InputAction : IAction {
    public static NormalBuilder Create(string name) => new(name);
    public static NormalBuilder Create(string inputActionsContainerName, string name) => new(inputActionsContainerName, name);

    public static ConfigurableBuilder Configurable(string name) => new(name);
    public static ConfigurableBuilder Configurable(string inputActionsContainerName, string name) => new(inputActionsContainerName, name);

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
    public MouseButton MouseButton = MouseButton.None;

    public SaveSetting<string>? SaveSetting { get; private set; }
    
    [Inject] private Container Container { get; set; }
    [Inject] private SceneTree SceneTree { get; set; }
    private readonly HashSet<JoyButton> _buttons = new();
    private readonly HashSet<Key> _keys = new();
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

    public DelayedAction CreateDelayed(bool processAlways = false, bool processInPhysics = false, bool ignoreTimeScale = false) {
        return new DelayedAction(SceneTree, this, processAlways, processInPhysics, ignoreTimeScale);
    }

    public void LoadFromProjectSettings() {
        if (!InputMap.HasAction(Name)) {
            GD.PushWarning($"LoadFromProjectSettings: Action {Name} not found in project");
            return;
        }
        foreach (var inputEvent in InputMap.ActionGetEvents(Name)) {
            if (inputEvent is InputEventKey key) {
                AddKey(key.Keycode);
            } else if (inputEvent is InputEventJoypadButton button) {
                AddButton(button.ButtonIndex);
            } else if (inputEvent is InputEventJoypadMotion motion) {
                // TODO: feature missing, not tested!!!
                SetAxis(motion.Axis);
                SetAxisValue(motion.AxisValue);
            } else if (inputEvent is InputEventMouseButton mouseButton) {
                SetClick(mouseButton.ButtonIndex);
            }
        }
    }

    [PostInject]
    private void Configure() {
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
        if (MouseButton != MouseButton.None) {
            var e = new InputEventMouseButton();
            e.ButtonIndex = MouseButton;
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

    public InputAction SetClick(MouseButton mouseButton) {
        MouseButton = mouseButton;
        return this;
    }

    public InputAction ClearMouse() {
        MouseButton = MouseButton.None;
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

    public bool HasMouseButton() {
        return MouseButton != MouseButton.None;
    }

    public bool HasAxis() {
        return Axis != JoyAxis.Invalid;
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
        export.AddRange(_keys.Select(key => $"Key:{key}"));
        export.AddRange(_buttons.Select(button => $"Button:{button}"));
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
            ImportKey(value);
        } else if (key == "button") {
            ImportButton(value);
        } else if (key == "axis") {
            ImportAxis(value);
        }
    }

    private bool ImportKey(string value) {
        try {
            var key = int.TryParse(value, out _) ? (Key)value.ToInt() : Parse<Key>(value); 
            AddKey(key);
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private bool ImportButton(string value) {
        try {
            var joyButton = int.TryParse(value, out _) ? (JoyButton)value.ToInt() : Parse<JoyButton>(value); 
            AddButton(joyButton);
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private bool ImportAxis(string value) {
        try {
            var axis = int.TryParse(value, out _) ? (JoyAxis)value.ToInt() : Parse<JoyAxis>(value); 
            SetAxis(axis);
            return true;
        } catch (Exception) {
            return false;
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
        protected bool _keepProjectSettings = false;
        protected MouseButton _mouseButton = MouseButton.None;

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

        public TBuilder Click(MouseButton mouseButton) {
            _mouseButton = mouseButton;
            return this as TBuilder;
        }
        
        protected InputAction Build(InputAction inputAction) {
            if (_axis != JoyAxis.Invalid) {
                inputAction.SetAxis(_axis);
                inputAction.SetAxisValue(_axisValue);
            }
            if (_deadZone >= 0f) {
                inputAction.SetDeadZone(_deadZone);
            }
            if (_mouseButton != MouseButton.None) {
                inputAction.SetClick(_mouseButton);
            }
            if (_keys.Count > 0) {
                inputAction.AddKeys(_keys.ToArray());
            }
            if (_buttons.Count > 0) {
                inputAction.AddButtons(_buttons.ToArray());
            }
            return inputAction;
        }
    }

    public class NormalBuilder : Builder<NormalBuilder> {

        internal NormalBuilder(string name): base(name) {
        }

        internal NormalBuilder(string inputActionsContainerName, string name) : 
            base(inputActionsContainerName, name) {
        }

        public InputAction Build() {
            return Build(new InputAction(_inputActionsContainerName, _name, _oppositeActionName,
                _keepProjectSettings, false, null, null));
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
            return Build(new InputAction(_inputActionsContainerName, _name, _oppositeActionName,
                _keepProjectSettings, true, _settingsContainerName, _settingsSection));
        }
    }
}