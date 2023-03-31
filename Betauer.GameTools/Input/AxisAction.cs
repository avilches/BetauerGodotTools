using System;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input;

public class AxisAction : IAction, IInjectable {
    public float Strength => (Positive.Strength - Negative.Strength) * (Reverse ? -1 : 1);
    public float RawStrength => (Positive.RawStrength - Negative.RawStrength) * (Reverse ? -1 : 1);
    public JoyAxis Axis => Positive.Axis;
    public bool Reverse { get; set; } = false; // TODO: load a setting container and allow to export it. Optional: define a axis deadzone
    public string Name { get; private set; }
    public bool IsEvent(InputEvent inputEvent) => inputEvent is InputEventJoypadMotion motion && motion.Axis == Axis;
    public void Enable(bool enabled) {
        Negative.Enable(enabled);
        Positive.Enable(enabled);
    }
    
    public InputActionsContainer? InputActionsContainer { get; private set; }
    public InputAction Negative { get; private set; }
    public InputAction Positive { get; private set; }

    private AxisAction(string name) {
        Name = name;
    }
    
    public void SetNegativeAndPositive(InputAction negative, InputAction positive) {
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
        Positive.AxisActionName = Name;
        Negative.AxisActionName = Name;
        if (InputActionsContainer != null) {
            Positive.SetInputActionsContainer(InputActionsContainer);
            Negative.SetInputActionsContainer(InputActionsContainer);
        }
    }

    [Inject] private Container Container { get; set; }  
    private string? _inputActionsContainerName;
    public void PreInject(string name, string inputActionsContainerName) {
        if (Name == null) Name = name;
        _inputActionsContainerName = inputActionsContainerName;
    }

    public void PostInject() {
        SetInputActionsContainer(Container.Resolve<InputActionsContainer>(_inputActionsContainerName!));
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

    public static AxisAction Fake(JoyAxis joyAxis = JoyAxis.LeftX) {
        var positive = InputAction.Create().PositiveAxis(joyAxis).AsFake();
        var negative = InputAction.Create().NegativeAxis(joyAxis).AsFake();
        var axisAction = new AxisAction(null);
        axisAction.SetNegativeAndPositive(negative, positive);
        return axisAction;
    }

    public static AxisAction Simulate(JoyAxis joyAxis = JoyAxis.LeftX) {
        var positive = InputAction.Create().PositiveAxis(joyAxis).AsSimulator();
        var negative = InputAction.Create().NegativeAxis(joyAxis).AsSimulator();
        var axisAction = new AxisAction(null);
        axisAction.SetNegativeAndPositive(negative, positive);
        return axisAction;
    }

    public static AxisAction Create(string name = null) {
        var axisAction = new AxisAction(name);
        return axisAction;
    }
    
    public static AxisAction Create(string name, InputAction negative, InputAction positive) {
        var axisAction = new AxisAction(name);
        axisAction.SetNegativeAndPositive(negative, positive);
        return axisAction;
    }
}