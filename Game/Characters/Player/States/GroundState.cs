using Veronenger.Game.Controller;

namespace Veronenger.Game.Characters.Player.States {
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