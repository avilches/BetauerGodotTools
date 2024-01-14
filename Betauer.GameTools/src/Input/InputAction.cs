using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Application.Settings;
using Betauer.Core;
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
    Simulator,
    
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
    public static InputAction Simulator(string? name = null, string? saveAs = null) {
        return new InputAction(name, null, InputActionBehaviour.Simulator, saveAs) {
            Enabled = true
        };
    }

    public enum AxisSignEnum {
        Positive = 1,
        Negative = -1,
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

    // Updateable Input configuration 
    public string Name { get; internal set; }
    public List<JoyButton> Buttons { get; } = new();
    public List<Key> Keys { get; } = new();
    public JoyAxis Axis { get; private set; } = JoyAxis.Invalid;
    public AxisSignEnum AxisSign { get; private set; } = AxisSignEnum.Positive;
    public float DeadZone { get; private set; } = DefaultDeadZone;
    public MouseButton MouseButton { get; private set; } = MouseButton.None;
    public bool CommandOrCtrlAutoremap { get; private set; }
    public bool Ctrl { get; private set; }
    public bool Shift { get; private set; }
    public bool Alt { get; private set; }
    public bool Meta { get; private set; }
    public string? AxisName { get; internal set; }

    // Joypad
    private int _joypadId = -1;
    public int JoypadId {
        get => _joypadId;
        set {
            if (_joypadId == value) return;
            _joypadId = value;
            RefreshGodotInputMap();
        }
    }

    public InputActionBehaviour Behaviour { get; }
    public bool Enabled { get; private set; } = false;
    public AxisAction? AxisAction { get; internal set; }

    // Needed for Load() and Save()
    private bool _allowMultipleButtons = true;
    public bool AllowMultipleButtons {
        get => _allowMultipleButtons;
        set {
            _allowMultipleButtons = value;
            if (!_allowMultipleButtons && Buttons.Count > 1) {
                Buttons.RemoveRange(1, Buttons.Count - 1);
            }
        }
    }

    private bool _allowMultipleKeys = true;
    public bool AllowMultipleKeys {
        get => _allowMultipleKeys;
        set {
            _allowMultipleKeys = value;
            if (!_allowMultipleKeys && Keys.Count > 1) {
                Keys.RemoveRange(1, Keys.Count - 1);
            }
        }
    }

    public bool IncludeAxisSign { get; set; } = false;
    public bool IncludeDeadZone { get; set; } = false;
    public bool IncludeModifiers { get; set; } = true;
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
            InputActionBehaviour.Simulator => new MockStateHandler(this),
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
        SaveSetting = Setting.Create(SaveAs, Export(), true);
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
                AxisSign = motion.AxisValue > 0 ? AxisSignEnum.Positive : AxisSignEnum.Negative;
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

    public bool HasMouseButton() => MouseButton != MouseButton.None;

    public bool HasAxis() => Axis != JoyAxis.Invalid;

    public bool HasKeys() => Keys.Count > 0;
    
    public bool HasButtons() => Buttons.Count > 0;
    
    public bool HasKey(Key key) =>  Keys.Contains(key);

    public bool HasButton(JoyButton button) => Buttons.Contains(button);

    public void Load() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        SaveSetting.Refresh(); // Since multiple SaveSetting could share the same SettingsContainer and same SaveAs property, we need to refresh it
        Update(u => {
            u.ImportJoypad(SaveSetting.Value);
            u.ImportKeys(SaveSetting.Value);
            u.ImportMouse(SaveSetting.Value);
        });
    }

    public void Save() {
        if (SaveSetting == null) throw new Exception("InputAction does not have a SaveSetting");
        SaveSetting.Value = Export();
        
        if (!SaveSetting.AutoSave) SaveSetting.SettingsContainer!.Save();
    }

    public InputAction Update(Action<Updater> updater) {
        var (backupButtons, backupKeys, backupMouse) = (Buttons.ToArray(), Keys.ToArray(), MouseButton);
        var (axis, axisSign, backupDeadZone) = (Axis, AxisSign, DeadZone);
        var (commandOrCtrlAutoremap, ctrl, shift, alt, meta) = (CommandOrCtrlAutoremap, Ctrl, Shift, Alt, Meta);
        try {
            updater.Invoke(_updater);
            RefreshGodotInputMap();
            OnUpdate?.Invoke();
        } catch (Exception) {
            _updater.SetButtons(backupButtons)
                .SetKeys(backupKeys)
                .SetMouse(backupMouse)
                .SetAxis(axis)
                .SetAxisSign(axisSign)
                .SetDeadZone(backupDeadZone)
                .WithCommandOrCtrlAutoremap(commandOrCtrlAutoremap)
                .WithCtrl(ctrl)
                .WithShift(shift)
                .WithAlt(alt)
                .WithMeta(meta);
        }
        return this;
    }

    public string Export() {
        var export = new List<string>();
        if (HasButtons()) {
            Buttons.Where(button => button != JoyButton.Invalid)
                .Select(button => $"Button:{button}")
                .ForEach(export.Add);
        }
        if (HasAxis()) {
            export.Add($"JoyAxis:{Axis}");
            if (IncludeAxisSign) export.Add($"AxisSign:{AxisSign}");
            if (IncludeDeadZone) export.Add($"DeadZone:{DeadZone.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        }
        if (HasKeys()) {
            Keys.Where(key => key != Key.Unknown && key != Key.None)
                .Select(key => $"Key:{key}")
                .ForEach(export.Add);
        }
        if (HasMouseButton()) {
            export.Add($"Mouse:{MouseButton}");
        }
        if (IncludeModifiers && (HasKeys() || HasMouseButton())) {
            if (Alt) export.Add("Alt");
            if (Shift) export.Add("Shift");
            if (Ctrl) export.Add("Ctrl");
            if (Meta) export.Add("Meta");
        }
        return string.Join(",", export);
    }
}