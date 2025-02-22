using System;
using Betauer.Core;
using Godot;

namespace Betauer.Input;

public class AxisAction {
    public float Strength => (Positive.Strength - Negative.Strength) * (Reverse ? -1 : 1);
    public float RawStrength => (Positive.RawStrength - Negative.RawStrength) * (Reverse ? -1 : 1);
    public JoyAxis Axis => Positive.Axis;
    public bool Reverse { get; set; } = false;
    public string Name { get; internal set; }
    public bool Enabled => Negative is { Enabled: true } && Positive is { Enabled: true }; 
    
    public InputAction Negative { get; private set; }
    public InputAction Positive { get; private set; }

    public string? SaveAs { get; }

    public bool IsEvent(InputEvent inputEvent) => inputEvent is InputEventJoypadMotion motion && motion.Axis == Axis;
    public void Disable() => Enable(false);
    public void Enable(bool enable = true) {
        Negative?.Enable(enable);
        Positive?.Enable(enable);
    }
    
    public AxisAction(string name, string? saveAs = null) {
        Name = name;
        SaveAs = saveAs;
    }

    public void SetNegativeAndPositive(InputAction negative, InputAction positive) {
        if (positive == null || negative == null) throw new Exception("AxisAction must have both positive and negative actions");
        if (negative.Axis == JoyAxis.Invalid) throw new InvalidAxisConfigurationException($"InputAction {negative.Name} should define a valid Axis.");
        if (positive.Axis == JoyAxis.Invalid) throw new InvalidAxisConfigurationException($"InputAction {positive.Name} should define a valid Axis.");
        
        // Ensure sign is 1 or -1
        if (positive.AxisSign == negative.AxisSign) {
            throw new InvalidAxisConfigurationException($"InputAction {negative.Name} and {positive.Name} can't have the same AxisSign {positive.AxisSign}. One must be Positive and other Negative");
        }

        // Swap if needed
        if (positive.AxisSign < 0) (negative, positive) = (positive, negative);
        
        if (Positive != null) Positive.AxisAction = null;
        if (Negative != null) Negative.AxisAction = null;
        
        Negative = negative;
        Positive = positive;
        Positive.AxisAction = this;
        Positive.AxisName = Name;
        Negative.AxisAction = this;
        Negative.AxisName = Name;
    }

    public string Export() {
        return $"Reverse:{Reverse}";
    }

    public void Import(string values, bool reset) {
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
        var positive = InputAction.Create(null).PositiveAxis(joyAxis).Simulator();
        var negative = InputAction.Create(null).NegativeAxis(joyAxis).Simulator();
        var axisAction = new AxisAction(null);
        axisAction.SetNegativeAndPositive(negative, positive);
        return axisAction;
    }

    public static Builder Create(string name) {
        return new Builder(name);
    }

    public class Builder {
        private readonly string _name;
        private string _saveAs;
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
        
        public Builder SaveAs(string saveAs) {
            _saveAs = saveAs;
            return this;
        }
        
        public AxisAction Build() {
            var axisAction = new AxisAction(_name, _saveAs);
            if (_positive != null || _negative != null) {
                if (_positive == null ||_negative == null) {
                    throw new Exception("AxisAction must have both positive and negative actions");
                }
                axisAction.SetNegativeAndPositive(_negative, _positive);
            }
            axisAction.Reverse = _reverse;
            return axisAction;
        }
    }
}