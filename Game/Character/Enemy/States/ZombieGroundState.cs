using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public abstract class GroundState : State {
        protected EnemyZombieController EnemyZombie;

        protected EnemyConfig EnemyConfig => EnemyZombie.EnemyConfig;

        public GroundState(EnemyZombieController enemyZombie) {
            EnemyZombie = enemyZombie;
        }
    }
}