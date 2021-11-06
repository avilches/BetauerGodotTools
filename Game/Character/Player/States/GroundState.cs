using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public abstract class GroundState : PlayerState {
        public GroundState(PlayerController player) : base(player) {
        }

        protected bool CheckJump() {
            if (!Jump.JustPressed) return false;
            if (IsDown && Player.IsOnFallingPlatform()) {
                PlatformManager.BodyFallFromPlatform(Player);
            } else {
                GoToJumpState();
            }
            return true;
        }

        protected void CheckAttack() {
            if (Attack.JustPressed) {
                Player.Attack(true);
            }
        }

    }
}