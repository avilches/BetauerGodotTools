using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.Input.Handler;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Input;

public enum InputActionBehaviour {
    /// <summary>
    /// - It works only through Simulate*() methods
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
    /// It uses the Godot Input singleton to handle the action (WasPressed() and WasReleased() always return false)
    /// </summary>
    GodotInput,
    
    /// <summary>
    /// It add WasPressed() and WasReleased() methods.
    /// </summary>
    Extended,
}

public partial class InputAction {
    public static readonly Logger Logger = LoggerFactory.GetLogger<InputAction>();
    public const float DefaultDeadZone = 0.5f;
    public static Builder Create(string name) => new(name);
    public static InputAction Mock(string? name = null, string? saveAs = null) {
        return new InputAction(name, null, InputActionBehaviour.Mock, saveAs) {
            Enabled = true
        };
    }

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
    public string Name { get; internal set; }
    public List<JoyButton> Buttons { get; } = new();
    public List<Key> Keys { get; } = new();
    public int JoypadId { get; private set; } = -1;
    public JoyAxis Axis { get; private set; } = JoyAxis.Invalid;
    public int AxisSign { get; private set; } = 1;
    public float DeadZone { get; private set; } = DefaultDeadZone;
    public MouseButton MouseButton { get; private set; } = MouseButton.None;
    public bool CommandOrCtrlAutoremap { get; private set; }
    public bool Ctrl { get; private set; }
    public bool Shift { get; private set; }
    public bool Alt { get; private set; }
    public bool Meta { get; private set; }
    public string? AxisName { get; internal set; }
    public InputActionBehaviour Behaviour { get; }
    public bool Enabled { get; private set; } = false;

    public AxisAction? AxisAction { get; internal set; }

    public string? SaveAs { get; private set; }
    private SaveSetting<string>? _saveSetting;
    public SaveSetting<string>? SaveSetting {
        get => _saveSetting;
        set {
            _saveSetting = value;
            SaveAs = _saveSetting.SaveAs;
        }
    }

    internal readonly IHandler Handler;
    private readonly Updater _updater;

    public event Action<bool> OnEnable;
    public event Action OnUpdate;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="axisName"></param>
    /// <param name="behaviour"></param>
    /// <param name="enabled"></param>
    private InputAction(
        string name,
        string? axisName,
        InputActionBehaviour behaviour, string? saveAs) {

        Name = name;
        AxisName = axisName;
        Behaviour = behaviour;
        SaveAs = saveAs;

        Handler = behaviour switch {
            InputActionBehaviour.Mock => new MockStateHandler(this),
            InputActionBehaviour.GodotInput => new GodotInputHandler(this, false),
            InputActionBehaviour.Extended => new GodotInputHandler(this, true),
        };
        _updater = new Updater(this);
    }

    internal void ChangeName(string newName) {
        Name = newName;
        var stringName = (StringName)Name;
        if (InputMap.HasAction(stringName)) {
            InputMap.EraseAction(stringName);
        }
        Name = newName;
        RefreshGodotInputMap();
    }
    
    public void CreateSaveSetting(SettingsContainer settingsContainer, string? saveAs = null) {
        if (saveAs != null) SaveAs = saveAs;
        SaveSetting = Setting.Create(SaveAs, AsString(), true);
        settingsContainer.Add(SaveSetting);
        Load();
    }

    public void Enable(bool enable = true) {
        if (enable == Enabled) return;
        Enabled = enable;
        RefreshGodotInputMap();
        OnEnable?.Invoke(enable);
    }

    public void Disable() => Enable(false);

    public void RefreshGodotInputMap() {
        Handler.Refresh(this);
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
        SaveSetting.Refresh(); // Since multiple SaveSetting could share the same SettingsContainer and same SaveAs property, we need to refresh it
        Parse(SaveSetting.Value, true);
    }
    
    public void Save() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        SaveSetting.Value = AsString();
        
        if (!SaveSetting.AutoSave) SaveSetting.SettingsContainer!.Save();
    }

    public InputAction Update(Action<Updater> updater) {
        var (backupButtons, backupKeys, backupMouse) = (Buttons.ToArray(), Keys.ToArray(), MouseButton);
        var (axis, axisSign, backupDeadZone, joypadId) = (Axis, AxisSign, DeadZone, JoypadId);
        var (commandOrCtrlAutoremap, ctrl, shift, alt, meta) = (CommandOrCtrl: CommandOrCtrlAutoremap, Ctrl, Shift, Alt, Meta);
        try {
            updater.Invoke(_updater);
            RefreshGodotInputMap();
            OnUpdate?.Invoke();
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