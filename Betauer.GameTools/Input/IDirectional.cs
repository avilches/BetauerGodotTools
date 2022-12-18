namespace Betauer.Input;

public interface IDirectional {
    
    public float XInput { get; }
    public float YInput { get; }
    
    public bool IsPressingRight => XInput > 0;
    public bool IsPressingLeft => XInput < 0;
    public bool IsPressingUp => YInput < 0;
    public bool IsPressingDown => YInput > 0;
}