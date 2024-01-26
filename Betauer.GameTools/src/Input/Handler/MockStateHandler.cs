namespace Betauer.Input.Handler;

internal class MockStateHandler : IHandler {
    internal readonly InputAction InputAction;

    public bool Pressed { get; protected set; } = false;
    public float Strength { get; protected set; }
    public float RawStrength { get; protected set; }
    public float PressedTime => 0;
    public float ReleasedTime => 0;
    public bool JustPressed { get; protected set; }
    public bool JustReleased { get; protected set; }

    internal MockStateHandler(InputAction inputAction) {
        InputAction = inputAction;
        ClearState();
    }

    internal void ClearState() {
        Pressed = false;
        Strength = 0;
        RawStrength = 0;
        ClearJustStates();
    }

    public void ClearJustStates() {
        JustPressed = false;
        JustReleased = false;
    }

    protected void SetPressed(float strength) {
        JustPressed = !Pressed;
        JustReleased = false;
        Pressed = true;
        Strength = strength;
    }

    protected void SetReleased() {
        JustPressed = false;
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

    public void Refresh() { }
}