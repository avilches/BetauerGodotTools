using Betauer;
using Betauer.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Enemy.States {
    public class ZombieStateAttacked : ZombieState {

        [Inject] private CharacterManager CharacterManager;

        public ZombieStateAttacked(string name, EnemyZombieController enemyZombie) : base(name, enemyZombie) {
        }

        public override void Start(Context context) {
            EnemyZombie.DisableAll();

            if (EnemyZombie.IsToTheLeftOf(CharacterManager.PlayerController.PlayerDetector)) {
                EnemyZombie.AnimationDieLeft.PlayOnce(true);
            } else {
                EnemyZombie.AnimationDieRight.PlayOnce(true);
            }
        }

        public override NextState Execute(Context context) {
            if (!EnemyZombie.AnimationDieRight.Playing && !EnemyZombie.AnimationDieLeft.Playing) {
                EnemyZombie.Dispose();;
            }
            return context.Current();
        }

    }


}