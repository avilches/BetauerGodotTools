using Tools.Statemachine;
using Veronenger.Game.Controller.Character;

namespace Veronenger.Game.Character.Enemy.States {
    public class ZombieStateAttacked : ZombieState {

        public static string PLAYER_KEY = "player";

        private PlayerController _player;

        public ZombieStateAttacked(string name, EnemyZombieController enemyZombie) : base(name, enemyZombie) {
        }

        public override void Start(Context context) {
            EnemyZombie.DisableAll();
            _player = context.Config.Get<PlayerController>(PLAYER_KEY);

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