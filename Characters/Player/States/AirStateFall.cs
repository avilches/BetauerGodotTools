using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Characters.Player.States {
    public class AirStateFall : AirState {
        public AirStateFall(PlayerController player) : base(player) {
        }

        private bool CoyoteJump = false;
        private float _timeLastJump = -1;
        private float _timeElapsed;

        public override void Configure(Dictionary<string, object> config) {
            CoyoteJump = config[COYOTE_JUMP] is bool && (bool) config[COYOTE_JUMP];
        }

        public override void Start() {
            _timeLastJump = -1;
            _timeElapsed = 0;
        }

        public override void Execute() {
            _timeElapsed += Delta;

            if (_timeLastJump >= 0) {
                _timeLastJump += Delta;
            }

            if (Jump.JustPressed) {
                if (CoyoteJump) {
                    if (_timeElapsed <= PlayerConfig.COYOTE_TIME) {
                        Debug(PlayerConfig.DEBUG_JUMP, "Coyote jump: IN TIME: " + _timeElapsed);
                        GoToJumpState();
                        return;
                    }
                    Debug(PlayerConfig.DEBUG_JUMP, "Coyote jump: too late...: " + _timeElapsed);
                }
                _timeLastJump = 0;
            }

            if (Motion.y > PlayerConfig.START_FALLING_SPEED) {
                Player.AnimateFall();
            }

            Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);

            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            if (CheckLanding()) {
                // Grounded!
                if (_timeLastJump > 0 && _timeLastJump <= PlayerConfig.JUMP_HELPER_TIME) {
                    // Scheduled jump
                    GoToJumpState();
                }
            }
        }
    }
}