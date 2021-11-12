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
        }
    }
}