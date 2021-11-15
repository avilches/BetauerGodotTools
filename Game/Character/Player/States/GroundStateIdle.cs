using Tools;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateIdle : GroundState {
        public GroundStateIdle(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.AnimationIdle.Play();
        }

        private OnceAnimationStatus status;

        public override void Execute() {
            CheckAttack();

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