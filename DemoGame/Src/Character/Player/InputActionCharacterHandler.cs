using Betauer.DI;
using Betauer.Input;

namespace Veronenger.Character.Player;

[Service(typeof(ICharacterHandler))]
public class InputActionCharacterHandler : ICharacterHandler {
    [Inject] public InputAction Left { get; set;}
    [Inject] public InputAction Up { get; set;}
    [Inject] public IActionHandler Jump { get; set;}
    [Inject] public IActionHandler Attack { get; set;}
    [Inject] public IActionHandler Float { get; set;}
        
    private AxisAction LateralMotion;
    private AxisAction VerticalMotion;

    [PostCreate]
    public void Configure() {
        LateralMotion = Left.AxisAction;
        VerticalMotion = Up.AxisAction;            
    }
        
    public float XInput => LateralMotion.Strength;
    public float YInput => VerticalMotion.Strength;
}