using Betauer.DI;
using Betauer.Input;

namespace Veronenger.Character.Handler;

public class BaseCharacterHandler  {
    private AxisAction LateralMotion;
    private AxisAction VerticalMotion;

    [Inject] public InputAction Left { get; set;}
    [Inject] public InputAction Up { get; set;}

    [PostCreate]
    public void Configure() {
        LateralMotion = Left.AxisAction!;
        VerticalMotion = Up.AxisAction!;            
    }
        
    public float XInput => LateralMotion.Strength;
    public float YInput => VerticalMotion.Strength;
    public bool IsPressingRight => XInput > 0;
    public bool IsPressingLeft => XInput < 0;
    public bool IsPressingUp => YInput < 0;
    public bool IsPressingDown => YInput > 0;

    public virtual void EndFrame() {
    }
}

[Service]
public class InputActionCharacterHandler : BaseCharacterHandler, ICharacterHandler {
    [Inject] public IActionHandler Jump { get; set;}
    [Inject] public IActionHandler Attack { get; set;}
    [Inject] public IActionHandler Float { get; set;}
        
}