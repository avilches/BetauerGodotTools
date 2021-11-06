using Godot;
using Veronenger.Game.Managers;
using Veronenger.Game.Managers.Autoload;
using Veronenger.Game.Tools;

namespace Veronenger.Game.Controller.Area {
    public class DeathArea2DController : Area2D {
        public override void _EnterTree() {
            GameManager.Instance.AreaManager.ConfigureDeathZone(this);
        }
    }
}