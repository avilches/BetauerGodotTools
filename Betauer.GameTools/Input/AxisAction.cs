using System;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Input;

public class AxisAction {
    public float Strength => Positive.GetStrength() - Negative.GetStrength();

    [Inject] private Container Container { get; set; }

    public InputAction Negative { get; private set; }
    public InputAction Positive { get; private set; }

    private readonly string? _inputActionsContainerName; 
    private readonly string? _negativeActionName; 
    private readonly string? _positiveActionName; 

    public AxisAction(InputAction negative, InputAction positive) {
        Fit(negative, positive);
    }

    private void Fit(InputAction negative, InputAction positive) {
        if (negative.AxisSign == 0) {
            throw new InvalidAxisConfiguration($"InputAction {negative.Name} should define an axis.");
        }
        if (positive.AxisSign == 0) {
            throw new InvalidAxisConfiguration($"InputAction {positive.Name} should define an axis.");
        }
        if (positive.AxisSign == negative.AxisSign) {
            throw new InvalidAxisConfiguration(
                $"InputAction {negative.Name} and {positive.Name} can't be both {(positive.AxisSign > 0 ? "positive" : "negative")}, they must be different.");
        }
        if (positive.AxisSign > 0) {
            Negative = negative;
            Positive = positive;
        } else {
            Negative = positive;
            Positive = negative;
        }
    }

    private AxisAction(string? inputActionsContainerName, string negativeActionName, string positiveActionName) {
        _inputActionsContainerName = inputActionsContainerName;
        _negativeActionName = negativeActionName ?? throw new ArgumentNullException(nameof(negativeActionName));
        _positiveActionName = positiveActionName ?? throw new ArgumentNullException(nameof(positiveActionName));
    }

    [PostInject]
    private void Configure() {
        if (_negativeActionName != null && _positiveActionName != null) {
            var inputActionsContainer = _inputActionsContainerName != null
                ? Container.Resolve<InputActionsContainer>(_inputActionsContainerName)
                : Container.Resolve<InputActionsContainer>();
            
            var negative = inputActionsContainer.FindAction(_negativeActionName);
            if (negative == null) throw new InvalidAxisConfiguration($"Error creating AxisAction: {_negativeActionName} InputAction not found");

            var positive = inputActionsContainer.FindAction(_positiveActionName);
            if (positive == null) throw new InvalidAxisConfiguration($"Error creating AxisAction: {_positiveActionName} InputAction not found");
            
            Fit(negative, positive);
        }
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
        var positive = InputAction.Fake().Update(u => u.SetAxis(JoyAxis.LeftX, 1));
        var negative = InputAction.Fake().Update(u => u.SetAxis(JoyAxis.LeftX, -1));
        return new AxisAction(negative, positive);
    }

    public static AxisAction Create(string inputActionsContainerName, string negative, string positive) {
        return new AxisAction(inputActionsContainerName, negative, positive);
    }
    public static AxisAction Create(string negative, string positive) {
        return new AxisAction(null, negative, positive);
    }
}

public class InvalidAxisConfiguration : Exception {
    public InvalidAxisConfiguration(string? message) : base(message) {
    }
}