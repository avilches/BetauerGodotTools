using Betauer.Nodes;
using Godot;

namespace Betauer.Input;

internal class InputActionStateHandler : BaseEventHandler, IProcessHandler, IInputEventHandler {
    public const float MaxPressedTime = 31536000f; 
    private readonly InputAction _inputAction;

    internal bool Pressed = false;
    internal float Strength { get; private set; }
    internal float PressedTime { get; private set; }
    internal float ReleasedTime { get; private set; }
    internal ulong ProcessFramePressed { get; private set; }
    internal ulong ProcessFrameReleased { get; private set; }
    internal ulong PhysicsFramePressed { get; private set; }
    internal ulong PhysicsFrameReleased { get; private set; }
    internal bool CtrlPressed { get; private set; } = false;
    internal bool ShiftPressed { get; private set; } = false;
    internal bool AltPressed { get; private set; } = false;
    internal bool MetaPressed { get; private set; } = false;
    internal bool CommandOrCtrlPressed { get; private set; } = false;

    internal bool JustPressed => Pressed && Engine.IsInPhysicsFrame()
        ? Engine.GetPhysicsFrames() == PhysicsFramePressed
        : Engine.GetProcessFrames() == ProcessFramePressed;

    internal bool JustReleased => !Pressed && Engine.IsInPhysicsFrame()
        ? Engine.GetPhysicsFrames() == PhysicsFrameReleased
        : Engine.GetProcessFrames() == ProcessFrameReleased;

    internal void ClearState() {
        Pressed = false;
        Strength = 0;
        PressedTime  = MaxPressedTime; // Pressed one year ago ;)
        ReleasedTime = MaxPressedTime; // Released one year ago ;)
        ProcessFramePressed = 0;        
        ProcessFrameReleased = 0;        
        PhysicsFramePressed = 0;        
        PhysicsFrameReleased = 0;        
        CtrlPressed = false;
        ShiftPressed = false;
        AltPressed = false;
        MetaPressed = false;
        CommandOrCtrlPressed = false;
    }

    internal InputActionStateHandler(InputAction inputAction, Node.ProcessModeEnum processMode) : 
        base(inputAction.Name, processMode) {
        _inputAction = inputAction;
        ClearState();
    }

    public void Handle(InputEvent inputEvent) {
        if (!_inputAction.Matches(inputEvent)) return;
        if (inputEvent is InputEventJoypadButton or InputEventKey or InputEventMouseButton) {
            if (inputEvent is InputEventWithModifiers modifiers) {
                if (_inputAction.Shift && !modifiers.ShiftPressed) return;
                ShiftPressed = modifiers.ShiftPressed;

                if (_inputAction.Alt && !modifiers.AltPressed) return;
                AltPressed = modifiers.AltPressed;

                if (_inputAction.CommandOrCtrl) {
                    modifiers.CommandOrControlAutoremap = true;
                    if (!modifiers.IsCommandOrControlPressed()) return;
                    CommandOrCtrlPressed = true;
                } else {
                    if (_inputAction.Ctrl && !modifiers.CtrlPressed) return;
                    CtrlPressed = modifiers.CtrlPressed;

                    if (_inputAction.Meta && !modifiers.MetaPressed) return;
                    MetaPressed = modifiers.MetaPressed;
                }
            }

            var strength = inputEvent.GetStrength();
            if (inputEvent.IsJustPressed()) {
                SetJustPressed(strength);
            } else if (inputEvent.IsPressed()) {
                SetPressed(strength);
            } else if (inputEvent.IsReleased()) {
                SetReleased();
            } else {
                // echo
            }
        } else if (inputEvent is InputEventJoypadMotion axis) {
            var value = Mathf.Abs(axis.AxisValue);
            if (value > _inputAction.DeadZone) {
                if (Mathf.Sign(axis.AxisValue) == _inputAction.AxisSign) {
                    if (Pressed) {
                        SetPressed(value);
                    } else {
                        SetJustPressed(value);
                    }
                }
            } else {
                SetReleased();
            }
        }
    }

    public void Handle(double delta) {
        PressedTime += (float)delta;
        ReleasedTime += (float)delta;
    }

    private void SetPressed(float strength) {
        Pressed = true;
        Strength = strength;
    }

    private void SetJustPressed(float strength) {
        Pressed = true;
        Strength = strength;

        PressedTime = 0;
        ProcessFramePressed = Engine.GetProcessFrames();
        PhysicsFramePressed = Engine.GetPhysicsFrames();
    }

    private void SetReleased() {
        Pressed = false;
        Strength = 0;

        ReleasedTime = 0;
        ProcessFrameReleased = Engine.GetProcessFrames();
        PhysicsFrameReleased = Engine.GetPhysicsFrames();
    }

    public void SimulateCtrl(bool pressed) => CtrlPressed = pressed;

    public void SimulateShift(bool pressed) => ShiftPressed = pressed;
    
    public void SimulateAlt(bool pressed) => AltPressed = pressed;
    
    public void SimulateMeta(bool pressed) => MetaPressed = pressed;
    
    public void SimulateCommandOrCtrl(bool pressed) => CommandOrCtrlPressed = pressed;

    public void SimulatePress(float strength) {
        if (Pressed) {
            if (strength == 0f) SetReleased();
            else SetPressed(strength);
        } else {
            if (strength != 0f) SetJustPressed(strength);
            // If !Pressed && strength == 0, ignore 
        }
    }
}