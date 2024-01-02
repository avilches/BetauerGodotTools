using Betauer.Core.Time;
using Godot;

namespace Betauer.Input.Handler;

internal class GodotInputHandler : IHandler {
    public const float MaxPressedTime = 31536000f; // 1 year! 

    internal readonly InputAction InputAction;
    private StringName? _stringName;
    private readonly GodotStopwatch? _stopwatchPressed;
    private readonly GodotStopwatch? _stopwatchReleased;
    
    public bool HasJustTimers => _stopwatchPressed != null;
    
    // Don't cache the string name in the constructor.
    // Delay the StringName creation until it's actually needed. Reason: the InputAction.Name could be empty and changed later through [InputAction] attribute.
    private StringName ActionStringName => _stringName ??= new StringName(InputAction.Name);

    public GodotInputHandler(InputAction inputAction, bool loadWasPressed) {
        if (loadWasPressed) {
            _stopwatchPressed = new GodotStopwatch().Start();
            _stopwatchReleased = new GodotStopwatch().Start();
        }
        InputAction = inputAction;
    }

    public bool Pressed => Godot.Input.IsActionPressed(ActionStringName);
    public float Strength => Godot.Input.GetActionStrength(ActionStringName);
    public float RawStrength => Godot.Input.GetActionRawStrength(ActionStringName);
    public float PressedTime => _stopwatchPressed != null ? (float)_stopwatchPressed.Elapsed : MaxPressedTime;
    public float ReleasedTime => _stopwatchReleased != null ? (float)_stopwatchReleased.Elapsed : MaxPressedTime;
    public bool JustPressed => Godot.Input.IsActionJustPressed(ActionStringName);
    public bool JustReleased => Godot.Input.IsActionJustReleased(ActionStringName);

    public void SimulatePress(float strength) {
        if (strength >= 0.001f) {
            Godot.Input.ActionPress(ActionStringName, strength);
        } else {
            Godot.Input.ActionRelease(ActionStringName);
        }
    }

    public void SimulateRelease() {
        Godot.Input.ActionRelease(ActionStringName);
    }

    public void ClearJustStates() {
    }

    public void UpdateJustTimers() {
        if (JustPressed) {
            _stopwatchPressed!.Restart();
        } else if (JustReleased) {
            _stopwatchReleased!.Restart();
        }
    }
}