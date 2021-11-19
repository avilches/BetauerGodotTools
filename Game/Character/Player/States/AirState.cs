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
                if (Player.FallingJumpTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                    DebugJumpHelper($"{Player.FallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} Done!");
                    return context.Immediate(typeof(AirStateJump));
                }
                DebugJumpHelper($"{Player.FallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} TOO MUCH TIME");
            }

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (Body.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    Body.SetMotionX(0);
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
            if (Player.FallingTimer.Elapsed <= PlayerConfig.CoyoteJumpTime) {
                DebugCoyoteJump($"{Player.FallingTimer.Elapsed} <= {PlayerConfig.CoyoteJumpTime} Done!");
                return true;
            }
            DebugCoyoteJump($"{Player.FallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime} TOO LATE");
            return false;
        }
    }
}