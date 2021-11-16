using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public abstract class AirState : PlayerState {
        public AirState(PlayerController player) : base(player) {
        }

        protected NextState CheckLanding(Context context) {
            if (!Player.IsOnFloor()) return context.Current(); // Still in the air! :)

            GameManager.Instance.PlatformManager.BodyStopFallFromPlatform(Player);

            // Check helper jump
            if (!Player.FallingJumpTimer.Stopped) {
                Player.FallingJumpTimer.Stop();
                if (Player.FallingJumpTimer.Elapsed <= PlayerConfig.JUMP_HELPER_TIME) {
                    Debug(PlayerConfig.DEBUG_JUMP_HELPER,
                        $"Helper jump: {Player.FallingJumpTimer.Elapsed} <= {PlayerConfig.JUMP_HELPER_TIME} Done!");
                    return context.Immediate(typeof(AirStateJump));
                } else {
                    Debug(PlayerConfig.DEBUG_JUMP_HELPER,
                        $"Helper jump: {Player.FallingJumpTimer.Elapsed} <= {PlayerConfig.JUMP_HELPER_TIME} TOO MUCH TIME");
                }
            }

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (Player.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    Player.SetMotionX(0);
                }
                return context.Immediate(typeof(GroundStateIdle));
            }
            return context.Immediate(typeof(GroundStateRun));
        }

        protected bool CheckAttack() {
            if (!Attack.JustPressed) return false;
            // Attack was pressed
            Player.AnimationJumpAttack.PlayOnce();
            return true;
        }

        protected bool CheckCoyoteJump() {
            if (!Jump.JustPressed) return false;
            // Jump was pressed
            Player.FallingJumpTimer.Reset().Start();
            if (Player.FallingTimer.Elapsed <= PlayerConfig.COYOTE_TIME) {
                Debug(PlayerConfig.DEBUG_JUMP_COYOTE,
                    $"Coyote jump: {Player.FallingTimer.Elapsed} <= {PlayerConfig.COYOTE_TIME} Done!");
                return true;
            }
            Debug(PlayerConfig.DEBUG_JUMP_COYOTE,
                $"Coyote jump: {Player.FallingTimer.Elapsed} > {PlayerConfig.COYOTE_TIME} TOO LATE");
            return false;
        }
    }
}