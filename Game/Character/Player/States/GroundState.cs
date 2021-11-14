using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public abstract class GroundState : PlayerState {
        public GroundState(PlayerController player) : base(player) {
        }

        protected bool CheckJump() {
            if (!Jump.JustPressed) return false;
            if (IsDown && Player.IsOnFallingPlatform()) {
                GameManager.Instance.PlatformManager.BodyFallFromPlatform(Player);
            } else {
                GoToJumpState(true);
            }
            return true;
        }

        protected bool CheckAttack() {
            if (Attack.JustPressed && !Player.IsAttacking) {
                Player.AnimateAttack();
                return true;
            }
            return false;
        }

        protected void EnableSlopeStairs() {
            if (Player.IsOnSlopeStairsDown()) {
                if (IsUp) {
                    Player.EnableSlopeStairs();
                } else {
                    Player.DisableSlopeStairs();
                }
            } else if (Player.IsOnSlopeStairsUp()) {
                if (IsDown) {
                    Player.EnableSlopeStairs();
                } else {
                    Player.DisableSlopeStairs();
                }
            }
        }
    }
}