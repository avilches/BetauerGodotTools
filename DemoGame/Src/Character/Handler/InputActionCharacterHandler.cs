using Betauer.DI;
using Betauer.Input;

namespace Veronenger.Character.Handler;

// Singleton. All characters can share the same handler because there is only one human player
[Service]
public class InputActionCharacterHandler : ICharacterHandler {
    public IDirectional Directional { get; private set; }
    public IAction JumpAction => Jump;
    public IAction AttackAction => Attack;
    public IAction FloatAction => Float;

    [Inject] public InputAction Jump { get; set; }
    [Inject] public InputAction Attack { get; set; }
    [Inject] public InputAction Float { get; set; }
    [Inject] private InputAction Left { get; set; }
    [Inject] private InputAction Up { get; set; }

    [PostInject]
    public void Configure() {
        Directional = new InputDirectional(Left.AxisAction!, Up.AxisAction!);
    }

    public void EndFrame() {
    }
}