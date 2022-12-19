using Betauer.DI;
using Betauer.Input;

namespace Veronenger.Character.Handler;

// Singleton. All characters can share the same handler because there is only one human player
[Service]
public class InputActionCharacterHandler : ICharacterHandler {
    public IDirectional Directional { get; private set; }
    [Inject] public IAction Jump { get; set; }
    [Inject] public IAction Attack { get; set; }
    [Inject] public IAction Float { get; set; }

    [Inject] private InputAction Left { get; set; }
    [Inject] private InputAction Up { get; set; }

    [PostCreate]
    public void Configure() {
        Directional = new InputDirectional(Left.AxisAction!, Up.AxisAction!);
    }

    public void EndFrame() {
    }
}