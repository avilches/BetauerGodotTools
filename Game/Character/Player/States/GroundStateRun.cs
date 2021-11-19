using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateRun : GroundState {
        private readonly StateConfig COYOTE_JUMP_ENABLED = new StateConfig().AddFlag("CoyoteJumpEnabled");

        public GroundStateRun(Player2DPlatformController player2DPlatform) : base(player2DPlatform) {
        }

        public override void Start(Context context, StateConfig config) {
            Player2DPlatform.AnimationRun.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (!Player2DPlatform.IsOnFloor()) {
                return context.Immediate(typeof(AirStateFallShort), COYOTE_JUMP_ENABLED);
            }

            if (XInput == 0 && Motion.x == 0) {
                return context.Immediate(typeof(GroundStateIdle));
            }

            if (Jump.JustPressed) {
                if (IsDown && Player2DPlatform.IsOnFallingPlatform()) {
                    GameManager.Instance.PlatformManager.BodyFallFromPlatform(Player2DPlatform);
                } else {
                    return context.Immediate(typeof(AirStateJump));
                }
            }

            // Suelo + no salto + movimiento/inercia
            EnableSlopeStairs();

            if (Player2DPlatform.IsAttacking) {
                Player2DPlatform.StopLateralMotionWithFriction(PlayerConfig.Friction, PlayerConfig.StopIfSpeedIsLessThan);
            } else {
                Player2DPlatform.AddLateralMotion(XInput, PlayerConfig.Acceleration, PlayerConfig.Friction,
                    PlayerConfig.StopIfSpeedIsLessThan, 0);
                Player2DPlatform.LimitMotion();
                Player2DPlatform.Flip(XInput);
            }

            Player2DPlatform.MoveSnapping();

            return context.Current();
        }
    }
}