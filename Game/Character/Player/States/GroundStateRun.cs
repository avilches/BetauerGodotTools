using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateRun : GroundState {
        public GroundStateRun(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.AnimateRun();
        }

        public override void Execute() {
            if (!Player.IsAttacking) {
                // Ensure that the jump animation is changed as soon as the previous attack (started from the air) is finished
                CheckAttack();
            }

            if (!Player.IsOnFloor()) {
                GoToFallShortState(true);
                return;
            }

            if (XInput == 0 && Motion.x == 0) {
                GoToIdleState();
                return;
            }

            if (CheckJump()) return;

            // Suelo + no salto + movimiento/inercia
            EnableSlopeStairs();

            if (Player.IsAttacking) {
                Player.StopLateralMotionWithFriction(PlayerConfig.FRICTION,PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN);
            } else {
                Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.FRICTION,
                    PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
                Player.Flip(XInput);
            }

            Player.LimitMotion();
            Player.MoveSnapping();
        }
    }
}