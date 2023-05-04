using Godot;

namespace Betauer.Input.Handler;

internal class FrameStateHandler : IHandler {
    internal readonly InputAction InputAction;

    public bool Pressed { get; protected set; } = false;
    public float Strength { get; protected set; }
    public float RawStrength { get; protected set; }
    public float PressedTime { get; protected set; }
    public float ReleasedTime { get; protected set; }
    public ulong ProcessFramePressed { get; protected set; }
    public ulong ProcessFrameReleased { get; protected set; }
    public ulong PhysicsFramePressed { get; protected set; }
    public ulong PhysicsFrameReleased { get; protected set; }

    public bool JustPressed => Pressed && Engine.IsInPhysicsFrame()
                ? Engine.GetPhysicsFrames() == PhysicsFramePressed
                : Engine.GetProcessFrames() == ProcessFramePressed;

    public bool JustReleased => !Pressed && Engine.IsInPhysicsFrame()
        ? Engine.GetPhysicsFrames() == PhysicsFrameReleased
        : Engine.GetProcessFrames() == ProcessFrameReleased;

    public void ClearJustStates() {
        PressedTime = GodotInputHandler.MaxPressedTime; // Pressed one year ago ;)
        ReleasedTime = GodotInputHandler.MaxPressedTime; // Released one year ago ;)
        ProcessFramePressed = 0;
        ProcessFrameReleased = 0;
        PhysicsFramePressed = 0;
        PhysicsFrameReleased = 0;
    }

    internal void ClearState() {
        Pressed = false;
        Strength = 0;
        RawStrength = 0;
        ClearJustStates();
    }

    internal FrameStateHandler(InputAction inputAction) {
        InputAction = inputAction;
        ClearState();
    }

    protected void SetPressed(float strength) {
        if (!Pressed) {
            Pressed = true;
            PressedTime = 0;
            ProcessFramePressed = Engine.GetProcessFrames();
            PhysicsFramePressed = Engine.GetPhysicsFrames();
        }
        Strength = strength;
    }

    protected void SetReleased() {
        Pressed = false;
        Strength = 0;

        ReleasedTime = 0;
        ProcessFrameReleased = Engine.GetProcessFrames();
        PhysicsFrameReleased = Engine.GetPhysicsFrames();
    }

    public void SimulateRelease() {
        SetReleased();
    }

    public void SimulatePress(float strength) {
        if (Pressed) {
            if (strength == 0f) SimulateRelease();
            else SetPressed(strength);
        } else {
            if (strength != 0f) SetPressed(strength);
            // If !Pressed && strength == 0, ignore 
        }
        RawStrength = strength;
    }
}