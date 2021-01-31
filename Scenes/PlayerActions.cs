using Godot;

public class PlayerActions {
    public bool IsUsingKeyboard = true;
    public readonly DirectionInput LateralMotion;
    public readonly ActionState Jump;
    public readonly ActionState Attack;

    private readonly ActionInputList _actionInputList;

    public PlayerActions(int deviceId) {
        _actionInputList = new ActionInputList(this, deviceId);

        LateralMotion = _actionInputList.AddDirectionalMotion("Lateral");
        Jump = _actionInputList.AddAction("Jump");
        Attack = _actionInputList.AddAction("Attack");
    }

    public bool Update(EventWrapper w) {
        return _actionInputList.Update(w);
    }

    public void ClearJustState() {
        _actionInputList.ClearJustState();
    }

    public void ConfigureMapping() {
        // TODO: subscribe to signal with the mapping preferences on load or on change
        LateralMotion.Configure(DirectionInput.CursorPositive, DirectionInput.CursorNegative, JoystickList.Axis0, 0.5F);
        LateralMotion.AxisDeadZone = 0.5f;

        Jump.Configure(KeyList.Space, JoystickList.XboxA);
        Attack.Configure(KeyList.C, JoystickList.XboxX);
    }
}