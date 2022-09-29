using System.Linq;
using Godot;

namespace Betauer.Input {
    public static partial class InputEventExtensions {
        
        // For all types
        public static float GetStrength(this InputEvent input) {
            if (input is InputEventJoypadMotion motion) return motion.AxisValue;
            if (input is InputEventJoypadButton button) return button.Pressure;
            if (input is InputEventMouseButton mouse) return mouse.Pressed ? 1 : 0;
            if (input is InputEventKey key) return key.Pressed ? 1 : 0;
            return 0;
        }

        public static bool IsJustPressed(this InputEvent input) => input.IsPressed() && !input.IsEcho();
        
        public static bool IsReleased(this InputEvent input) => !input.IsPressed() && !input.IsEcho();

        public static bool HasShift(this InputEvent input) =>
            input is InputEventWithModifiers { Shift: true };

        public static bool HasControl(this InputEvent input) =>
            input is InputEventWithModifiers { Control: true };

        public static bool HasAlt(this InputEvent input) =>
            input is InputEventWithModifiers { Alt: true };

        public static bool HasCommand(this InputEvent input) =>
            input is InputEventWithModifiers { Command: true };

        public static bool HasMeta(this InputEvent input) =>
            input is InputEventWithModifiers { Meta: true };

        
        // Joystick axis
        public static bool IsAnyAxis(this InputEvent input, int deviceId = -1) =>
            input is InputEventJoypadMotion motion
            && (deviceId == -1 || motion.Device == deviceId);

        public static bool IsAxis(this InputEvent input, JoystickList axis, int deviceId = -1) =>
            input is InputEventJoypadMotion motion
            && (deviceId == -1 || motion.Device == deviceId)
            && (JoystickList)motion.Axis == axis;

        public static JoystickList GetAxis(this InputEvent input) =>
            input is InputEventJoypadMotion k ? (JoystickList)k.Axis : JoystickList.InvalidOption;

        public static float GetAxisValue(this InputEvent input) =>
            input is InputEventJoypadMotion k ? k.AxisValue : 0f;

        
        // Joystick buttons
        public static bool IsAnyButton(this InputEvent input, int deviceId = -1) =>
            input is InputEventJoypadButton
            && (deviceId == -1 || input.Device == deviceId);

        public static bool IsButton(this InputEvent input, JoystickList button, int deviceId = -1) =>
            input is InputEventJoypadButton k
            && (deviceId == -1 || input.Device == deviceId)
            && (JoystickList)k.ButtonIndex == button;

        public static JoystickList GetButton(this InputEvent input) =>
            input is InputEventJoypadButton k ? (JoystickList)k.ButtonIndex : JoystickList.InvalidOption;

        public static float GetButtonPressure(this InputEvent input) =>
            input is InputEventJoypadButton k ? k.Pressure : 0f;

        public static string GetButtonString(this InputEvent input) =>
            input is InputEventJoypadButton k ? Godot.Input.GetJoyButtonString(k.ButtonIndex) : "";

        // This include echoes
        public static bool IsButtonPressed(this InputEvent input, JoystickList button, int deviceId = -1) =>
            input is InputEventJoypadButton k
            && (deviceId == -1 || input.Device == deviceId)
            && (JoystickList)k.ButtonIndex == button && k.Pressed;

        public static bool IsButtonJustPressed(this InputEvent input, JoystickList button, int deviceId = -1) =>
            input is InputEventJoypadButton k
            && (deviceId == -1 || input.Device == deviceId)
            && (JoystickList)k.ButtonIndex == button && input.IsJustPressed();

        public static bool IsButtonReleased(this InputEvent input, JoystickList button, int deviceId = -1) =>
            input is InputEventJoypadButton k
            && (deviceId == -1 || input.Device == deviceId)
            && (JoystickList)k.ButtonIndex == button && input.IsReleased();

        
        // Keys
        public static bool IsAnyKey(this InputEvent input) =>
            input is InputEventKey;

        public static bool IsKey(this InputEvent input, KeyList scancode) =>
            input is InputEventKey k && (KeyList)k.Scancode == scancode;

        public static KeyList GetKey(this InputEvent input) =>
            input is InputEventKey k ? (KeyList)k.Scancode : KeyList.Unknown;

        public static string GetKeyString(this InputEvent input) =>
            input is InputEventKey k ? OS.GetScancodeString(k.Scancode) : "";

        public static string GetKeyUnicode(this InputEvent input) =>
            input is InputEventKey k ? char.ToString((char)k.Unicode) : "";
        
        // This include echoes
        public static bool IsKeyPressed(this InputEvent input, KeyList scancode) =>
            input is InputEventKey k && (KeyList)k.Scancode == scancode && k.Pressed;

        public static bool IsKeyJustPressed(this InputEvent input, KeyList scancode) =>
            input is InputEventKey k && (KeyList)k.Scancode == scancode && input.IsJustPressed();

        public static bool IsKeyReleased(this InputEvent input, KeyList scancode) =>
            input is InputEventKey k && (KeyList)k.Scancode == scancode && input.IsReleased();

        
        // Mouse
        public static bool IsMouseMotion(this InputEvent input) =>
            input is InputEventMouseMotion;

        public static bool IsClick(this InputEvent input) =>
            input is InputEventMouseButton;

        public static bool IsClick(this InputEvent input, ButtonList button) =>
            input is InputEventMouseButton k &&
            (ButtonList)k.ButtonIndex == button;

        public static Vector2 GetMousePosition(this InputEvent input) =>
            input is InputEventMouse m ? m.Position : Vector2.Zero;

        public static Vector2 GetMouseGlobalPosition(this InputEvent input) =>
            input is InputEventMouse m ? m.GlobalPosition : Vector2.Zero;

        public static bool IsDoubleClick(this InputEvent input) =>
            input is InputEventMouseButton { Doubleclick: true };

        public static bool IsDoubleClick(this InputEvent input, ButtonList button) =>
            input.IsClick(button) && input.IsDoubleClick();

        public static bool IsLeftClick(this InputEvent input) => 
            input.IsClick(ButtonList.Left);

        public static bool IsLeftDoubleClick(this InputEvent input) => 
            input.IsClick(ButtonList.Left) && input.IsDoubleClick();

        public static bool IsMiddleClick(this InputEvent input) => 
            input.IsClick(ButtonList.Middle);

        public static bool IsMiddleDoubleClick(this InputEvent input) => 
            input.IsClick(ButtonList.Middle) && input.IsDoubleClick();

        public static bool IsRightClick(this InputEvent input) => 
            input.IsClick(ButtonList.Right);

        public static bool IsRightDoubleClick(this InputEvent input) => 
            input.IsClick(ButtonList.Right) && input.IsDoubleClick();

        public static ButtonList GetClick(this InputEvent input) =>
            input is InputEventMouseButton k ? (ButtonList)k.ButtonIndex : ButtonList.MaskXbutton2;

        // This include echoes
        // TODO: it needs the Left and Right version of Pressed, JustPressed and Released
        public static bool IsClickPressed(this InputEvent input, ButtonList button) =>
            input is InputEventMouseButton k && (ButtonList)k.ButtonIndex == button && k.Pressed;

        public static bool IsClickJustPressed(this InputEvent input, ButtonList button) =>
            input is InputEventMouseButton k && (ButtonList)k.ButtonIndex == button && input.IsJustPressed();

        public static bool IsClickReleased(this InputEvent input, ButtonList button) =>
            input is InputEventMouseButton k && (ButtonList)k.ButtonIndex == button && input.IsReleased();

        
    }
}