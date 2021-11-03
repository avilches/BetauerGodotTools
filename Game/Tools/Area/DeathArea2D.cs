using Godot;

namespace Veronenger.Game.Tools.Area {
    public class DeathArea2D : Area2D {
        public override void _EnterTree() {
            GameManager.Instance.AreaManager.RegisterDeathZone(this);
        }
    }
}