using Godot;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallLong : AirState {
        public AirStateFallLong(Player2DPlatformController player2DPlatform) : base(player2DPlatform) {
        }

        public override void Start(Context context, StateConfig config) {
            if (Player2DPlatform.FallingTimer.Elapsed > PlayerConfig.CoyoteJumpTime) {
                DebugCoyoteJump(
                    $"Coyote jump will never happen in FallLong state: {Player2DPlatform.FallingTimer.Elapsed} > {PlayerConfig.CoyoteJumpTime}");
            }
            Player2DPlatform.AnimationFall.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (CheckCoyoteJump()) {
                return context.Immediate(typeof(AirStateJump));
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