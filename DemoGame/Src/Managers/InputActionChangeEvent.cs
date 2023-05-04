using Betauer.Input;

namespace Veronenger.Managers;

public class InputActionChangeEvent {
    public readonly InputAction InputAction;
    public InputActionChangeEvent(InputAction inputAction) {
        InputAction = inputAction;
    }
}