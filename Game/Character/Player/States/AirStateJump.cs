using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateJump : AirState {
        public AirStateJump(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.SetMotionY(-PlayerConfig.JUMP_FORCE);
            Debug(PlayerConfig.DEBUG_JUMP_VELOCITY,
                "Jump: decelerating to " + -PlayerConfig.JUMP_FORCE);
            Player.AnimateJump();
        }

        public override void Execute() {
            if (!Player.IsAttacking) {
                // Ensure that the jump animation is changed as soon as the previous attack (started from the ground) is finished
                CheckAttack();
            }

            if (Jump.JustReleased && Motion.y < -PlayerConfig.JUMP_FORCE_MIN) {
                Debug(PlayerConfig.DEBUG_JUMP_VELOCITY,
                    "Short jump: decelerating from " + Motion.y + " to " + -PlayerConfig.JUMP_FORCE_MIN);
                Player.SetMotionY(-PlayerConfig.JUMP_FORCE_MIN);
            }

            Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);
            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            if (CheckLanding()) {
                return;
            }

            if (Motion.y >= 0) { // Ya no sube: se queda quieto exactamente (raro) o empieza a caer
                GoToFallShortState();
            }
        }
    }
}