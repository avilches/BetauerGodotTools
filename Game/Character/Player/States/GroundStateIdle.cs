using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateIdle : GroundState {
        public GroundStateIdle(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.AnimateIdle();
        }

        public override void Execute() {
            if (!Player.IsAttacking) {
                // Ensure that the jump animation is changed as soon as the previous attack (started from the air) is finished
                CheckAttack();
            }

            if (!Player.IsOnFloor()) {
                GoToFallShortState();
                return;
            }

            if (XInput != 0) {
                GoToRunState();
                return;
            }

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