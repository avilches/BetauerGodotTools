using System.Collections.Generic;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallShort : AirState {
        public AirStateFallShort(PlayerController player) : base(player) {
        }

        private bool CoyoteJumpEnabled = false;

        public override void Configure(Dictionary<string, object> config) {
            CoyoteJumpEnabled = config[COYOTE_JUMP] is bool && (bool)config[COYOTE_JUMP];
        }

        public override void Start() {
            Player.FallingClock.EnableAndStart();
        }

        public override void Execute() {
            if (!Player.IsAttacking) {
                CheckAttack();
            }

            if (CoyoteJumpEnabled && CheckCoyoteJump()) {
                return;
            }
            if (Motion.y > PlayerConfig.START_FALLING_SPEED) {
                GoToFallLongState();
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