namespace Betauer.Input;

public class InputDirectional : IDirectional {
    
    private readonly AxisAction _lateralMotion;
    private readonly AxisAction _verticalMotion;

    public InputDirectional(AxisAction lateralMotion, AxisAction verticalMotion) {
        _lateralMotion = lateralMotion;
        _verticalMotion = verticalMotion;
    }

    public float XInput => _lateralMotion.Strength;
    public float YInput => _verticalMotion.Strength;
    
    public bool IsPressingRight => XInput > 0;
    public bool IsPressingLeft => XInput < 0;
    public bool IsPressingUp => YInput < 0;
    public bool IsPressingDown => YInput > 0;
}