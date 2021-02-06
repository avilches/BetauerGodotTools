using Betauer.Tools.Platforms;

namespace Betauer.Characters.Player.States {
    public class GroundStateRun : GroundState {
        public GroundStateRun(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.AnimateRun();
        }

        public override void Execute() {
            if (!Player.IsOnFloor()) {
                GoToFallState();
                return;
            }

            if (XInput == 0 && Motion.x == 0) {
                GoToIdleState();
                return;
            }

            // TODO: create bool HasJumped() along with the StateIdle
            if (Jump.JustPressed) {
                if (IsDown && Player.IsOnFallingPlatform()) {
                    PlatformManager.BodyFallFromPlatform(Player);
                } else {
                    GoToJumpState();
                }
                return;
            }

            // Suelo + no salto + movimiento/inercia. Movemos lateralmente y empujamos hacia abajo


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

            Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.FRICTION,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);

            Player.Flip(XInput);
            Player.ApplyGravity();
            Player.LimitMotion();
            Player.MoveSnapping();


        }
    }
}