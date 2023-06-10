using Betauer.Input;

namespace Veronenger.Main.UI.Settings;

public class InputActionChangeEvent {
    public readonly InputAction InputAction;
    public InputActionChangeEvent(InputAction inputAction) {
        InputAction = inputAction;
    }
}