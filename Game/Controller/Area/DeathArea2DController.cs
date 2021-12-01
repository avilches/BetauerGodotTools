using Tools;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Area {
    public class DeathArea2DController : DiArea2D {
        [Inject] public CharacterManager CharacterManager;

        public override void Ready() {
            CharacterManager.ConfigurePlayerDeathZone(this);
        }
    }
}