using Godot;

namespace Betauer.Input;

internal class ExtendedInputFrameBasedStateHandler : FrameBasedStateHandler {
    internal ExtendedInputFrameBasedStateHandler(InputAction inputAction) : base(inputAction) {
    }

    public void Update(bool paused, InputEvent inputEvent) {
        if (paused && InputAction.Pausable) return;
        if (inputEvent is InputEventJoypadButton or InputEventKey or InputEventMouseButton) {
            if (inputEvent is InputEventWithModifiers modifiers) {
                if (InputAction.Shift && !modifiers.ShiftPressed) return;
                if (InputAction.Alt && !modifiers.AltPressed) return;
                if (InputAction.CommandOrCtrl) {
                    modifiers.CommandOrControlAutoremap = true;
                    if (!modifiers.IsCommandOrControlPressed()) return;
                } else {
                    if (InputAction.Ctrl && !modifiers.CtrlPressed) return;
                    if (InputAction.Meta && !modifiers.MetaPressed) return;
                }
            }
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