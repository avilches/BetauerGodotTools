using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public abstract class GroundState : State {
        protected EnemyController Enemy;

        protected EnemyConfig EnemyConfig => Enemy.EnemyConfig;

        public GroundState(EnemyController enemy) {
            Enemy = enemy;
        }
    }
}