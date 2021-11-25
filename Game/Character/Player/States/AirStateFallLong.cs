using Godot;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallLong : AirState {
        public AirStateFallLong(PlayerController player) : base(player) {
        }

        public override void Start(Context context, StateConfig config) {
            if (Player.FallingTimer.Elapsed > PlayerConfig.CoyoteJumpTime) {
                DebugCoyoteJump(
                    $"Coyote jump will never happen in FallLong state: {Player.FallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime}");
            }
            Player.AnimationFall.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (CheckCoyoteJump()) {
                return context.Immediate(typeof(AirStateJump));
            }

            Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                MotionConfig.StopIfSpeedIsLessThan, 0);
            Body.Flip(XInput);

            Body.Fall();

            return CheckLanding(context);
        }
    }
}