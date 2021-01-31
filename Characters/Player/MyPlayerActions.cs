using Betauer.Tools.Input;
using Godot;

namespace Betauer.Characters.Player {
    public class MyPlayerActions : PlayerActions {
        public readonly DirectionInput LateralMotion;
        public readonly ActionState Jump;
        public readonly ActionState Attack;

        public MyPlayerActions(int deviceId) : base(deviceId) {
            LateralMotion = ActionInputList.AddDirectionalMotion("Lateral");
            Jump = ActionInputList.AddAction("Jump");
            Attack = ActionInputList.AddAction("Attack");
        }

        public void ConfigureMapping() {
            // TODO: subscribe to signal with the mapping preferences on load or on change
            LateralMotion.Configure(DirectionInput.CursorPositive, DirectionInput.CursorNegative, JoystickList.Axis0,
                0.5F);
            LateralMotion.AxisDeadZone = 0.5f;

            Jump.Configure(KeyList.Space, JoystickList.XboxA);
            Attack.Configure(KeyList.C, JoystickList.XboxX);
        }
    }
}