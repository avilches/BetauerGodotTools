using Betauer;
using Betauer.DI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Area {
    public class DeathArea2DController : Area2D {
        [Inject] public CharacterManager CharacterManager;

        public override void _Ready() {
            CharacterManager.ConfigurePlayerDeathZone(this);
        }
    }
}