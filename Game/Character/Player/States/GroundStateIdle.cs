using Betauer.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Player.States {
    public class GroundStateIdle : GroundState {
        public GroundStateIdle(string name, PlayerController player) : base(name, player) {
        }

        public override void Start(Context context) {
            Player.AnimationIdle.PlayLoop();
        }

        public override NextState Execute(Context context) {
            CheckAttack();

            if (!Player.IsOnFloor()) {
                return context.NextFrame(StateFallShort);
            }

            if (XInput != 0) {
                return context.Immediate(StateRun);
            }

            if (Jump.JustPressed) {
                if (IsDown && Body.IsOnFallingPlatform()) {
                    PlatformManager.BodyFallFromPlatform(Player);
                } else {
                    return context.Immediate(StateJump);
                }
            }

            // Suelo + no salto + sin movimiento

            if (!Body.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Body.ApplyGravity();
            }
            Body.MoveSnapping();

            return context.Current();
        }
    }
}