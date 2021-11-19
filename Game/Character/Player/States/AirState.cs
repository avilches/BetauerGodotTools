using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public abstract class AirState : PlayerState {
        public AirState(Player2DPlatformController player2DPlatform) : base(player2DPlatform) {
        }

        protected NextState CheckLanding(Context context) {
            if (!Player2DPlatform.IsOnFloor()) return context.Current(); // Still in the air! :)

            GameManager.Instance.PlatformManager.BodyStopFallFromPlatform(Player2DPlatform);

            // Check helper jump
            if (!Player2DPlatform.FallingJumpTimer.Stopped) {
                Player2DPlatform.FallingJumpTimer.Stop();
                if (Player2DPlatform.FallingJumpTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                    DebugJumpHelper($"{Player2DPlatform.FallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} Done!");
                    return context.Immediate(typeof(AirStateJump));
                } else {
                    DebugJumpHelper($"{Player2DPlatform.FallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} TOO MUCH TIME");
                }
            }

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (Player2DPlatform.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    Player2DPlatform.SetMotionX(0);
                }
                return context.Immediate(typeof(GroundStateIdle));
            }
            return context.Immediate(typeof(GroundStateRun));
        }

        protected bool CheckAttack() {
            if (!Attack.JustPressed) return false;
            // Attack was pressed
            Player2DPlatform.AnimationJumpAttack.PlayOnce();
            return true;
        }

        protected bool CheckCoyoteJump() {
            if (!Jump.JustPressed) return false;
            // Jump was pressed
            Player2DPlatform.FallingJumpTimer.Reset().Start();
            if (Player2DPlatform.FallingTimer.Elapsed <= PlayerConfig.CoyoteJumpTime) {
                DebugCoyoteJump($"{Player2DPlatform.FallingTimer.Elapsed} <= {PlayerConfig.CoyoteJumpTime} Done!");
                return true;
            }
            DebugCoyoteJump($"{Player2DPlatform.FallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime} TOO LATE");
            return false;
        }
    }
}