using Betauer.Input;

namespace Veronenger.Game.UI.Settings;

public class InputActionChangeEvent {
    public readonly InputAction InputAction;
    public InputActionChangeEvent(InputAction inputAction) {
        InputAction = inputAction;
    }
}