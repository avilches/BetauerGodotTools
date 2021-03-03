namespace Betauer.Characters.Player.States {
    public class GroundStateRun : GroundState {
        public GroundStateRun(PlayerController player) : base(player) {
        }

        public override void Start() {
            Player.AnimateRun();
        }

        public override void Execute() {
            if (!Player.IsOnFloor()) {
                GoToFallState(true);
                return;
            }

            if (XInput == 0 && Motion.x == 0) {
                GoToIdleState();
                return;
            }

            if (CheckJump()) return;

            // Suelo + no salto + movimiento/inercia

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

            if (Attack.JustPressed && !Player.IsAttacking) {
                Player.Attack();
            }

            /*

            var slowdownVector = Vector2.ONE
            var slope_down = null

            if is_on_slope && !isJumping && x_input != 0:
            slope_down = sign(colliderNormal.x) == sign(x_input) # pendiente y direccion al mismo lado
            slowdownVector = C.SLOW_ON_SLOPE_DOWN if slope_down else C.SLOW_ON_SLOPE_UP


            move_and_slide_with_snap(motion * slowdownVector
            */

            if (Player.IsAttacking) {
                Player.AddLateralMotion(XInput*0.05f, PlayerConfig.ACCELERATION, PlayerConfig.FRICTION,
                    PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            } else {
                Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.FRICTION,
                    PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            }


            Player.Flip(XInput);
            Player.LimitMotion();
            Player.MoveSnapping();
        }
    }
}