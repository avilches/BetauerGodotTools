using System.Dynamic;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateIdle : GroundState {
        public GroundStateIdle(PlayerController player) : base(player) {
        }

        public override void Start(StateConfig config) {
            Player.AnimationIdle.Play();
        }

        private OnceAnimationStatus status;

        public override NextState Execute(NextState nextState) {
            CheckAttack();

            if (!Player.IsOnFloor()) {
                return nextState.NextFrame(typeof(AirStateFallShort));
            }

            if (XInput != 0) {
                return nextState.Immediate(typeof(GroundStateRun));
            }

            if (Jump.JustPressed) {
                if (IsDown && Player.IsOnFallingPlatform()) {
                    GameManager.Instance.PlatformManager.BodyFallFromPlatform(Player);
                } else {
                    return nextState.Immediate(typeof(AirStateJump));
                }
            }

            // Suelo + no salto + sin movimiento

            if (!Player.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Player.ApplyGravity();
            }
            Player.MoveSnapping();

            return nextState.Current();
        }
    }
}