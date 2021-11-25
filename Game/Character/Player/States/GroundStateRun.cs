using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateRun : GroundState {
        private static readonly StateConfig ConfigWithCoyoteJumpEnabled =
            new StateConfig().AddFlag(AirStateFallShort.COYOTE_JUMP_ENABLED_KEY);

        public GroundStateRun(PlayerController player) : base(player) {
        }

        public override void Start(Context context) {
            Player.AnimationRun.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (!Player.IsOnFloor()) {
                return context.Immediate(typeof(AirStateFallShort), ConfigWithCoyoteJumpEnabled);
            }

            if (XInput == 0 && Motion.x == 0) {
                return context.Immediate(typeof(GroundStateIdle));
            }

            if (Jump.JustPressed) {
                if (IsDown && Body.IsOnFallingPlatform()) {
                    PlatformManager.BodyFallFromPlatform(Player);
                } else {
                    return context.Immediate(typeof(AirStateJump));
                }
            }

            // Suelo + no salto + movimiento/inercia
            EnableSlopeStairs();

            if (Player.IsAttacking) {
                Body.StopLateralMotionWithFriction(MotionConfig.Friction, MotionConfig.StopIfSpeedIsLessThan);
            } else {
                Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.Friction,
                    MotionConfig.StopIfSpeedIsLessThan, 0);
                Body.LimitMotion();
                Body.Flip(XInput);
            }

            Body.MoveSnapping();

            return context.Current();
        }
    }
}