using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallLong : AirState {
        public AirStateFallLong(PlayerController player) : base(player) {
        }

        public override void Start() {
            if (Player.FallingClock.Elapsed > PlayerConfig.COYOTE_TIME) {
                Debug(PlayerConfig.DEBUG_JUMP_COYOTE, $"Coyote jump will never happen in FallLong state: {Player.FallingClock.Elapsed} > {PlayerConfig.COYOTE_TIME}");
            }
            Player.AnimateFall();
        }



        public override void Execute() {
            if (!Player.IsAttacking) {
                CheckAttack();
            }

            if (CheckCoyoteJump()) {
                return;
            }

            Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);

            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            CheckLanding();
        }
    }
}