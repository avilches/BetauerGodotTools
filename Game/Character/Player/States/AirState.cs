using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public abstract class AirState : PlayerState {
        public AirState(PlayerController player) : base(player) {
        }

        protected bool CheckLanding() {
            if (!Player.IsOnFloor()) return false; // Still in the air! :)

            PlatformManager.BodyStopFallFromPlatform(Player);

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (Player.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    Player.SetMotionX(0);
                }
                GoToIdleState();
            } else {
                GoToRunState();
            }

            // Grounded!
            if (Player.FallingJumpClock.Enabled) {
                if (Player.FallingJumpClock.Elapsed <= PlayerConfig.JUMP_HELPER_TIME) {
                    Debug(PlayerConfig.DEBUG_JUMP_HELPER,
                        $"Helper jump: {Player.FallingJumpClock.Elapsed} <= {PlayerConfig.JUMP_HELPER_TIME} Done!");
                    // Scheduled jump
                    GoToJumpState(false);
                } else {
                    Debug(PlayerConfig.DEBUG_JUMP_HELPER,
                        $"Helper jump: {Player.FallingJumpClock.Elapsed} <= {PlayerConfig.JUMP_HELPER_TIME} TOO MUCH TIME");

                }
                Player.FallingJumpClock.Disable();
            }
            return true;
        }

        protected bool CheckAttack() {
            if (Attack.JustPressed) {
                Player.AnimateJumpAttack();
                return true;
            }
            return false;
        }

        protected bool CheckCoyoteJump() {
            if (Jump.JustPressed) {
                Player.FallingJumpClock.EnableAndStart();
                if (Player.FallingClock.Elapsed <= PlayerConfig.COYOTE_TIME) {
                    Debug(PlayerConfig.DEBUG_JUMP_COYOTE, $"Coyote jump: {Player.FallingClock.Elapsed} <= {PlayerConfig.COYOTE_TIME} Done!");
                    GoToJumpState(true);
                    return true;
                }
                Debug(PlayerConfig.DEBUG_JUMP_COYOTE, $"Coyote jump: {Player.FallingClock.Elapsed} > {PlayerConfig.COYOTE_TIME} TOO LATE");
            }
            return false;
        }

    }
}