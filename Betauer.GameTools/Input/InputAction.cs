using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input;

public enum InputActionBehaviour {
    /// <summary>
    /// It works only through Simulate*() methods
    /// </summary>
    Simulate,
    
    /// <summary>
    /// It uses the Godot Input singleton to handle the action.
    /// </summary>
    GodotInput,
    
    /// <summary>
    /// Add new features to the action, like WasPressed() and WasReleased() methods.
    /// It can be processed in _Input or _UnhandledInput, so using unhandled, if a event is consumed by the GUI or any
    /// other part of the game, the action won't be affected.
    /// Optionally, the action can be added to the Godot Input Singleton too, so you can access to the action
    /// using Godot.Input.IsActionPressed too.
    /// </summary>
    Extended,
}
public partial class InputAction : IAction {
    public const float DefaultDeadZone = 0.5f;
    public static NormalBuilder Create(string name) => new(name);
    public static NormalBuilder Create(string inputActionsContainerName, string name) => new(inputActionsContainerName, name);

    public static ConfigurableBuilder Configurable(string name) => new(name);
    public static ConfigurableBuilder Configurable(string inputActionsContainerName, string name) => new(inputActionsContainerName, name);

    // Usage
    public string Name { get; }
    public bool IsPressed => Handler.Pressed;
    public bool IsJustPressed => Handler.JustPressed;
    public bool IsJustReleased => Handler.JustReleased;
    public float PressedTime => Handler.PressedTime;
    public float ReleasedTime => Handler.ReleasedTime;
    public float Strength => Handler.Strength;
    public float RawStrength => Handler.RawStrength;
    public void SimulatePress(float strength = 1f) => Handler.SimulatePress(strength);
    public void SimulateRelease() => Handler.SimulatePress(0f);

    public bool WasPressed(float elapsed) => PressedTime <= elapsed;
    public bool WasReleased(float elapsed) => ReleasedTime <= elapsed;

    public bool IsEvent(InputEvent e) => Matches(e);
    public bool IsEventPressed(InputEvent e) => IsEvent(e) && e.IsPressed();
    public bool IsEventJustPressed(InputEvent e) => IsEvent(e) && e.IsJustPressed();
    public bool IsEventReleased(InputEvent e) => IsEvent(e) && e.IsReleased();

    // Configuration
    public bool Pausable { get; private set; } = false;
    public List<JoyButton> Buttons { get; } = new();
    public List<Key> Keys { get; } = new();
    public JoyAxis Axis { get; private set; } = JoyAxis.Invalid;
    public int AxisSign { get; private set; } = 1;
    public float DeadZone { get; private set; } = DefaultDeadZone;
    public MouseButton MouseButton { get; private set; } = MouseButton.None;
    public bool CommandOrCtrl { get; private set; }
    public bool Ctrl { get; private set; }
    public bool Shift { get; private set; }
    public bool Alt { get; private set; }
    public bool Meta { get; private set; }
    
    // Constructor flags used by the [PostInject] to create the SaveSetting and locate the InputActionsContainer
    [Inject] private Container Container { get; set; }
    private readonly string? _inputActionsContainerName;
    private readonly bool _configureSaveSetting = false;
    private readonly string? _settingsContainerName;
    private readonly string? _settingsSection;
    
    public InputActionsContainer? InputActionsContainer { get; private set; }
    public InputActionBehaviour Behaviour { get; }
    public bool IsUnhandledInput { get; } = false;
    public bool Enabled { get; private set; } = true;
    public SaveSetting<string>? SaveSetting { get; private set; }
    internal readonly IHandler Handler;
    private readonly Updater _updater;
    private readonly bool _configureGodotInputMap = false;

    public static InputAction Fake() => new InputAction(null,
        null,
        false,
        false,
        null,
        null,
        InputActionBehaviour.Simulate,
        false,
        false);

    private InputAction(string inputActionsContainerName, 
        string name, 
        bool keepProjectSettings,
        bool configureSaveSetting, 
        string? settingsContainerName, 
        string? settingsSection,
        InputActionBehaviour behaviour, 
        bool configureGodotInputMap,
        bool isUnhandledInput) {
        Name = name;
        Behaviour = behaviour;
        _updater = new Updater(this);
        Enabled = true;
        _inputActionsContainerName = inputActionsContainerName;
        _configureSaveSetting = configureSaveSetting;
        _settingsContainerName = settingsContainerName;
        _settingsSection = settingsSection;
        _configureGodotInputMap = behaviour == InputActionBehaviour.GodotInput || configureGodotInputMap;
        IsUnhandledInput = isUnhandledInput;

        Handler = behaviour switch {
            InputActionBehaviour.Simulate => new ActionStateHandler(this),
            InputActionBehaviour.GodotInput => new GodotInputHandler(name),
            InputActionBehaviour.Extended => new ExtendedInputActionStateHandler(this),
        };
        _configureGodotInputMap = true;
        if (keepProjectSettings) {
            LoadFromGodotProjectSettings();
        }
        // Don't call SetupGodotInputMap here. It's better to wait until Configure() load the saved setting
    }

    [PostInject]
    private void Configure() {
        // Configure and load settings
        if (_configureSaveSetting) {
            var section = _settingsSection ?? "Controls";
            var setting = Setting<string>.Persistent(_settingsContainerName, section, Name, AsString());
            Container.InjectServices(setting);
            setting.ConfigureAndAddToSettingContainer();
            SetSaveSettings(setting);
            
            // Load settings from file
            Load();
        }
        
        // Find the InputContainer from constructor
        var inputActionsContainer = _inputActionsContainerName != null
            ? Container.Resolve<InputActionsContainer>(_inputActionsContainerName)
            : Container.Resolve<InputActionsContainer>();
        inputActionsContainer.Add(this);
        inputActionsContainer.Start();

        SetupGodotInputMap();
    }

    internal void OnAddToInputActionsContainer(InputActionsContainer inputActionsContainer) {
        if (InputActionsContainer != null && InputActionsContainer != inputActionsContainer) {
            InputActionsContainer.Remove(this);
        }
        InputActionsContainer = inputActionsContainer;
    }

    internal void OnRemoveFromInputActionsContainer(InputActionsContainer inputActionsContainer) {
        InputActionsContainer = null;
    }

    public void Enable(bool enabled = true) {
        if (enabled) {
            if (!Enabled) {
                Enabled = true;
                InputActionsContainer?.Enable(this);
                SetupGodotInputMap();
            }
        } else {
            Disable();
        }
    }

    public void Disable() {
        Enabled = false;
        InputActionsContainer?.Disable(this);
        if (Handler is ActionStateHandler stateHandler) stateHandler.ClearState();
        if (_configureGodotInputMap && InputMap.HasAction(Name)) InputMap.EraseAction(Name);
    }

    public void SetupGodotInputMap() {
        if (!_configureGodotInputMap) return;
        
        if (InputMap.HasAction(Name)) InputMap.EraseAction(Name);
        InputMap.AddAction(Name, DeadZone);

        CreateInputEvents().ForEach(e => InputMap.ActionAddEvent(Name, e));
    }

    private List<InputEvent> CreateInputEvents() {
        void AddModifiers(InputEventWithModifiers e) {
            e.ShiftPressed = Shift;
            e.AltPressed = Alt;
            if (CommandOrCtrl) {
                e.CommandOrControlAutoremap = true;
            } else {
                e.CtrlPressed = Ctrl;
                e.MetaPressed = Meta;
            }
        }
        
        List<InputEvent> events = new List<InputEvent>(Keys.Count + Buttons.Count + 1);
        foreach (var key in Keys) {
            var e = new InputEventKey();
            e.Keycode = key;
            AddModifiers(e);
            events.Add(e);
        }
        if (MouseButton != MouseButton.None) {
            var e = new InputEventMouseButton();
            e.ButtonIndex = MouseButton;
            AddModifiers(e);
            events.Add(e);
        }
        foreach (var button in Buttons) {
            var e = new InputEventJoypadButton();
            // e.Device = -1; // TODO: you can add a device id here
            e.ButtonIndex = button;
            events.Add(e);
        }

        if (Axis != JoyAxis.Invalid && AxisSign != 0) {
            var e = new InputEventJoypadMotion();
            e.Device = -1; // TODO: you can add a device id here
            e.Axis = Axis;
            e.AxisValue = AxisSign;
            events.Add(e);
        }
        return events;
    }

    public void LoadFromGodotProjectSettings() {
        if (!InputMap.HasAction(Name)) {
            GD.PushWarning($"{nameof(LoadFromGodotProjectSettings)}: Action {Name} not found in project");
            return;
        }
        
        foreach (var inputEvent in InputMap.ActionGetEvents(Name)) {
            if (inputEvent is InputEventKey key) {
                Keys.Add(key.Keycode);
            } else if (inputEvent is InputEventJoypadButton button) {
                Buttons.Add(button.ButtonIndex);
            } else if (inputEvent is InputEventJoypadMotion motion) {
                // TODO: feature missing, not tested!!!
                Axis = motion.Axis;
                AxisSign = (int)motion.AxisValue;
            } else if (inputEvent is InputEventMouseButton mouseButton) {
                MouseButton = mouseButton.ButtonIndex;
            }
        }
    }

    public InputAction SetSaveSettings(SaveSetting<string> saveSetting) {
        SaveSetting = saveSetting;
        return this;
    }

    private bool Matches(InputEvent e) =>
        e switch {
            InputEventKey key => HasKey(key.Keycode),
            InputEventMouseButton mouse => MouseButton == mouse.ButtonIndex,
            InputEventJoypadButton button => HasButton(button.ButtonIndex),
            InputEventJoypadMotion motion => motion.Axis == Axis,
            _ => false
        };

    public bool HasMouseButton() {
        return MouseButton != MouseButton.None;
    }

    public bool HasAxis() {
        return Axis != JoyAxis.Invalid;
    }

    public bool HasKey(Key key) {
        if (Keys.Count == 0) return false;
        if (Keys.Count == 1) return Keys[0] == key;
        for (var i = 2; i < Keys.Count; i++) if (Keys[i] == key) return true;
        return false;
    }

    public bool HasButton(JoyButton button) {
        if (Buttons.Count == 0) return false;
        if (Buttons.Count == 1) return Buttons[0] == button;
        for (var i = 2; i < Buttons.Count; i++) if (Buttons[i] == button) return true;
        return false;
    }

    public InputAction ResetToDefaults() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        Parse(SaveSetting.DefaultValue, true);
        return this;
    }
    
    public InputAction Load() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        Parse(SaveSetting.Value, true);
        return this;
    }
    
    public InputAction Save() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        SaveSetting.Value = AsString();
        return this;
    }

    public InputAction Update(Action<Updater> updater) {
        var (backupButtons, backupKeys, backupMouse) = (Buttons.ToArray(), Keys.ToArray(), MouseButton);
        var (axis, axisSign, backupDeadZone) = (Axis, AxisSign, DeadZone);
        var (commandOrCtrl, ctrl, shift, alt, meta) = (CommandOrCtrl, Ctrl, Shift, Alt, Meta);
        try {
            updater.Invoke(_updater);
            SetupGodotInputMap();
        } catch (Exception e) {
            _updater.SetButtons(backupButtons)
                .SetKeys(backupKeys)
                .SetMouse(backupMouse)
                .SetAxis(axis)
                .SetAxisSign(axisSign)
                .SetDeadZone(backupDeadZone)
                .WithCommandOrCtrl(commandOrCtrl)
                .WithCtrl(ctrl)
                .WithShift(shift)
                .WithAlt(alt)
                .WithMeta(meta);
        }
        return this;
    }

    public string AsString() {
        var export = new List<string>(Keys.Count + Buttons.Count + 1);
        export.AddRange(Keys.Select(key => $"Key:{key}"));
        export.AddRange(Buttons.Select(button => $"Button:{button}"));
        if (Axis != JoyAxis.Invalid) {
            export.Add($"Axis:{Axis}");
        }
        return string.Join(",", export);
    }

    public InputAction Parse(string input, bool reset) {
        if (string.IsNullOrWhiteSpace(input)) return this;
        Update(updater => {
            if (reset) updater.ClearAll();
            input.Split(",").ToList().ForEach(ImportItem);
        });
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
            Keys.Add(key);
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private bool ImportButton(string value) {
        try {
            var joyButton = int.TryParse(value, out _) ? (JoyButton)value.ToInt() : Parse<JoyButton>(value); 
            Buttons.Add(joyButton);
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private bool ImportAxis(string value) {
        try {
            var axis = int.TryParse(value, out _) ? (JoyAxis)value.ToInt() : Parse<JoyAxis>(value); 
            Axis = axis;
            return true;
        } catch (Exception) {
            return false;
        }
    }

    private static T Parse<T>(string key) => (T)Enum.Parse(typeof(T), key);
}