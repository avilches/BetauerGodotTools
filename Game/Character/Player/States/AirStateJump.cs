using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateJump : AirState {
        public AirStateJump(PlayerController player) : base(player) {
        }

        public override void Start(Context context, StateConfig config) {
            Player.SetMotionY(-PlayerConfig.JUMP_FORCE);
            DebugJump("Jump start: decelerating to " + -PlayerConfig.JUMP_FORCE);
            Player.AnimationJump.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (Jump.JustReleased && Motion.y < -PlayerConfig.JUMP_FORCE_MIN) {
                DebugJump("Short jump: decelerating from " + Motion.y + " to " + -PlayerConfig.JUMP_FORCE_MIN);
                Player.SetMotionY(-PlayerConfig.JUMP_FORCE_MIN);
            }

            Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);
            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            if (Motion.y >= 0) {
                return context.Immediate(typeof(AirStateFallShort));
            }

            return CheckLanding(context);
        }
    }
}