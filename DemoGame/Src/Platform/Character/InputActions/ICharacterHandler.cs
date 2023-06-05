using Betauer.Input;

namespace Veronenger.Platform.Character.InputActions;

public interface ICharacterHandler {
    public InputAction Jump { get; }
    public InputAction Attack { get; }
    public InputAction Float { get; }
    public AxisAction Lateral { get; }
    public AxisAction Vertical { get; }
}