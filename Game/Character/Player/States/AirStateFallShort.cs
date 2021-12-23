using Betauer.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallShort : AirState {
        public AirStateFallShort(string name, PlayerController player) : base(name, player) {
        }

        private bool CoyoteJumpEnabled = false;

        public override void Start(Context context) {
            // Only if the state comes from running -> fall, the Coyote jump is enabled
            // Other cases (state comes from idle or jump), the coyote is not enabled
            CoyoteJumpEnabled = context.Config.HasFlag(CoyoteJumpEnabledKey);
            Player.FallingTimer.Reset().Start();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (CoyoteJumpEnabled && CheckCoyoteJump()) {
                return context.Immediate(StateJump);
            }
            if (Motion.y > MotionConfig.StartFallingSpeed) {
                return context.Immediate(StateFallLong);
            }

            Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                MotionConfig.StopIfSpeedIsLessThan, 0);
            Body.Flip(XInput);

            Body.Fall();

            return CheckLanding(context);
        }
    }
}