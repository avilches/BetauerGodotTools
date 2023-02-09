using System;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input;

public class AxisAction : IAction {
    public float Strength => (Positive.Strength - Negative.Strength) * (Reverse ? -1 : 1);
    public float RawStrength => (Positive.RawStrength - Negative.RawStrength) * (Reverse ? -1 : 1);
    public JoyAxis Axis => Positive.Axis;
    public bool Reverse { get; set; } = false; // TODO: load a setting container and allow to export it. Optional: define a axis deadzone
    public string Name { get; }
    public bool IsEvent(InputEvent inputEvent) => inputEvent is InputEventJoypadMotion motion && motion.Axis == Axis;
    public void Enable(bool enabled) {
        Negative.Enable(enabled);
        Positive.Enable(enabled);
    }
    
    public InputActionsContainer? InputActionsContainer { get; private set; }
    public InputAction Negative { get; private set; }
    public InputAction Positive { get; private set; }
    
    // Constructor flags used by the [PostInject] to create to locate the InputActionsContainer
    [Inject] private Container Container { get; set; }
    private readonly string? _inputActionsContainerName;
    private readonly string? _negativeServiceName; 
    private readonly string? _positiveServiceName;


    public AxisAction(InputAction negative, InputAction positive) {
        Fit(negative, positive);
    }

    public AxisAction(string name, InputAction negative, InputAction positive) {
        Name = name;
        Fit(negative, positive);
    }

    private void Fit(InputAction negative, InputAction positive) {
        if (negative.Axis == JoyAxis.Invalid) throw new InvalidAxisConfiguration($"InputAction {negative.Name} should define a valid Axis.");
        if (positive.Axis == JoyAxis.Invalid) throw new InvalidAxisConfiguration($"InputAction {positive.Name} should define a valid Axis.");
        
        if (negative.AxisSign == 0) throw new InvalidAxisConfiguration($"InputAction {negative.Name} should define AxisSign.");
        if (positive.AxisSign == 0) throw new InvalidAxisConfiguration($"InputAction {positive.Name} should define AxisSign.");

        if (positive.AxisSign == negative.AxisSign) {
            var axisSign = $"{(positive.AxisSign > 0 ? "positive " : "negative")} ({positive.AxisSign})";
            throw new InvalidAxisConfiguration($"InputAction {negative.Name} and {positive.Name} can't be both {axisSign}, they must be different.");
        }
        if (positive.AxisSign > 0) {
            Negative = negative;
            Positive = positive;
        } else {
            Negative = positive;
            Positive = negative;
        }
    }

    private AxisAction(string? inputActionsContainerName, string name, string negativeServiceName, string positiveServiceName) {
        _inputActionsContainerName = inputActionsContainerName;
        Name = name;
        _negativeServiceName = negativeServiceName ?? throw new ArgumentNullException(nameof(negativeServiceName));
        _positiveServiceName = positiveServiceName ?? throw new ArgumentNullException(nameof(positiveServiceName));
    }

    [PostInject]
    private void Configure() {
        if (_negativeServiceName != null && _positiveServiceName != null) {
            var negative = Container.Resolve<InputAction>(_negativeServiceName);
            if (negative == null) throw new InvalidAxisConfiguration($"Error creating AxisAction: {_negativeServiceName} InputAction not found");

            var positive =  Container.Resolve<InputAction>(_positiveServiceName);
            if (positive == null) throw new InvalidAxisConfiguration($"Error creating AxisAction: {_positiveServiceName} InputAction not found");
            
            Fit(negative, positive);
            
            var inputActionsContainer = _inputActionsContainerName != null
                ? Container.Resolve<InputActionsContainer>(_inputActionsContainerName)
                : Container.Resolve<InputActionsContainer>();
            inputActionsContainer.Add(this);
        }
    }

    internal void OnAddToInputActionsContainer(InputActionsContainer inputActionsContainer) {
        if (InputActionsContainer != null && InputActionsContainer != inputActionsContainer) {
            InputActionsContainer.Remove(this);
        }
        InputActionsContainer = inputActionsContainer;
        InputActionsContainer.Add(Positive);
        InputActionsContainer.Add(Negative);
    }

    internal void OnRemoveFromInputActionsContainer() {
        if (InputActionsContainer != null) {
            InputActionsContainer.Remove(Positive);
            InputActionsContainer.Remove(Negative);
        }
        InputActionsContainer = null;
    }


    public void SimulatePress(float strength) {
        if (strength == 0f) {
            Negative.SimulatePress(0f);
            Positive.SimulatePress(0f);
        } else {
            if (strength > 0) {
                Negative.SimulatePress(0f);
                Positive.SimulatePress(strength);
            } else {
                Negative.SimulatePress(Mathf.Abs(strength));
                Positive.SimulatePress(0f);
            }
        }
    }

    public void SimulateRelease() => SimulatePress(0f);

    public static AxisAction Fake() {
        var positive = InputAction.Fake().Update(u => u.SetAxis(JoyAxis.LeftX).SetAxisSign(1));
        var negative = InputAction.Fake().Update(u => u.SetAxis(JoyAxis.LeftX).SetAxisSign(-1));
        return new AxisAction(null, negative, positive);
    }

    public static AxisAction Create(string name, string negative, string positive) {
        return new AxisAction(null, name, negative, positive);
    }
    
    public static AxisAction Create(string inputActionsContainerName, string name, string negative, string positive) {
        return new AxisAction(inputActionsContainerName, name, negative, positive);
    }
}

public class InvalidAxisConfiguration : Exception {
    public InvalidAxisConfiguration(string? message) : base(message) {
    }
}