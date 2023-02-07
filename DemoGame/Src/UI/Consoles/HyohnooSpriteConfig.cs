using Betauer.DI;
using Godot;

namespace Veronenger.UI.Consoles;

public abstract class HyohnooSpriteConfig : ControllerSpriteConfig {
    protected HyohnooSpriteConfig() {
        Add(JoyButton.A, "A", 13, 14);
        Add(JoyButton.B, "B", 49, 50);
        Add(JoyButton.X, "X", 25, 26);
        Add(JoyButton.Y, "Y", 37, 38);

        Add(JoyButton.LeftShoulder, null, 46, 45); // LB
        Add(JoyButton.RightShoulder, null, 58, 57); // RB
        Add(JoyAxis.TriggerLeft, null, 22, 21); // LT
        Add(JoyAxis.TriggerRight, null, 34, 33); // RT

        // TODO Godot 4
        Add(JoyButton.Back, "", 16, 15);
        Add(JoyButton.Start, "", 19, 20);
        Add(JoyButton.Guide, "", 17, 18); // Xbox Button (big button between select & start)

        Add(JoyButton.DpadRight, "", 28, 27);
        Add(JoyButton.DpadDown, "", 29, 27);
        Add(JoyButton.DpadLeft, "", 30, 27);
        Add(JoyButton.DpadUp, "", 31, 27);

        // Right analog Click
        Add(JoyButton.RightStick, "", 39, 44);

        // Right analog Click
        Add(JoyButton.LeftStick, "left click", 51, 56);


        // Right analog stick:
        // 39:Stop, 40:R, 41:D, 42:L, 43:U
        // [40, 41, 42, 43] Circle
        // [39, 40, 39, 42] Right & left
        // [39, 43, 39, 41] Up & down
			
        // Left analog stick:
        // 51:Stop, 52:R, 53:D, 54:L, 55:U, 56:pressed
        Add(JoyAxis.LeftX, "left lateral", 51, 51); // No pressed, axis!
        // [52, 53, 54, 55] Circle
        // [51, 52, 51, 54] Right & left
        // [51, 55, 51, 53] Up & down

        // Analog stick (without R/L)
        // 63:Stop, 64:R, 65:D, 66:L, 67:U

    }
}