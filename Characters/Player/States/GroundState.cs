namespace Betauer.Characters.Player.States {
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

    }
}