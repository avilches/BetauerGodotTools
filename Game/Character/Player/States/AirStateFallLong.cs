using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateFallLong : AirState {
        public AirStateFallLong(PlayerController player) : base(player) {
        }

        public override void Start(Context context, StateConfig config) {
            if (Player.FallingTimer.Elapsed > PlayerConfig.COYOTE_TIME) {
                Debug(PlayerConfig.DEBUG_JUMP_COYOTE, $"Coyote jump will never happen in FallLong state: {Player.FallingTimer.Elapsed} > {PlayerConfig.COYOTE_TIME}");
            }
            Player.AnimationFall.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (CheckCoyoteJump()) {
                return context.Immediate(typeof(AirStateJump));
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