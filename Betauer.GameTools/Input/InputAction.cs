using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input;

public enum InputActionBehaviour {
    /// <summary>
    /// It works only through Simulate*() methods but:
    /// - WasPressed() and WasReleased() always return false
    /// - JustPressed is true only the first call to SimulatePress() when the action is not pressed.
    /// - JustReleased is true only the first call to SimulateRelease() when the action was pressed.
    /// - ClearJustStates() sets JustPressed and JustReleased to false
    /// 
    /// * How to simulate JustPressed and JustReleased for more than one frame:
    /// Call to ClearJustStates() after every frame to simulate a button pressed in multiple frames. Then you can use
    /// SimulateRelease() in a later frame to simulate a JustRelease.
    /// - Frame 1: SimulatePress()
    ///            [your code here] JustPressed and Pressed are true
    ///            ClearJustStates()
    /// - Frame 2: [your code here] JustPressed is false. Pressed is true
    ///            ClearJustStates()
    /// - Frame 3: SimulateRelease()
    ///            [your code here] JustRelease is true. Pressed is false
    ///            ClearJustStates()
    /// - Frame 3: [your code here] JustRelease and Pressed are false.
    ///            ClearJustStates()
    /// 
    /// * If you don't care about button pressed in multiple frames, just call SimulateRelease after every frame.
    /// - Frame 1: SimulatePress()
    ///            [your code here] JustPressed and Pressed are true
    ///            SimulateRelease()
    /// - Frame 2: [your code here] JustRelease is true. Pressed is false
    ///            SimulateRelease()
    /// - Frame 3: [your code here] JustRelease and Pressed are false.
    ///            SimulateRelease()
    /// 
    /// * To force a JustPressed (no matter if the action is pressed or it isn't) call to ClearJustStates() and SimulatePress().
    /// </summary>
    Fake,
    
    /// <summary>
    /// It works only through Simulate*() methods.
    /// - JustPressed and JustRelease uses the current process/physics frame to check if the button is just pressed or
    /// just released in the same frame.
    /// - ClearJustStates() makes the JustPressed, JustReleased, WasPressed() and WasReleased() returns false setting the frame to 0 and the PressedTime/ReleaseTime one year ago
    /// - WasPressed() and WasReleased() works
    /// </summary>
    Simulate,
    
    /// <summary>
    /// It uses the Godot Input singleton to handle the action.
    /// - WasPressed() and WasReleased() always return false
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
public partial class InputAction : IAction, IInjectable {
    public const float DefaultDeadZone = 0.5f;
    public static Builder Create() => new(null);
    public static Builder Create(string name) => new(name);

    // Usage
    public string Name { get; private set; }
    public bool IsPressed => Handler.Pressed;
    public bool IsJustPressed => Handler.JustPressed;
    public bool IsJustReleased => Handler.JustReleased;
    public float PressedTime => Handler.PressedTime;
    public float ReleasedTime => Handler.ReleasedTime;
    public float Strength => Handler.Strength;
    public float RawStrength => Handler.RawStrength;
    public void SimulatePress(float strength = 1f) => Handler.SimulatePress(strength);
    public void SimulateRelease() => Handler.SimulateRelease();
    public void ClearJustStates() => Handler.ClearJustStates();

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
    
    
    public string? AxisActionName { get; internal set; }
    public AxisAction? AxisAction { get; internal set; }

    public InputActionsContainer? InputActionsContainer { get; private set; }
    public InputActionBehaviour Behaviour { get; }
    public bool IsUnhandledInput { get; } = false;
    public bool Enabled { get; private set; } = true;
    public SaveSetting<string>? SaveSetting { get; private set; }
    internal readonly IHandler Handler;
    private readonly Updater _updater;
    private readonly bool _configureGodotInputMap = false;

    public static InputAction Fake() => new InputAction(null,
        false,
        InputActionBehaviour.Fake,
        false,
        false);

    public static InputAction Simulate() => new InputAction(null,
        false,
        InputActionBehaviour.Simulate,
        false,
        false);

    private InputAction( 
        string name, 
        bool keepProjectSettings,
        InputActionBehaviour behaviour, 
        bool configureGodotInputMap,
        bool isUnhandledInput) {
        Name = name;
        Behaviour = behaviour;
        _updater = new Updater(this);
        Enabled = true;
        _configureGodotInputMap = behaviour == InputActionBehaviour.GodotInput || configureGodotInputMap;
        IsUnhandledInput = isUnhandledInput;

        Handler = behaviour switch {
            InputActionBehaviour.Fake => new FakeStateHandler(this),
            InputActionBehaviour.Simulate => new FrameBasedStateHandler(this),
            InputActionBehaviour.GodotInput => new GodotInputHandler(this),
            InputActionBehaviour.Extended => new ExtendedInputFrameBasedStateHandler(this),
        };
        _configureGodotInputMap = true;
        if (keepProjectSettings) {
            LoadFromGodotProjectSettings();
        }
        // Don't call SetupGodotInputMap here. It's better to wait until PostInject() load the saved setting
    }

    [Inject] private Container Container { get; set; }
    private string _inputActionsContainerName;
    private string? _settingsContainerName;
    private string? _settingSaveAs;
    private bool _settingAutoSave;

    public void PreInject(string? name, string? axisName, string inputActionsContainerName, string? settingsContainerName, string? saveAs, bool autoSave) {
        if (Name == null) Name = name;
        AxisActionName = axisName;
        _inputActionsContainerName = inputActionsContainerName;
        _settingsContainerName = settingsContainerName;
        _settingSaveAs = saveAs;
        _settingAutoSave = autoSave;
    }

    public void PostInject() {
        if (_settingsContainerName != null && _settingSaveAs != null) {
            CreateSaveSettings(_settingsContainerName, _settingSaveAs, _settingAutoSave, true);
            // Load settings from file
            Load();
        }

        SetInputActionsContainer(Container.Resolve<InputActionsContainer>(_inputActionsContainerName));
        SetupGodotInputMap();
    }

    private void CreateSaveSettings(string settingsContainerName, string propertyName, bool autoSave = false, bool enabled = true) {
        var setting = Setting.Create(propertyName, AsString(), autoSave, enabled);
        setting.PreInject(settingsContainerName);
        Container.InjectServices(setting);
        SetSaveSettings(setting);
    }

    public void UnsetInputActionsContainer() {
        InputActionsContainer?.Remove(this);
        InputActionsContainer = null;
    }

    public void SetInputActionsContainer(InputActionsContainer inputActionsContainer) {
        if (InputActionsContainer != null && InputActionsContainer != inputActionsContainer) {
            UnsetInputActionsContainer();
        }
        InputActionsContainer = inputActionsContainer;
        InputActionsContainer.Add(this);
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
        if (Handler is FrameBasedStateHandler stateHandler) stateHandler.ClearState();
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