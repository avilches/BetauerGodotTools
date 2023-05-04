using Godot;

namespace Betauer.Input.Handler;

internal class GodotInputHandler : IHandler {
    public const float MaxPressedTime = 31536000f; // 1 year! 

    internal readonly InputAction InputAction;
    private StringName? _stringName;
    
    // Don't cache the string name in the constructor.
    // Delay the StringName creation until it's actually needed. Reason: the InputAction.Name could be empty and changed later through [InputAction] attribute.
    private StringName StringName => _stringName ??= new StringName(InputAction.Name);

    public GodotInputHandler(InputAction inputAction) {
        InputAction = inputAction;
    }

    public bool Pressed => Godot.Input.IsActionPressed(StringName);
    public float Strength => Godot.Input.GetActionStrength(StringName);
    public float RawStrength => Godot.Input.GetActionRawStrength(StringName);
    public float PressedTime => MaxPressedTime;
    public float ReleasedTime => MaxPressedTime;
    public bool JustPressed => Godot.Input.IsActionJustPressed(StringName);
    public bool JustReleased => Godot.Input.IsActionJustReleased(StringName);

    public void SimulatePress(float strength) {
        if (strength >= 0.001f)
            Godot.Input.ActionPress(StringName, strength);
        else
            Godot.Input.ActionRelease(StringName);
    }

    public void SimulateRelease() {
        Godot.Input.ActionRelease(StringName);
    }

    public void ClearJustStates() {
    }
}