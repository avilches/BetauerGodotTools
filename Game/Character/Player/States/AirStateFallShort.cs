using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallShort : AirState {
        public AirStateFallShort(PlayerController player) : base(player) {
        }

        private bool CoyoteJumpEnabled = false;

        public override void Start(StateConfig config) {
            // Only if the state comes from running -> fall, the Coyote jump is enabled
            // Other cases (state comes from idle or jump), the coyote is not enabled
            CoyoteJumpEnabled = config.HasFlag("CoyoteJumpEnabled");
            Player.FallingClock.Start();
        }

        public override NextState Execute(NextState nextState) {
            CheckAttack();

            if (CoyoteJumpEnabled && CheckCoyoteJump()) {
                return nextState.Immediate(typeof(AirStateJump));
            }
            if (Motion.y > PlayerConfig.START_FALLING_SPEED) {
                return nextState.Immediate(typeof(AirStateFallLong));
            }

            Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);

            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            return CheckLanding(nextState);
        }
    }
}