using Betauer.Input;

namespace Veronenger.Game.Platform.Character.InputActions;

public class NpcController : ICharacterHandler {
    public InputAction Jump { get; set; } = InputAction.Simulator();
    public InputAction Attack { get; set; } = InputAction.Simulator();
    public InputAction Float { get; set; } = InputAction.Simulator();
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