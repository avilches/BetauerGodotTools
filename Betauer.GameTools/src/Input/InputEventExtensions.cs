using System.Linq;
using Godot;

namespace Betauer.Input;
public static partial class InputEventExtensions {
    
    // For all types
    public static float GetStrength(this InputEvent input) {
        if (input is InputEventJoypadMotion motion) return motion.AxisValue;
        if (input is InputEventJoypadButton button) return button.Pressure;
        if (input is InputEventMouseButton mouse) return mouse.Pressed ? 1 : 0;
        if (input is InputEventKey key) return key.Pressed ? 1 : 0;
        return 0;
    }

    /// <summary>
    /// For key events, this method ensures echoes are ignored. No echo means it is the first press (just pressed!).
    /// Joypads don't send echoes, so it works like IsPressed()
    /// For axis actions, this will return tru only if strength is 1, which is different from listening in InputAction
    /// or Godot.Input, where the action.IsJustPressed returns null the first event > deadzone 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsJustPressed(this InputEvent input) => input.IsPressed() && (input is not InputEventKey key || !key.IsEcho());
    
    /// <summary>
    /// For key events, this method ignores echoes. Joypads don't send echoes, so it works like !IsPressed() 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsReleased(this InputEvent input) => !input.IsPressed() && !input.IsEcho();

    public static bool HasShift(this InputEvent input) =>
        input is InputEventWithModifiers { ShiftPressed: true };

    public static bool HasControl(this InputEvent input) =>
        input is InputEventWithModifiers { CtrlPressed: true };

    public static bool HasAlt(this InputEvent input) =>
        input is InputEventWithModifiers { AltPressed: true };

    public static bool HasMeta(this InputEvent input) =>
        input is InputEventWithModifiers { MetaPressed: true };

    // Joypad
    public static bool IsJoypad(this InputEvent input) =>
        input is InputEventJoypadButton or InputEventJoypadMotion;

    public static string? GetJoypadName(this InputEvent input) =>
        input is InputEventJoypadButton or InputEventJoypadMotion ? Godot.Input.GetJoyName(input.Device) : null;

    public static string? GetJoypadGuid(this InputEvent input) =>
        input is InputEventJoypadButton or InputEventJoypadMotion ? Godot.Input.GetJoyGuid(input.Device) : null;

    
    // Joypad: Motion and Axis
    public static bool IsAnyAxis(this InputEvent input, int deviceId = -1) =>
        input is InputEventJoypadMotion motion
        && (deviceId == -1 || motion.Device == deviceId);

    public static bool IsAxis(this InputEvent input, JoyAxis axis, int deviceId = -1) =>
        input is InputEventJoypadMotion motion
        && (deviceId == -1 || motion.Device == deviceId)
        && motion.Axis == axis;

    public static JoyAxis GetAxis(this InputEvent input) =>
        input is InputEventJoypadMotion k ? k.Axis : JoyAxis.Invalid;

    public static float GetAxisValue(this InputEvent input) =>
        input is InputEventJoypadMotion k ? k.AxisValue : 0f;

    
    // Joypad: buttons
    public static bool IsAnyButton(this InputEvent input, int deviceId = -1) =>
        input is InputEventJoypadButton
        && (deviceId == -1 || input.Device == deviceId);

    public static bool IsButton(this InputEvent input, JoyButton button, int deviceId = -1) =>
        input is InputEventJoypadButton k
        && (deviceId == -1 || input.Device == deviceId)
        && k.ButtonIndex == button;

    public static JoyButton GetButton(this InputEvent input) =>
        input is InputEventJoypadButton k ? k.ButtonIndex : JoyButton.Invalid;

    public static float GetButtonPressure(this InputEvent input) =>
        input is InputEventJoypadButton k ? k.Pressure : 0f;

    // This include echoes
    public static bool IsButtonPressed(this InputEvent input, JoyButton button, int deviceId = -1) =>
        input is InputEventJoypadButton k
        && (deviceId == -1 || input.Device == deviceId)
        && k.ButtonIndex == button && k.Pressed;

    public static bool IsButtonReleased(this InputEvent input, JoyButton button, int deviceId = -1) =>
        input is InputEventJoypadButton k
        && (deviceId == -1 || input.Device == deviceId)
        && k.ButtonIndex == button && !k.Pressed;

    // Keys
    public static bool IsAnyKey(this InputEvent input) =>
        input is InputEventKey;

    public static bool IsKey(this InputEvent input, Key key) =>
        input is InputEventKey k && k.Keycode == key;

    public static Key GetKey(this InputEvent input) =>
        input is InputEventKey k ? k.Keycode : Key.None;

    public static string GetKeyStringWithModifiers(this InputEvent input) =>
        input is InputEventKey k ? OS.GetKeycodeString(k.GetKeycodeWithModifiers()) : "";

    public static string GetKeyString(this InputEvent input) =>
        input is InputEventKey k ? OS.GetKeycodeString(k.Keycode) : "";

    public static string GetKeyUnicode(this InputEvent input) =>
        input is InputEventKey k ? char.ToString((char)k.Unicode) : "";
    
    // This include echoes
    public static bool IsKeyPressed(this InputEvent input, Key key) =>
        input is InputEventKey k && k.Keycode == key && k.Pressed;

    public static bool IsKeyJustPressed(this InputEvent input, Key key) =>
        input is InputEventKey k && k.Keycode == key && input.IsJustPressed();

    public static bool IsKeyReleased(this InputEvent input, Key key) =>
        input is InputEventKey k && k.Keycode == key && input.IsReleased();

    /*
     * Mouse
     */
    public static bool IsMouse(this InputEvent input) =>
        input is InputEventMouse;

    public static bool IsMouseMotion(this InputEvent input) =>
        input is InputEventMouseMotion;

    public static Vector2 GetMousePosition(this InputEvent input) =>
        input is InputEventMouse m ? m.Position : Vector2.Zero;

    public static Vector2 GetMouseGlobalPosition(this InputEvent input) =>
        input is InputEventMouse m ? m.GlobalPosition : Vector2.Zero;

    /*
     * Any click
     */
    public static MouseButton GetClick(this InputEvent input) =>
        input is InputEventMouseButton k ? k.ButtonIndex : MouseButton.None;

    public static bool IsAnyClick(this InputEvent input) =>
        input is InputEventMouseButton;

    public static bool IsClick(this InputEvent input, MouseButton button) =>
        input is InputEventMouseButton k &&
        k.ButtonIndex == button;

    public static bool IsClickPressed(this InputEvent input, MouseButton button) =>
        input is InputEventMouseButton k && k.ButtonIndex == button && k.Pressed;

    public static bool IsClickReleased(this InputEvent input, MouseButton button) =>
        input is InputEventMouseButton k && k.ButtonIndex == button && !k.Pressed;

    public static bool IsDoubleClick(this InputEvent input) =>
        input is InputEventMouseButton { DoubleClick: true };

    public static bool IsDoubleClick(this InputEvent input, MouseButton button) =>
        input.IsClick(button) && input.IsDoubleClick();

    public static bool IsMouseInside(this InputEvent input, Control control) => 
        control.GetGlobalRect().HasPoint(input.GetMousePosition());
    
    /*
     * Left click
     */
    public static bool IsLeftClick(this InputEvent input) => 
        input.IsClick(MouseButton.Left);

    public static bool IsLeftClickPressed(this InputEvent input) =>
        input.IsClickPressed(MouseButton.Left);

    public static bool IsLeftClickReleased(this InputEvent input) =>
        input.IsClickReleased(MouseButton.Left);

    public static bool IsLeftDoubleClick(this InputEvent input) =>
        input.IsDoubleClick(MouseButton.Left);

    /*
     * Middle click
     */
    public static bool IsMiddleClick(this InputEvent input) => 
        input.IsClick(MouseButton.Middle);

    public static bool IsMiddleClickPressed(this InputEvent input) =>
        input.IsClickPressed(MouseButton.Middle);

    public static bool IsMiddleClickReleased(this InputEvent input) =>
        input.IsClickReleased(MouseButton.Middle);

    public static bool IsMiddleDoubleClick(this InputEvent input) =>
        input.IsDoubleClick(MouseButton.Middle);

    /*
     * Right click
     */
    public static bool IsRightClick(this InputEvent input) => 
        input.IsClick(MouseButton.Right);

    public static bool IsRightClickPressed(this InputEvent input) =>
        input.IsClickPressed(MouseButton.Right);

    public static bool IsRightClickReleased(this InputEvent input) =>
        input.IsClickReleased(MouseButton.Right);

    public static bool IsRightDoubleClick(this InputEvent input) =>
        input.IsDoubleClick(MouseButton.Right);
    
}