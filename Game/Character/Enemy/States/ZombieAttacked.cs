using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class ZombieAttacked : GroundState {

        private PlayerController _player;

        public ZombieAttacked(EnemyZombieController enemyZombie) : base(enemyZombie) {
        }

        public override void Start(Context context, StateConfig config) {
            EnemyZombie.DisableAll();
            _player = config.Get<PlayerController>("player");

            EnemyZombie.AnimationDie.PlayOnce();
        }

        public override NextState Execute(Context context) {
            if (!EnemyZombie.AnimationDie.Playing) {
                EnemyZombie.Dispose();;
            }
            return context.Current();
        }

    }


}