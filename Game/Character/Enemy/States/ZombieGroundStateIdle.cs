using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class GroundStateIdle : GroundState {

        public GroundStateIdle(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        public override void Start(Context context, StateConfig config) {
            EnemyZombie.AnimationIdle.PlayLoop();
        }

        public override NextState Execute(Context context) {
            if (!EnemyZombie.IsOnFloor()) {
                EnemyZombie.ApplyGravity();
                EnemyZombie.LimitMotion();
                EnemyZombie.Slide();
                return context.Current();
            }

            if (!EnemyZombie.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                EnemyZombie.ApplyGravity();
            }

            EnemyZombie.MoveSnapping();
            return context.ImmediateIfElapsed(2, typeof(GroundStatePatrolStep));
        }
    }
}