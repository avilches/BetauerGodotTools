using Godot;
using Betauer;
using Betauer.DI;
using Veronenger.Managers;

namespace Veronenger.Controller.Platforms {
    public class PlatformController :KinematicBody2D {

        [Inject] public PlatformManager PlatformManager { get; set; }
        [Export] public bool IsFallingPlatform = false;
        [Export] public bool IsMovingPlatform = false;

        public override void _Ready() {
            Configure();
        }

        private void Configure() {
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, IsMovingPlatform);
        }
    }
}