namespace Betauer.Input;

internal class GodotInputHandler : IHandler {
    public const float MaxPressedTime = 31536000f; // 1 year! 

    private readonly string _name;

    public GodotInputHandler(string name) {
        _name = name;
    }

    public bool Pressed => Godot.Input.IsActionPressed(_name);
    public float Strength => Godot.Input.GetActionStrength(_name);
    public float RawStrength => Godot.Input.GetActionRawStrength(_name);
    public float PressedTime => MaxPressedTime;
    public float ReleasedTime => MaxPressedTime;
    public bool JustPressed => Godot.Input.IsActionJustPressed(_name);
    public bool JustReleased => Godot.Input.IsActionJustReleased(_name);
    public void SimulatePress(float strength) {
        if (strength >= 0.001f) 
            Godot.Input.ActionPress(_name, strength);
        else 
            Godot.Input.ActionRelease(_name);
    }
}