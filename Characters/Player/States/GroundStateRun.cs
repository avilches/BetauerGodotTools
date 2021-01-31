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

            if (Jump.JustPressed) {
                GoToJumpState();
                return;
            }

            // Suelo + no salto + movimiento/inercia. Movemos lateralmente y empujamos hacia abajo

            Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.FRICTION,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);

            Player.Flip(XInput);
            Player.ApplyGravity();
            Player.LimitMotion();
            Player.MoveSnapping();


        }
    }
}