using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public abstract class AirState : PlayerState {
        public AirState(PlayerController player) : base(player) {
        }

        protected NextState CheckLanding(NextState nextState) {
            if (!Player.IsOnFloor()) return nextState.Current(); // Still in the air! :)

            GameManager.Instance.PlatformManager.BodyStopFallFromPlatform(Player);

            // Check helper jump
            if (!Player.FallingJumpClock.Stopped) {
                Player.FallingJumpClock.Pause();
                if (Player.FallingJumpClock.Elapsed <= base.PlayerConfig.JUMP_HELPER_TIME) {
                    Debug(PlayerConfig.DEBUG_JUMP_HELPER,
                        $"Helper jump: {Player.FallingJumpClock.Elapsed} <= {base.PlayerConfig.JUMP_HELPER_TIME} Done!");
                    return nextState.Immediate(typeof(AirStateJump));
                } else {
                    Debug(PlayerConfig.DEBUG_JUMP_HELPER,
                        $"Helper jump: {Player.FallingJumpClock.Elapsed} <= {base.PlayerConfig.JUMP_HELPER_TIME} TOO MUCH TIME");
                }
            }

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (Player.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    Player.SetMotionX(0);
                }
                return nextState.Immediate(typeof(GroundStateIdle));
            }
            return nextState.Immediate(typeof(GroundStateRun));
        }

        protected bool CheckAttack() {
            if (Attack.JustPressed) {
                Player.AnimationJumpAttack.Play();
                return true;
            }
            return false;
        }

        protected bool CheckCoyoteJump() {
            if (Jump.JustPressed) {
                Player.FallingJumpClock.Start();
                if (Player.FallingClock.Elapsed <= base.PlayerConfig.COYOTE_TIME) {
                    Debug(PlayerConfig.DEBUG_JUMP_COYOTE,
                        $"Coyote jump: {Player.FallingClock.Elapsed} <= {base.PlayerConfig.COYOTE_TIME} Done!");
                    return true;
                }
                Debug(PlayerConfig.DEBUG_JUMP_COYOTE,
                    $"Coyote jump: {Player.FallingClock.Elapsed} > {base.PlayerConfig.COYOTE_TIME} TOO LATE");
            }
            return false;
        }
    }
}