using System;

namespace Betauer.Characters.Player.States {
    public class AirStateFall : AirState {
        public AirStateFall(PlayerController player) : base(player) {
        }

        private float _timeLastJump = -1;
        public override void Start() {
            _timeLastJump = -1;
        }

        public override void Execute() {
            if (_timeLastJump >= 0) {
                _timeLastJump += Delta;
            }

            if (Jump.JustPressed) {
                _timeLastJump = 0;
            }

            if (Motion.y > PlayerConfig.START_FALLING_SPEED) {
                Player.AnimateFall();
            }

            Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);

            bool wasOnFloor = Player.IsOnFloor();
            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            if (CheckLanding()) {
                if (_timeLastJump > 0 && _timeLastJump <= PlayerConfig.JUMP_HELPER_TIME) {
                    GoToJumpState();
                }
            }
        }
    }
}