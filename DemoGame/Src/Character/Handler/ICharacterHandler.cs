using Betauer.Input;

namespace Veronenger.Character.Handler;

public interface ICharacterHandler {
    public float XInput => 0f;
    public float YInput => 0f;
    public bool IsPressingRight => XInput > 0;
    public bool IsPressingLeft => XInput < 0;
    public bool IsPressingUp => YInput < 0;
    public bool IsPressingDown => YInput > 0;
        
    public IActionHandler Jump { get; }
    public IActionHandler Attack { get; }
    public IActionHandler Float { get; }

    public void EndFrame();
}