namespace Betauer.Characters.Player.States {
    public class AirStateJump : AirState {
        public AirStateJump(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.SetMotionY(-PlayerConfig.JUMP_FORCE);
            Player.AnimateJump();
        }

        public override void Execute() {
            if (Motion.y > 0) {
                GoToFallState();
                return;
            }

            Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);
            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            CheckLanding();
        }
    }
}