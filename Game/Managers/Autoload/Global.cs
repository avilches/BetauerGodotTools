using Godot;
using Tools;

namespace Veronenger.Game.Managers.Autoload {
    public class Global : Node /* needed because it's an autoload */ {

        [Inject] private GameManager GameManager;
        [Inject] private CharacterManager CharacterManager;

        public override void _Ready() {
            DiBootstrap.DefaultRepository.AutoWire(this);
            GodotTools.DisableAllNotifications(this);
        }

        public bool IsPlayer(KinematicBody2D player) {
            return CharacterManager.IsPlayer(player);
        }
    }
}