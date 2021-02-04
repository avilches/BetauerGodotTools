using Betauer.Tools.Platforms;

namespace Betauer.Characters.Player.States {
    public abstract class AirState : PlayerState {
        public AirState(PlayerController player) : base(player) {
        }

        public bool CheckLanding() {
            if (!Player.IsOnFloor()) return false;

            PlatformManager.body_stop_falling_from_platform(Player);

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
    }
}