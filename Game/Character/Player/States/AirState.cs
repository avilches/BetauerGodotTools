using Betauer.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public abstract class AirState : PlayerState {

        protected AirState(string name, PlayerController player) : base(name, player) {
        }

        protected NextState CheckLanding(Context context) {
            if (!Player.IsOnFloor()) return context.Current(); // Still in the air! :)

            PlatformManager.BodyStopFallFromPlatform(Player);

            // Check helper jump
            if (!Player.FallingJumpTimer.Stopped) {
                Player.FallingJumpTimer.Stop();
                if (Player.FallingJumpTimer.Elapsed <= PlayerConfig.JumpHelperTime) {
                    DebugJumpHelper($"{Player.FallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} Done!");
                    return context.Immediate(StateJump);
                }
                DebugJumpHelper($"{Player.FallingJumpTimer.Elapsed} <= {PlayerConfig.JumpHelperTime} TOO MUCH TIME");
            }

            // Debug("Just grounded!");
            if (XInput == 0) {
                if (Body.IsOnSlope()) {
                    // Evita resbalarse hacia abajo al caer sobre un slope
                    Body.SetMotionX(0);
                }
                return context.Immediate(StateIdle);
            }
            return context.Immediate(StateRun);
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