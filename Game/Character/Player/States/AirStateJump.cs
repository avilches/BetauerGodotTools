using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class AirStateJump : AirState {
        public AirStateJump(string name, PlayerController player) : base(name, player) {
        }

        public override void Start(Context context) {
            Body.SetMotionY(-MotionConfig.JumpForce);
            DebugJump("Jump start: decelerating to " + -MotionConfig.JumpForce);
            Player.AnimationJump.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (Jump.JustReleased && Motion.y < -MotionConfig.JumpForceMin) {
                DebugJump("Short jump: decelerating from " + Motion.y + " to " + -MotionConfig.JumpForceMin);
                Body.SetMotionY(-MotionConfig.JumpForceMin);
            }

            Body.AddLateralMotion(XInput, MotionConfig.Acceleration, MotionConfig.AirResistance,
                MotionConfig.StopIfSpeedIsLessThan, 0);
            Body.Flip(XInput);
            Body.Fall();

            if (Motion.y >= 0) {
                return context.Immediate(StateFallShort);
            }

            return CheckLanding(context);
        }
    }
}