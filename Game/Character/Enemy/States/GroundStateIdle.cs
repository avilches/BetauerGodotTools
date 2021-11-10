using Godot;
using Tools.Statemachine;
using Veronenger.Game.Character.Player.States;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateIdle : State {
        public GroundStateIdle() {
        }

        public override void Start() {
            // Player.AnimateIdle();
        }

        public override void Execute() {
            // GD.Print("NO HAGO NADA....");
            // if (!Player.IsOnFloor()) {
            //     GoToFallState();
            //     return;
            // }
            //
            // if (XInput != 0) {
            //     GoToRunState();
            //     return;
            // }
            //
            // CheckAttack();
            //
            // if (CheckJump()) return;
            //
            // // Suelo + no salto + sin movimiento
            //
            // if (!Player.IsOnMovingPlatform()) {
            //     // No gravity in moving platforms
            //     // Gravity in slopes to avoid go down slowly
            //     Player.ApplyGravity();
            // }
            // Player.MoveSnapping();
        }
    }
}