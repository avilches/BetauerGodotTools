using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public abstract class ZombieState : State {
        public const string Attacked = nameof(ZombieStateAttacked);
        public const string Idle = nameof(ZombieStateIdle);
        public const string PatrolStep = nameof(ZombieStatePatrolStep);
        public const string PatrolWait = nameof(ZombieStatePatrolWait);

        protected EnemyZombieController EnemyZombie;
        protected MotionBody Body => EnemyZombie.MotionBody;
        protected MotionConfig MotionConfig => EnemyZombie.EnemyConfig.MotionConfig;

        public ZombieState(string name, EnemyZombieController enemyZombie) :base(name){
            EnemyZombie = enemyZombie;
        }
    }
}