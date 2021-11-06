using Godot;
using Veronenger.Game.Managers;
using Veronenger.Game.Managers.Autoload;
using Veronenger.Game.Tools;

namespace Veronenger.Game.Controller.Platforms {
    public class PlatformController : KinematicBody2D {

        [Export] public bool IsFallingPlatform = false;
        [Export] public bool IsMovingPlatform = false;

        public override void _EnterTree() {
            Configure();
        }

        private void Configure() {
            GameManager.Instance.PlatformManager.ConfigurePlatform(this, IsFallingPlatform, IsMovingPlatform);
        }
    }
}