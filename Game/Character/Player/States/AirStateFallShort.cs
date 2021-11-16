using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallShort : AirState {
        public AirStateFallShort(PlayerController player) : base(player) {
        }

        private bool CoyoteJumpEnabled = false;

        public override void Start(Context context, StateConfig config) {
            // Only if the state comes from running -> fall, the Coyote jump is enabled
            // Other cases (state comes from idle or jump), the coyote is not enabled
            CoyoteJumpEnabled = config.HasFlag("CoyoteJumpEnabled");
            Player.FallingTimer.Reset().Start();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (CoyoteJumpEnabled && CheckCoyoteJump()) {
                return context.Immediate(typeof(AirStateJump));
            }
            if (Motion.y > PlayerConfig.START_FALLING_SPEED) {
                return context.Immediate(typeof(AirStateFallLong));
            }

            Player.AddLateralMotion(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
                PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
            Player.Flip(XInput);

            Player.ApplyGravity();
            Player.LimitMotion();
            Player.Slide();

            return CheckLanding(context);
        }
    }
}