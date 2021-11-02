using Betauer.Tools.Platforms;

namespace Betauer.Characters.Player.States {
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

            return true;
        }

        protected void CheckAttack() {
            if (Attack.JustPressed) {
                Player.Attack(false);
            }
        }

    }
}