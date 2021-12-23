using Godot;
using Betauer;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Platforms {
    public class PlatformController : DiKinematicBody2D {

        [Inject] public PlatformManager PlatformManager;
        [Export] public bool IsFallingPlatform = false;
        [Export] public bool IsMovingPlatform = false;

        public override void Ready() {
            Configure();
        }

        private void Configure() {
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, IsMovingPlatform);
        }
    }
}