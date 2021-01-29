using System;
using System.Collections.Generic;
using Godot;

public class EventWrapper {
    public InputEvent @event;

    public EventWrapper(InputEvent @event) {
        this.@event = @event;
    }

    public bool IsAxis(int axis, int deviceId = -1) {
        if (@event is InputEventJoypadMotion motion) {
            if (deviceId == -1 || motion.Device == deviceId) {
                if (motion.Axis == axis) {
                    return true;
                }
            }
        }
        return false;
    }

    public float GetAxisValue() {
        return ((InputEventJoypadMotion) @event).AxisValue;
    }

    public float GetStrength(float deadZone = 0.5f) {
        if (@event is InputEventJoypadMotion motion) {
            // TODO: Normalize with deadzone
            GD.Print(motion.AxisValue+ " "+(Mathf.Abs(motion.AxisValue) > deadZone ? "SI":"Ignored"));
            if (Mathf.Abs(motion.AxisValue) > deadZone) {
                return motion.AxisValue;
            }

            return 0;
        } else if (@event is InputEventJoypadButton button) {
            return button.Pressure;
        } else if (@event is InputEventKey key) {
            return key.Pressed ? 1f : 0f;
        }

        throw new Exception("Strength not supported for " + @event.GetType().Name + ":" + @event.AsText());
    }

    public bool IsButton(int button, int deviceId = -1) {
        return (deviceId == -1 || @event.Device == deviceId) && @event is InputEventJoypadButton b && b.ButtonIndex == button;
    }

    public bool IsButton(ISet<int> buttons, int deviceId = -1) {
        return (deviceId == -1 || @event.Device == deviceId) && @event is InputEventJoypadButton b && buttons.Contains(b.ButtonIndex);
    }

    public bool IsPressed() {
        return @event.IsPressed();
    }

    public bool IsButtonPressed(JoystickList button, int deviceId = -1) {
        return IsButtonPressed((int) button, deviceId);
    }

    public bool IsButtonReleased(JoystickList button, int deviceId = -1) {
        return IsButtonReleased((int) button, deviceId);
    }

    public bool IsButtonPressed(int button, int deviceId = -1) {
        if ((deviceId == -1 || @event.Device == deviceId) && @event is InputEventJoypadButton b && b.ButtonIndex == button) {
            return b.Pressed;
        }
        return false;
    }

    public bool IsButtonReleased(int button, int deviceId = -1) {
        if ((deviceId == -1 || @event.Device == deviceId) && @event is InputEventJoypadButton b && b.ButtonIndex == button) {
            return !b.Pressed;
        }
        return false;
    }

    public bool IsKey(KeyList k) {
        return IsKey((int) k);
    }

    public bool IsKey(int scancode) {
        return @event is InputEventKey k && k.Scancode == scancode;
    }

    public bool IsKey(ISet<int> scancodes) {
        return @event is InputEventKey k && scancodes.Contains((int)k.Scancode);
    }

    public bool IsKeyPressed(KeyList k) {
        return IsKeyPressed((int) k);
    }

    public bool IsKeyPressed(KeyList k, bool echo) {
        return IsKeyPressed((int) k, echo);
    }

    public bool IsKeyReleased(KeyList k) {
        return IsKeyReleased((int) k);
    }

    public bool IsKeyReleased(KeyList k, bool echo) {
        return IsKeyReleased((int) k, echo);
    }

    public bool IsKeyPressed(int scancode) {
        return @event is InputEventKey k && k.Scancode == scancode && k.Pressed;
    }

    public bool IsKeyPressed(int scancode, bool echo) {
        return @event is InputEventKey k && k.Scancode == scancode && k.Pressed && k.Echo == echo;
    }

    public bool IsKeyReleased(int scancode) {
        return @event is InputEventKey k && k.Scancode == scancode && !k.Pressed;
    }

    public bool IsKeyReleased(int scancode, bool echo) {
        return @event is InputEventKey k && k.Scancode == scancode && !k.Pressed && k.Echo == echo;
    }
}