using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateIdle : State {
        private EnemyController Enemy;

        public GroundStateIdle(EnemyController enemy) {
            Enemy = enemy;
        }

        public override void Start() {
            Enemy.AnimateIdle();
        }

        public override void Execute() {
            if (!Enemy.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Enemy.ApplyGravity();
            }
            Enemy.MoveSnapping();
        }
    }
}