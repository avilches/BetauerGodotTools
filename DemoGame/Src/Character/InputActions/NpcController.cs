using Betauer.Input;

namespace Veronenger.Character.InputActions;

public class NpcController : ICharacterHandler {
    public InputAction Jump { get; set; } = InputAction.Fake();
    public InputAction Attack { get; set; } = InputAction.Fake();
    public InputAction Float { get; set; } = InputAction.Fake();
    public AxisAction Lateral { get; set; } = AxisAction.Fake();
    public AxisAction Vertical { get; set; } = AxisAction.Fake();

    public void ClearState() {
        Jump.SimulateRelease();
        Attack.SimulateRelease();
        Float.SimulateRelease();
        Lateral.SimulateRelease();
        Vertical.SimulateRelease();
    }
}