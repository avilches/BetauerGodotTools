namespace Betauer.Characters.Player.States {
    public class GroundStateIdle : GroundState {
        public GroundStateIdle(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.AnimateIdle();
        }

        public override void Execute() {
            if (!Player.IsOnFloor()) {
                GoToFallState();
                return;
            }

            if (XInput != 0) {
                GoToRunState();
                return;
            }

            if (CheckJump()) return;

            // Suelo + no salto + sin movimiento

            // Fuerza un movimiento de 0 para que detecte las colisiones y sea empujado por plataformas
            Player.MoveSnapping();
        }
    }
}