using System;
using Betauer.Application.Settings;
using Betauer.Core;
using Godot;

namespace Betauer.Input;

public class AxisAction : IAction {
    public float Strength => (Positive.Strength - Negative.Strength) * (Reverse ? -1 : 1);
    public float RawStrength => (Positive.RawStrength - Negative.RawStrength) * (Reverse ? -1 : 1);
    public JoyAxis Axis => Positive.Axis;
    public bool Reverse { get; set; } = false;
    public string Name { get; internal set; }
    public bool Enabled => Negative is { Enabled: true } && Positive is { Enabled: true }; 
    public SaveSetting<string>? SaveSetting { get; set; }
    public bool IsEvent(InputEvent inputEvent) => inputEvent is InputEventJoypadMotion motion && motion.Axis == Axis;
    public void Enable(bool enable) {
        Negative.Enable(enable);
        Positive.Enable(enable);
    }
    
    public InputActionsContainer? InputActionsContainer { get; private set; }
    public InputAction Negative { get; private set; }
    public InputAction Positive { get; private set; }

    public AxisAction Clone(string suffix) {
        var axisAction = new AxisAction($"{Name}/{suffix}") {
            Reverse = Reverse,
        };
        return axisAction;
    }

    public AxisAction(string name) {
        Name = name;
    }
    
    public AxisAction(string name, InputAction left, InputAction right) {
        Name = name;
        SetNegativeAndPositive(left, right);
    }

    public void SetNegativeAndPositive(InputAction negative, InputAction positive) {
        if (positive == null || negative == null) throw new Exception("AxisAction must have both positive and negative actions");
        if (negative.Axis == JoyAxis.Invalid) throw new InvalidAxisConfigurationException($"InputAction {negative.Name} should define a valid Axis.");
        if (positive.Axis == JoyAxis.Invalid) throw new InvalidAxisConfigurationException($"InputAction {positive.Name} should define a valid Axis.");
        
        // Ensure sign is 1 or -1
        if (Math.Abs(negative.AxisSign) != 1) throw new InvalidAxisConfigurationException($"InputAction {negative.Name} AxisSign should be 1 or -1. Wrong value: {negative.AxisSign}");
        if (Math.Abs(positive.AxisSign) != 1) throw new InvalidAxisConfigurationException($"InputAction {positive.Name} AxisSign should be 1 or -1. Wrong value: {positive.AxisSign}");
        if (positive.AxisSign == negative.AxisSign) {
            throw new InvalidAxisConfigurationException($"InputAction {negative.Name} and {positive.Name} can't have the same AxisSign {positive.AxisSign}. One must be -1 and other 1");
        }

        // Swap if needed
        if (positive.AxisSign < 0) (negative, positive) = (positive, negative);
        
        Positive?.UnsetInputActionsContainer();
        Negative?.UnsetInputActionsContainer();
        if (Positive != null) Positive.AxisAction = null;
        if (Negative != null) Negative.AxisAction = null;
        
        Negative = negative;
        Positive = positive;
        Positive.AxisAction = this;
        Negative.AxisAction = this;
        Positive.AxisName = Name;
        Negative.AxisName = Name;
        if (InputActionsContainer != null) {
            Positive.SetInputActionsContainer(InputActionsContainer);
            Negative.SetInputActionsContainer(InputActionsContainer);
        }
    }

    public void CreateSaveSetting(SettingsContainer settingsContainer, string saveAs) {
        SaveSetting = Setting.Create(saveAs, AsString(), true, true);
        SaveSetting.SetSettingsContainer(settingsContainer);
        Load();
    }

    public void UnsetInputActionsContainer() {
        InputActionsContainer?.Remove(this);
        InputActionsContainer = null;
        Positive?.UnsetInputActionsContainer();
        Negative?.UnsetInputActionsContainer();
    }

    public void SetInputActionsContainer(InputActionsContainer inputActionsContainer) {
        if (InputActionsContainer != null && InputActionsContainer != inputActionsContainer) {
            UnsetInputActionsContainer();
        }
        InputActionsContainer = inputActionsContainer;
        InputActionsContainer.Add(this); 
        Positive?.SetInputActionsContainer(inputActionsContainer);
        Negative?.SetInputActionsContainer(inputActionsContainer);
    }

    public string AsString() {
        return $"Reverse:{Reverse}";
    }

    public void Parse(string values, bool reset) {
        if (reset) Reverse = false;
        values.Split(',').ForEach(value => {
            if (value.Contains(':')) {
                var parts = value.Split(':');
                var key = parts[0];
                var val = parts[1];
                switch (key) {
                    case "Reverse":
                        Reverse = bool.Parse(val);
                        break;
                }
            }
        });
    }

    public void ResetToDefaults() {
        if (SaveSetting == null) throw new Exception("AxisAction does not have a SaveSetting");
        Parse(SaveSetting.DefaultValue, true);
    }
    
    public void Load() {
        if (SaveSetting == null) throw new Exception("AxisAction does not have a SaveSetting");
        Parse(SaveSetting.Value, true);
    }
    
    public void Save() {
        if (SaveSetting == null) throw new Exception("AxisAction does not have a SaveSetting");
        SaveSetting.Value = AsString();
        if (!SaveSetting.AutoSave) SaveSetting.SettingsContainer?.Save();
    }
    
    public void SimulatePress(float strength) {
        if (strength == 0f) {
            if (Strength != 0) SimulateRelease();
        } else {
            if (strength > 0) {
                Negative.SimulateRelease();
                Positive.SimulatePress(strength);
            } else {
                Negative.SimulatePress(Mathf.Abs(strength));
                Positive.SimulateRelease();
            }
        }
    }

    public void SimulateRelease() {
        if (Strength >= 0) Positive.SimulateRelease();
        if (Strength <= 0) Negative.SimulateRelease();
    }

    public void ClearJustStates() {
        Negative.ClearJustStates();        
        Positive.ClearJustStates();        
    }

    public static AxisAction Mock(JoyAxis joyAxis = JoyAxis.LeftX) {
        var positive = InputAction.Create().PositiveAxis(joyAxis).AsMock();
        var negative = InputAction.Create().NegativeAxis(joyAxis).AsMock();
        var axisAction = new AxisAction(null);
        axisAction.SetNegativeAndPositive(negative, positive);
        return axisAction;
    }

    public static Builder Create(string name = null) {
        return new Builder(name);
    }

    public class Builder {
        private readonly string _name;
        private InputAction? _positive;
        private InputAction? _negative;
        private bool _reverse = false;

        public Builder(string name) {
            _name = name;
        }
        
        public Builder Positive(InputAction positive) {
            _positive = positive;
            return this;
        }
        
        public Builder Negative(InputAction negative) {
            _negative = negative;
            return this;
        }

        public Builder ReverseAxis(bool reverse = true) {
            _reverse = reverse;
            return this;
        }
        
        public AxisAction Build() {
            var axisAction = new AxisAction(_name);
            if (_positive != null) {
                if (_negative == null) {
                    throw new Exception("AxisAction must have both positive and negative actions");
                }
                axisAction.SetNegativeAndPositive(_negative, _positive);
            }
            axisAction.Reverse = _reverse;
            return axisAction;
        }
    }
}