using Betauer.Input;

namespace Veronenger.Character.InputActions;

public interface ICharacterHandler {
    public InputAction Jump { get; set; }
    public InputAction Attack { get; set; }
    public InputAction Float { get; set; }
    public AxisAction Lateral { get; set; }
    public AxisAction Vertical { get; set; }
}