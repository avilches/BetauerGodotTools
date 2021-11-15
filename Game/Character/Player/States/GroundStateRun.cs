using System.Dynamic;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateRun : GroundState {
        private readonly StateConfig COYOTE_JUMP_ENABLED = new StateConfig().AddFlag("CoyoteJumpEnabled");
        public GroundStateRun(PlayerController player) : base(player) {
        }

        public override void Start(StateConfig config) {
            Player.AnimationRun.Play();
        }

        public override NextState Execute(NextState nextState) {
            CheckAttack();

            if (!Player.IsOnFloor()) {
                return nextState.Immediate(typeof(AirStateFallShort), COYOTE_JUMP_ENABLED);
            }

            if (XInput == 0 && Motion.x == 0) {
                return nextState.Immediate(typeof(GroundStateIdle));
            }

            if (Jump.JustPressed) {
                if (IsDown && Player.IsOnFallingPlatform()) {
                    GameManager.Instance.PlatformManager.BodyFallFromPlatform(Player);
                } else {
                    return nextState.Immediate(typeof(AirStateJump));
                }
            }

            // Suelo + no salto + movimiento/inercia
            EnableSlopeStairs();

            if (Player.IsAttacking) {
                Player.StopLateralMotionWithFriction(PlayerConfig.FRICTION,PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN);
            } else {
                Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.FRICTION,
                    PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
                Player.LimitMotion();
                Player.Flip(XInput);
            }

            Player.MoveSnapping();

            return nextState.Current();
        }
    }
}