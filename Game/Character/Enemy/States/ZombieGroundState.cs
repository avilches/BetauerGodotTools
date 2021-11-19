using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public abstract class GroundState : State {
        protected EnemyZombieController EnemyZombie;
        protected MotionBody BOdy => EnemyZombie.MotionBody;

        protected MotionConfig MotionConfig => EnemyZombie.EnemyConfig.MotionConfig;

        public GroundState(EnemyZombieController enemyZombie) {
            EnemyZombie = enemyZombie;
        }
    }
}