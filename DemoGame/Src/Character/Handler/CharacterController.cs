using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.Input.Controller;

namespace Veronenger.Character.Handler;

[Service(Lifetime.Transient)]
public class CharacterController : ICharacterHandler {
    public readonly ActionController JumpController = new();
    public readonly ActionController AttackController = new();
    public readonly ActionController FloatController = new();
    public readonly DirectionalController DirectionalController = new();

    public IDirectional Directional => DirectionalController;
    public IAction Jump => JumpController;
    public IAction Attack => AttackController;
    public IAction Float => FloatController;

    public void EndFrame() {
        JumpController.EndFrame();
        AttackController.EndFrame();
        FloatController.EndFrame();
    }
}