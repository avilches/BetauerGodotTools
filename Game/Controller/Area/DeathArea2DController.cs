using Tools;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Area {
    public class DeathArea2DController : DiArea2D {
        [Inject] public AreaManager AreaManager;

        public override void _EnterTree() {
            AreaManager.ConfigureDeathZone(this);
        }
    }
}