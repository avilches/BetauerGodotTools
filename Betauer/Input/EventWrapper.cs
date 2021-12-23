using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Input {
    public class EventWrapper {
        public InputEvent Event;

        public EventWrapper(InputEvent @event) {
            Event = @event;
        }

        public int Device => Event.Device;
        public int Button => Event is InputEventJoypadButton button ? button.ButtonIndex : -1;
        public bool Pressed => Event.IsPressed();
        public float Pressure => Event is InputEventJoypadButton button ? button.Pressure : -1;

        public int Key => Event is InputEventKey key ? (int) key.Scancode : -1;

        public string KeyString => OS.GetScancodeString((uint) Key);

        public float Axis => Event is InputEventJoypadMotion joypadMotion ? joypadMotion.Axis : -1;
        public float AxisValue => Event is InputEventJoypadMotion joypadMotion ? joypadMotion.AxisValue : 0;
        public bool Echo => Event is InputEventKey key && key.Echo;

        public bool IsMotion() {
            return Event is InputEventJoypadMotion;
        }

        public bool IsDevice(int deviceId) {
            return Event.Device == deviceId;
        }

        public bool IsAxis(int axis, int deviceId = -1) {
            if (Event is InputEventJoypadMotion motion) {
                if (deviceId == -1 || motion.Device == deviceId) {
                    if (motion.Axis == axis) {
                        return true;
                    }
                }
            }

            return false;
        }

        public float GetStrength(float deadZone = 0.5f) {
            if (Event is InputEventJoypadMotion motion) {
                // TODO: Normalize with deadzone
                if (Mathf.Abs(motion.AxisValue) > deadZone) {
                    return motion.AxisValue;
                }

                return 0;
            } else if (Event is InputEventJoypadButton button) {
                return button.Pressed ? 1f : 0f;
            } else if (Event is InputEventKey key) {
                return key.Pressed ? 1f : 0f;
            }

            throw new Exception("Strength not supported for " + Event.GetType().Name + ":" + Event.AsText());
        }

        public bool IsAnyButton(int deviceId = -1) {
            return (deviceId == -1 || Event.Device == deviceId) && Event is InputEventJoypadButton;
        }

        public bool IsButton(int button, int deviceId = -1) {
            return (deviceId == -1 || Event.Device == deviceId) && Event is InputEventJoypadButton b &&
                   b.ButtonIndex == button;
        }

        public bool IsButton(ISet<int> buttons, int deviceId = -1) {
            return (deviceId == -1 || Event.Device == deviceId) && Event is InputEventJoypadButton b &&
                   buttons.Contains(b.ButtonIndex);
        }

        public bool IsButtonPressed(JoystickList button, int deviceId = -1) {
            return IsButtonPressed((int) button, deviceId);
        }

        public bool IsButtonReleased(JoystickList button, int deviceId = -1) {
            return IsButtonReleased((int) button, deviceId);
        }

        public bool IsButtonPressed(int button, int deviceId = -1) {
            if ((deviceId == -1 || Event.Device == deviceId) && Event is InputEventJoypadButton b &&
                b.ButtonIndex == button) {
                return b.Pressed;
            }

            return false;
        }

        public bool IsButtonReleased(int button, int deviceId = -1) {
            if ((deviceId == -1 || Event.Device == deviceId) && Event is InputEventJoypadButton b &&
                b.ButtonIndex == button) {
                return !b.Pressed;
            }

            return false;
        }

        public bool IsAnyKey() {
            return Event is InputEventKey;
        }

        public bool IsKey(KeyList k) {
            return IsKey((int) k);
        }

        public bool IsKey(int scancode) {
            return Event is InputEventKey k && k.Scancode == scancode;
        }

        public bool IsKey(ISet<int> scancodes) {
            return Event is InputEventKey k && scancodes.Contains((int) k.Scancode);
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
            return Event is InputEventKey k && k.Scancode == scancode && k.Pressed;
        }

        public bool IsKeyPressed(int scancode, bool echo) {
            return Event is InputEventKey k && k.Scancode == scancode && k.Pressed && k.Echo == echo;
        }

        public bool IsKeyReleased(int scancode) {
            return Event is InputEventKey k && k.Scancode == scancode && !k.Pressed;
        }

        public bool IsKeyReleased(int scancode, bool echo) {
            return Event is InputEventKey k && k.Scancode == scancode && !k.Pressed && k.Echo == echo;
        }
    }
}