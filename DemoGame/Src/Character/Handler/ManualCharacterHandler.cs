using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;

namespace Veronenger.Character.Handler;

[Service(Lifetime.Transient)]
public class ManualCharacterHandler : ICharacterHandler {
    public readonly ManualActionHandler HandlerJump = new();
    public readonly ManualActionHandler HandlerAttack = new();
    public readonly ManualActionHandler HandlerFloat = new();
    public readonly float HandlerXInput = 0f;
    public readonly float HandlerYInput = 0f;
    
    public IActionHandler Jump => HandlerJump;
    public IActionHandler Attack => HandlerAttack;
    public IActionHandler Float => HandlerFloat;
        
    public float XInput => HandlerXInput;
    public float YInput => HandlerYInput;

    public void EndFrame() {
        HandlerJump.EndFrame();
        // HandlerAttack.Tick();
        // HandlerFloat.Tick();
    }
}