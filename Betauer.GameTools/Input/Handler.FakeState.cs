namespace Betauer.Input;

internal class FakeStateHandler : IHandler {
    internal readonly InputAction InputAction;

    public bool Pressed { get; protected set; } = false;
    public float Strength { get; protected set; }
    public float RawStrength { get; protected set; }
    public float PressedTime => GodotInputHandler.MaxPressedTime; // Pressed one year ago ;)
    public float ReleasedTime => GodotInputHandler.MaxPressedTime; // Pressed one year ago ;)
    public bool JustPressed { get; protected set; }
    public bool JustReleased { get; protected set; }

    public void ClearJustStates() {
        JustPressed = false;
        JustReleased = false;
    }

    internal void ClearState() {
        Pressed = false;
        Strength = 0;
        RawStrength = 0;
        ClearJustStates();
    }

    internal FakeStateHandler(InputAction inputAction) {
        InputAction = inputAction;
        ClearState();
    }

    protected void SetPressed(float strength) {
        JustPressed = !Pressed;
        Pressed = true;
        Strength = strength;
    }

    protected void SetReleased() {
        JustReleased = Pressed;
        Pressed = false;
        Strength = 0;
    }

    public void SimulateRelease() {
        SetReleased();
    }

    public void SimulatePress(float strength) {
        if (Pressed) {
            if (strength == 0f) SetReleased();
            else SetPressed(strength);
        } else {
            if (strength != 0f) SetPressed(strength);
            // If !Pressed && strength == 0, ignore 
        }
        RawStrength = strength;
    }
}