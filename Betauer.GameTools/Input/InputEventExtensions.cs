using System.Linq;
using Godot;

namespace Betauer.Input {
    public static partial class InputEventExtensions {

        public static float GetStrength(this InputEvent input) {
            if (input is InputEventJoypadMotion motion) return motion.AxisValue;
            if (input is InputEventJoypadButton button) return button.Pressure;
            if (input is InputEventMouseButton mouse) return mouse.Pressed ? 1 : 0;
            if (input is InputEventKey key) return key.Pressed ? 1 : 0;
            return 0;
        }
        
        public static bool IsJustPressed(this InputEvent input) => input.IsPressed() && !input.IsEcho();
        public static bool IsReleased(this InputEvent input) => !input.IsPressed() && !input.IsEcho();
        
        // Axis

        public static bool IsAnyAxis(this InputEvent input, int deviceId = -1) {
            return input is InputEventJoypadMotion motion
                   && (deviceId == -1 || motion.Device == deviceId);
        }

        public static bool IsAxis(this InputEvent input, JoystickList axis, int deviceId = -1) {
            return input is InputEventJoypadMotion motion
                   && (deviceId == -1 || motion.Device == deviceId)
                   && (JoystickList)motion.Axis == axis;
        }

        public static JoystickList GetAxis(this InputEvent input) {
            return input is InputEventJoypadMotion k ? (JoystickList)k.Axis : JoystickList.InvalidOption;
        }

        public static float GetAxisValue(this InputEvent input) {
            return input is InputEventJoypadMotion k ? k.AxisValue : 0f;
        }

        // Buttons

        public static bool IsAnyButton(this InputEvent input, int deviceId = -1) {
            return input is InputEventJoypadButton 
                   && (deviceId == -1 || input.Device == deviceId);
        }

        public static bool IsAnyButton(this InputEvent input, params JoystickList[] buttons) {
            return input is InputEventJoypadButton k
                   && buttons.Any(scancode => (JoystickList)k.ButtonIndex == scancode);
        }

        public static bool IsButton(this InputEvent input, JoystickList button, int deviceId = -1) {
            return input is InputEventJoypadButton k
                   && (deviceId == -1 || input.Device == deviceId)
                   && (JoystickList)k.ButtonIndex == button;
        }

        public static JoystickList GetButton(this InputEvent input) {
            return input is InputEventJoypadButton k ? (JoystickList)k.ButtonIndex : JoystickList.InvalidOption;
        }

        public static float GetButtonPressure(this InputEvent input) {
            return input is InputEventJoypadButton k ? k.Pressure : 0f;
        }

        public static string GetButtonString(this InputEvent input) {
            return input is InputEventJoypadButton k ? Godot.Input.GetJoyButtonString(k.ButtonIndex) : "";
        }

        // This include echoes
        public static bool IsButtonPressed(this InputEvent input, JoystickList button, int deviceId = -1) {
            return input is InputEventJoypadButton k
                   && (deviceId == -1 || input.Device == deviceId)
                   && (JoystickList)k.ButtonIndex == button && k.Pressed;
        }

        public static bool IsButtonJustPressed(this InputEvent input, JoystickList button, int deviceId = -1) {
            return input is InputEventJoypadButton k
                   && (deviceId == -1 || input.Device == deviceId)
                   && (JoystickList)k.ButtonIndex == button && input.IsJustPressed();
        }

        public static bool IsButtonReleased(this InputEvent input, JoystickList button, int deviceId = -1) {
            return input is InputEventJoypadButton k
                   && (deviceId == -1 || input.Device == deviceId)
                   && (JoystickList)k.ButtonIndex == button && input.IsReleased();
        }


        
        // Keys
        
        public static bool IsAnyKey(this InputEvent input) {
            return input is InputEventKey;
        }

        public static bool IsAnyKey(this InputEvent input, params int[] scancodes) {
            return input is InputEventKey k && scancodes.Any(scancode => k.Scancode == scancode);
        }

        public static bool IsKey(this InputEvent input, KeyList scancode) {
            return input is InputEventKey k && (KeyList)k.Scancode == scancode;
        }

        public static KeyList GetKey(this InputEvent input) {
            return input is InputEventKey k ? (KeyList)k.Scancode : KeyList.Unknown;
        }

        public static string GetKeyString(this InputEvent input) {
            return input is InputEventKey k ? OS.GetScancodeString(k.Scancode) : "";
        }

        // This include echoes
        public static bool IsKeyPressed(this InputEvent input, KeyList scancode) {
            return input is InputEventKey k && (KeyList)k.Scancode == scancode && k.Pressed;
        }

        public static bool IsKeyJustPressed(this InputEvent input, KeyList scancode) {
            return input is InputEventKey k && (KeyList)k.Scancode == scancode && input.IsJustPressed();
        }

        public static bool IsKeyReleased(this InputEvent input, KeyList scancode) {
            return input is InputEventKey k && (KeyList)k.Scancode == scancode && input.IsReleased();
        }



        // Mouse
        
        public static bool IsAnyClick(this InputEvent input) {
            return input is InputEventMouseButton;
        }

        public static bool IsAnyClick(this InputEvent input, params ButtonList[] buttons) {
            return input is InputEventMouseButton k && buttons.Any(button => (ButtonList)k.ButtonIndex == button);
        }

        public static bool IsClick(this InputEvent input, ButtonList button) {
            return input is InputEventMouseButton k && (ButtonList)k.ButtonIndex == button;
        }

        public static ButtonList GetClick(this InputEvent input) {
            return input is InputEventMouseButton k ? (ButtonList)k.ButtonIndex : ButtonList.MaskXbutton2;
        }

        // This include echoes
        public static bool IsClickPressed(this InputEvent input, ButtonList button) {
            return input is InputEventMouseButton k && (ButtonList)k.ButtonIndex == button && k.Pressed;
        }

        public static bool IsClickJustPressed(this InputEvent input, ButtonList button) {
            return input is InputEventMouseButton k && (ButtonList)k.ButtonIndex == button && input.IsJustPressed();
        }

        public static bool IsClickReleased(this InputEvent input, ButtonList button) {
            return input is InputEventMouseButton k && (ButtonList)k.ButtonIndex == button && input.IsReleased();
        }
    }
}