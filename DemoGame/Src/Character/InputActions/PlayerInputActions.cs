using Betauer.DI;
using Betauer.Input;

namespace Veronenger.Character.InputActions;

[Service]
public class PlayerInputActions : ICharacterHandler {
    [Inject] public InputAction Jump { get; set; }
    [Inject] public InputAction Attack { get; set; }
    [Inject] public InputAction Float { get; set; }
    [Inject] public AxisAction Lateral { get; set; }
    [Inject] public AxisAction Vertical { get; set; }
    
    [Inject] public InputAction Left { get; set; }
    [Inject] public InputAction Right { get; set; }
    [Inject] public InputAction Up { get; set; }
    [Inject] public InputAction Down { get; set; }

}