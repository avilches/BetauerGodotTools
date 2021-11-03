using Veronenger.Game.Controller;

namespace Veronenger.Game.Characters.Player.States {
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

            CheckAttack();

            if (CheckJump()) return;

            // Suelo + no salto + sin movimiento

            if (!Player.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Player.ApplyGravity();
            }
            Player.MoveSnapping();
        }
    }
}