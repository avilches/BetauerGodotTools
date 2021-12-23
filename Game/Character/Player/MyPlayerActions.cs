using Godot;
using Betauer.Input;

namespace Veronenger.Game.Character.Player {
    public class MyPlayerActions : PlayerActions {
        public readonly DirectionInput LateralMotion;
        public readonly DirectionInput VerticalMotion;
        public readonly ActionState Jump;
        public readonly ActionState Attack;

        public MyPlayerActions(int deviceId) : base(deviceId) {
            LateralMotion = ActionInputList.AddDirectionalMotion("Lateral");
            VerticalMotion = ActionInputList.AddDirectionalMotion("Vertical");
            Jump = ActionInputList.AddAction("Jump");
            Attack = ActionInputList.AddAction("Attack");
        }

        public void ConfigureMapping() {
            // TODO: subscribe to signal with the mapping preferences on load or on change
            LateralMotion.ClearConfig();
            LateralMotion.ConfigureKeys(DirectionInput.CursorLateralPositive, DirectionInput.CursorLateralNegative);
            LateralMotion.ConfigureAxis(JoystickList.Axis0);
            LateralMotion.ConfigureButtons(DirectionInput.DpadLateralPositive, DirectionInput.DpadLateralNegative);
            LateralMotion.AxisDeadZone = 0.5f;

            VerticalMotion.ClearConfig();
            VerticalMotion.ConfigureKeys(DirectionInput.CursorVerticalPositive, DirectionInput.CursorVerticalNegative);
            VerticalMotion.ConfigureAxis(JoystickList.Axis1);
            VerticalMotion.ConfigureButtons(DirectionInput.DpadVerticalPositive, DirectionInput.DpadVerticalNegative);
            VerticalMotion.AxisDeadZone = 0.5f;

            Jump.Configure(KeyList.Space, JoystickList.XboxA);
            Attack.Configure(KeyList.C, JoystickList.XboxX);
        }
    }
}