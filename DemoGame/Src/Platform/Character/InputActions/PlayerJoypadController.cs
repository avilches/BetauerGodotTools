using Betauer.Input;
using Betauer.Input.Joypad;

namespace Veronenger.Platform.Character.InputActions;

public class PlayerJoypadController : JoypadController, ICharacterHandler {
    public AxisAction Lateral { get; private set; }
    public AxisAction Vertical { get; private set; }
    public InputAction Jump { get; private set; }
    public InputAction Attack { get; private set; }
    public InputAction Float { get; private set; }
    public InputAction Drop { get; private set; }
    public InputAction NextItem { get; private set; }
    public InputAction PrevItem { get; private set; }
    public InputAction Left { get; private set; }
    public InputAction Right { get; private set; }
    public InputAction Up { get; private set; }
    public InputAction Down { get; private set; }
}