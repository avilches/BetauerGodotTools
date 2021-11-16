using System.Dynamic;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateIdle : GroundState {
        public GroundStateIdle(PlayerController player) : base(player) {
        }

        public override void Start(Context context, StateConfig config) {
            Player.AnimationIdle.PlayLoop();
        }

        private OnceAnimationStatus status;

        public override NextState Execute(Context context) {
            CheckAttack();

            if (!Player.IsOnFloor()) {
                return context.NextFrame(typeof(AirStateFallShort));
            }

            if (XInput != 0) {
                return context.Immediate(typeof(GroundStateRun));
            }

            if (Jump.JustPressed) {
                if (IsDown && Player.IsOnFallingPlatform()) {
                    GameManager.Instance.PlatformManager.BodyFallFromPlatform(Player);
                } else {
                    return context.Immediate(typeof(AirStateJump));
                }
            }

            // Suelo + no salto + sin movimiento

            if (!Player.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Player.ApplyGravity();
            }
            Player.MoveSnapping();

            return context.Current();
        }
    }
}