using Betauer.Input;

namespace Veronenger.Character.InputActions;

public class CharacterController : ICharacterHandler {
    public InputAction Jump { get; set; } = InputAction.Fake();
    public InputAction Attack { get; set; } = InputAction.Fake();
    public InputAction Float { get; set; } = InputAction.Fake();
    public AxisAction Lateral { get; set; } = AxisAction.Fake();
    public AxisAction Vertical { get; set; } = AxisAction.Fake();
}