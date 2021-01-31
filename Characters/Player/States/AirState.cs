namespace Betauer.Characters.Player.States {
    public abstract class AirState : PlayerState {
        public AirState(PlayerController player) : base(player) {
        }

        public void CheckLanding() {
            if (Player.IsOnFloor()) {
                // Just grounded
                /*
                          if C.SQUEEZE_LAND_TIME != 0: sprite.scale = C.SQUEEZE_LAND_SCALE
                          isJumping = false
                          stop_falling_from_platform()
                          if is_on_slope and x_input == 0:
                          motion.x = 0
              #			motion.y = 0

              */
                Debug("Just grounded!");
                if (XInput == 0) {
                    if (Player.IsOnSlope()) {
                        // Evita resbalarse hacia abajo al caer sobre un slope
                        Player.SetMotionX(0);
                    }
                    GoToIdleState();
                } else {
                    GoToRunState();
                }
            }
        }
    }
}