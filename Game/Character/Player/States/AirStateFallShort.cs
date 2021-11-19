using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallShort : AirState {
        public AirStateFallShort(Player2DPlatformController player2DPlatform) : base(player2DPlatform) {
        }

        private bool CoyoteJumpEnabled = false;

        public override void Start(Context context, StateConfig config) {
            // Only if the state comes from running -> fall, the Coyote jump is enabled
            // Other cases (state comes from idle or jump), the coyote is not enabled
            CoyoteJumpEnabled = config.HasFlag("CoyoteJumpEnabled");
            Player2DPlatform.FallingTimer.Reset().Start();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (CoyoteJumpEnabled && CheckCoyoteJump()) {
                return context.Immediate(typeof(AirStateJump));
            }
            if (Motion.y > PlayerConfig.StartFallingSpeed) {
                return context.Immediate(typeof(AirStateFallLong));
            }

            Player2DPlatform.AddLateralMotion(XInput, PlayerConfig.Acceleration, PlayerConfig.AirResistance,
                PlayerConfig.StopIfSpeedIsLessThan, 0);
            Player2DPlatform.Flip(XInput);

            Player2DPlatform.ApplyGravity();
            Player2DPlatform.LimitMotion();
            Player2DPlatform.Slide();

            return CheckLanding(context);
        }
    }
}