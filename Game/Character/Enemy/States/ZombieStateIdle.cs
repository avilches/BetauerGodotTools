using Betauer.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class ZombieStateIdle : ZombieState {
        public ZombieStateIdle(string name, EnemyZombieController enemyZombie) : base(name, enemyZombie) {
        }

        public override void Start(Context context) {
            EnemyZombie.AnimationIdle.PlayLoop();
        }

        public override NextState Execute(Context context) {
            if (!EnemyZombie.IsOnFloor()) {
                Body.Fall();
                return context.Current();
            }

            if (!Body.IsOnMovingPlatform()) {
                // No gravity in moving platforms
                // Gravity in slopes to avoid go down slowly
                Body.ApplyGravity();
            }

            Body.MoveSnapping();
            return context.ImmediateIfElapsed(2, PatrolStep);
        }
    }
}