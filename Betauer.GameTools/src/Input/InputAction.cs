using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.Input.Handler;
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
    Mock,
    
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
    public static InputAction Mock(string? name = null) => new(
        name, 
        null,
        InputActionBehaviour.Mock,
        false,
        false,
        false,
        true);

    public static InputAction Simulate(string? name = null) => new(
        name,
        null,
        InputActionBehaviour.Simulate,
        false,
        false, 
        false, 
        true);


    // Usage
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
    public string Name { get; private set; }
    public bool Pausable { get; private set; } = false;
    public List<JoyButton> Buttons { get; } = new();
    public List<Key> Keys { get; } = new();
    public int JoypadId { get; private set; } = 0;
    public JoyAxis Axis { get; private set; } = JoyAxis.Invalid;
    public int AxisSign { get; private set; } = 1;
    public float DeadZone { get; private set; } = DefaultDeadZone;
    public MouseButton MouseButton { get; private set; } = MouseButton.None;
    public bool CommandOrCtrlAutoremap { get; private set; }
    public bool Ctrl { get; private set; }
    public bool Shift { get; private set; }
    public bool Alt { get; private set; }
    public bool Meta { get; private set; }
    public string? AxisActionName { get; internal set; }
    public InputActionBehaviour Behaviour { get; }
    public bool IsUnhandledInput { get; } = false;
    public bool Enabled { get; private set; } = true;
    private bool _godotInputMapEnabled = false;
    public bool GodotInputMapEnabled {
        get => _godotInputMapEnabled || Behaviour == InputActionBehaviour.GodotInput;
        set => _godotInputMapEnabled = value || Behaviour == InputActionBehaviour.GodotInput;
    }

    public AxisAction? AxisAction { get; internal set; }

    public InputActionsContainer? InputActionsContainer { get; private set; }
    public SaveSetting<string>? SaveSetting { get; set; }
    internal readonly IHandler Handler;
    private readonly Updater _updater;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="axisActionName"></param>
    /// <param name="behaviour"></param>
    /// <param name="configureGodotInputMap">Forced to true if Behaviour is GodotInput</param>
    /// <param name="isUnhandledInput">Only when Behaviour is Extended</param>
    /// <param name="pausable">Only when Behaviour is Extended</param>
    /// <param name="enabled"></param>
    private InputAction(
        string name,
        string? axisActionName,
        InputActionBehaviour behaviour,
        bool configureGodotInputMap = false,
        bool isUnhandledInput = false, 
        bool pausable = false,
        bool enabled = true) {

        Name = name;
        AxisActionName = axisActionName;
        Behaviour = behaviour;
        GodotInputMapEnabled = configureGodotInputMap;
        IsUnhandledInput = isUnhandledInput;
        Pausable = pausable;
        Enabled = enabled;

        Handler = behaviour switch {
            InputActionBehaviour.Mock => new MockStateHandler(this),
            InputActionBehaviour.Simulate => new FrameStateHandler(this),
            InputActionBehaviour.GodotInput => new GodotInputHandler(this),
            InputActionBehaviour.Extended => new ExtendedInputHandler(this),
        };
        _updater = new Updater(this);
    }

    /// <summary>
    /// Cloned need to be updated later with Update(u => u.CopyAll(input)). Cloned doesn't have save setting
    /// </summary>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public InputAction Clone(string suffix) {
        var name = $"{Name}/{suffix}";
        var axisName = AxisActionName != null ? $"{AxisActionName}/{suffix}" : null;
        var inputAction = new InputAction(name, axisName, Behaviour, GodotInputMapEnabled, IsUnhandledInput, Pausable, Enabled) {
            Container = Container,
        };
        return inputAction;
    }

    [Inject] private Container Container { get; set; }
    private string _inputActionsContainerName;
    private string? _settingsContainerName;
    private string? _settingSaveAs;
    private bool _settingAutoSave;

    public void PreInject(string? name, string? axisActionName, string inputActionsContainerName, string? settingsContainerName, string? saveAs, bool autoSave) {
        if (Name == null) Name = name;
        if (AxisActionName == null) AxisActionName = axisActionName;
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
        RefreshGodotInputMap();
    }

    private void CreateSaveSettings(string settingsContainerName, string propertyName, bool autoSave = false, bool enabled = true) {
        var setting = Setting.Create(propertyName, AsString(), autoSave, enabled);
        setting.PreInject(settingsContainerName);
        Container.InjectServices(setting);
        SaveSetting = setting;
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
            if (Enabled) return;
            Enabled = true;
            InputActionsContainer?.EnableAction(this);
            RefreshGodotInputMap();
        } else {
            if (!Enabled) return;
            Enabled = false;
            InputActionsContainer?.DisableAction(this);
            if (Handler is FrameStateHandler stateHandler) stateHandler.ClearState();
            RefreshGodotInputMap();
        }
    }

    public void Disable() => Enable(false);

    public void RefreshGodotInputMap() {
        if (!GodotInputMapEnabled) return;

        var stringName = (StringName)Name;
        if (InputMap.HasAction(stringName)) {
            InputMap.EraseAction(stringName);
        }
        if (Enabled) {
            InputMap.AddAction(stringName, DeadZone);
            CreateInputEvents().ForEach(e => InputMap.ActionAddEvent(stringName, e));
        }
    }

    private List<InputEvent> CreateInputEvents() {
        void AddModifiers(InputEventWithModifiers e) {
            e.ShiftPressed = Shift;
            e.AltPressed = Alt;
            if (CommandOrCtrlAutoremap) {
                e.CommandOrControlAutoremap = true;
            } else {
                e.CtrlPressed = Ctrl;
                e.MetaPressed = Meta;
            }
        }
        
        List<InputEvent> events = new List<InputEvent>(Keys.Count + Buttons.Count + 1);
        foreach (var key in Keys) {
            var e = new InputEventKey();
            // TODO: if (KeyboardDeviceId >= 0) e.Device = KeyboardDeviceId;
            e.Keycode = key;
            AddModifiers(e);
            events.Add(e);
        }
        if (MouseButton != MouseButton.None) {
            var e = new InputEventMouseButton();
            // TODO: if (MouseDeviceId >= 0) e.Device = MouseDeviceId;
            e.ButtonIndex = MouseButton;
            AddModifiers(e);
            events.Add(e);
        }
        foreach (var button in Buttons) {
            var e = new InputEventJoypadButton();
            if (JoypadId >= 0) e.Device = JoypadId;
            e.ButtonIndex = button;
            events.Add(e);
        }

        if (Axis != JoyAxis.Invalid && AxisSign != 0) {
            var e = new InputEventJoypadMotion();
            if (JoypadId >= 0) e.Device = JoypadId;
            e.Axis = Axis;
            e.AxisValue = AxisSign;
            events.Add(e);
        }
        return events;
    }

    public void LoadFromGodotProjectSettings() {
        var stringName = (StringName)Name;
        if (!InputMap.HasAction(stringName)) {
            GD.PushWarning($"{nameof(LoadFromGodotProjectSettings)}: Action {Name} not found in project");
            return;
        }
        
        foreach (var inputEvent in InputMap.ActionGetEvents(stringName)) {
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

    public bool MatchesModifiers(InputEventWithModifiers modifiers) {
        if (Shift && !modifiers.ShiftPressed) return false;
        if (Alt && !modifiers.AltPressed) return false;
        if (CommandOrCtrlAutoremap) {
            modifiers.CommandOrControlAutoremap = true;
            if (!modifiers.IsCommandOrControlPressed()) return false;
        } else {
            if (Ctrl && !modifiers.CtrlPressed) return false;
            if (Meta && !modifiers.MetaPressed) return false;
        }
        return true;
    }

    private bool Matches(InputEvent e) {
        return e switch {
            InputEventKey key => MatchesModifiers(key) && HasKey(key.Keycode),
            InputEventMouseButton mouse => MatchesModifiers(mouse) && MouseButton == mouse.ButtonIndex,
            InputEventJoypadButton button => IsJoypadId(e.Device) && HasButton(button.ButtonIndex),
            InputEventJoypadMotion motion => IsJoypadId(e.Device) && motion.Axis == Axis,
            _ => false
        };
    }

    public bool IsJoypadId(int device) => (JoypadId < 0 || JoypadId == device);

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

    public void ResetToDefaults() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        Parse(SaveSetting.DefaultValue, true);
    }
    
    public void Load() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        Parse(SaveSetting.Value, true);
    }
    
    public void Save() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        SaveSetting.Value = AsString();
    }

    public InputAction Update(Action<Updater> updater) {
        var (backupButtons, backupKeys, backupMouse) = (Buttons.ToArray(), Keys.ToArray(), MouseButton);
        var (axis, axisSign, backupDeadZone, joypadId) = (Axis, AxisSign, DeadZone, JoypadId);
        var (commandOrCtrlAutoremap, ctrl, shift, alt, meta) = (CommandOrCtrl: CommandOrCtrlAutoremap, Ctrl, Shift, Alt, Meta);
        try {
            updater.Invoke(_updater);
            RefreshGodotInputMap();
        } catch (Exception e) {
            _updater.SetButtons(backupButtons)
                .SetKeys(backupKeys)
                .SetMouse(backupMouse)
                .SetAxis(axis)
                .SetAxisSign(axisSign)
                .SetDeadZone(backupDeadZone)
                .SetJoypadId(joypadId)
                .WithCommandOrCtrlAutoremap(commandOrCtrlAutoremap)
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

    public void Parse(string input, bool reset) {
        if (string.IsNullOrWhiteSpace(input)) return;
        Update(updater => {
            if (reset) updater.ClearAll();
            input.Split(",").ToList().ForEach(ImportItem);
        });
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