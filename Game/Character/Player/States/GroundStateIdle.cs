using System.Dynamic;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateIdle : GroundState {
        public GroundStateIdle(Player2DPlatformController player2DPlatform) : base(player2DPlatform) {
        }

        public override void Start(Context context, StateConfig config) {
            Player2DPlatform.AnimationIdle.PlayLoop();
        }

        private OnceAnimationStatus status;

        public override NextState Execute(Context context) {
            CheckAttack();

            if (!Player2DPlatform.IsOnFloor()) {
                return context.NextFrame(typeof(AirStateFallShort));
            }

            if (XInput != 0) {
                return context.Immediate(typeof(GroundStateRun));
            }

            if (Jump.JustPressed) {
                if (IsDown && Player2DPlatform.IsOnFallingPlatform()) {
                    GameManager.Instance.PlatformManager.BodyFallFromPlatform(Player2DPlatform);
                } else {
                    return context.Immediate(typeof(AirStateJump));
                }
            }

            // Suelo + no salto + sin movimiento

            if (!Player2DPlatform.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Player2DPlatform.ApplyGravity();
            }
            Player2DPlatform.MoveSnapping();

            return context.Current();
        }
    }
}