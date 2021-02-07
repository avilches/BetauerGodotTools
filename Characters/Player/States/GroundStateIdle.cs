using Betauer.Tools.Platforms;

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

            // Suelo + no salto + sin movimiento. Empujamos hacia abajo

            Player.ApplyGravity();
            Player.LimitMotion();
            Player.MoveSnapping();

        }
    }
}