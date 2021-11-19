using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateJump : AirState {
        public AirStateJump(Player2DPlatformController player2DPlatform) : base(player2DPlatform) {
        }

        public override void Start(Context context, StateConfig config) {
            Player2DPlatform.SetMotionY(-PlayerConfig.JumpForce);
            DebugJump("Jump start: decelerating to " + -PlayerConfig.JumpForce);
            Player2DPlatform.AnimationJump.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (Jump.JustReleased && Motion.y < -PlayerConfig.JumpForceMin) {
                DebugJump("Short jump: decelerating from " + Motion.y + " to " + -PlayerConfig.JumpForceMin);
                Player2DPlatform.SetMotionY(-PlayerConfig.JumpForceMin);
            }

            Player2DPlatform.AddLateralMotion(XInput, PlayerConfig.Acceleration, PlayerConfig.AirResistance,
                PlayerConfig.StopIfSpeedIsLessThan, 0);
            Player2DPlatform.Flip(XInput);
            Player2DPlatform.ApplyGravity();
            Player2DPlatform.LimitMotion();
            Player2DPlatform.Slide();

            if (Motion.y >= 0) {
                return context.Immediate(typeof(AirStateFallShort));
            }

            return CheckLanding(context);
        }
    }
}