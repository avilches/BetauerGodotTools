using Godot;

namespace Betauer.Input.Handler;

internal class ExtendedInputHandler : FrameStateHandler {
    internal ExtendedInputHandler(InputAction inputAction) : base(inputAction) {
    }

    public void Update(bool paused, InputEvent inputEvent) {
        if (paused && InputAction.Pausable) return;
        if (!InputAction.IsEvent(inputEvent)) return;
        
        if (inputEvent is InputEventJoypadButton or InputEventKey or InputEventMouseButton) {
            var strength = inputEvent.GetStrength();
            if (inputEvent.IsJustPressed()) {
                SetPressed(strength);
            } else if (inputEvent.IsReleased()) {
                SetReleased();
            } else {
                // Ignore keyboard echoes
            }
            RawStrength = strength;
        } else if (inputEvent is InputEventJoypadMotion motion) {
            var sameSign = InputAction.AxisSign == 1 ? motion.AxisValue >= 0f : motion.AxisValue <= 0f;
            var value = Mathf.Abs(motion.AxisValue);
            if (sameSign) {
                if (value >= InputAction.DeadZone) SetPressed(value);
                else if (Pressed) SetReleased();
                RawStrength = value;
            } else {
                if (Pressed) SetReleased();
                RawStrength = 0;
            }
        }
    }

    public void AddTime(bool paused, float delta) {
        if (paused && InputAction.Pausable) return;
        PressedTime += delta;
        ReleasedTime += delta;
    }
}