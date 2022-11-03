using Betauer;
using Betauer.DI;
using Godot;
using Veronenger.Managers;

namespace Veronenger.Controller.Area {
    public class DeathArea2DController : Area2D {
        [Inject] public CharacterManager CharacterManager { get; set; }

        public override void _Ready() {
            CharacterManager.ConfigurePlayerDeathZone(this);
        }
    }
}