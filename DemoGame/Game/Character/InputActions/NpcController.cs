using Betauer.Input;

namespace Veronenger.Game.Character.InputActions;

public class NpcController : ICharacterHandler {
    public InputAction Jump { get; set; } = InputAction.Mock();
    public InputAction Attack { get; set; } = InputAction.Mock();
    public InputAction Float { get; set; } = InputAction.Mock();
    public AxisAction Lateral { get; set; } = AxisAction.Mock();
    public AxisAction Vertical { get; set; } = AxisAction.Mock();

    public void ClearState() {
        Jump.SimulateRelease();
        Attack.SimulateRelease();
        Float.SimulateRelease();
        Lateral.SimulateRelease();
        Vertical.SimulateRelease();
    }
}