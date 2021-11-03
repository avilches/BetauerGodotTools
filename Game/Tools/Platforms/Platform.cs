using Godot;

namespace Veronenger.Game.Tools.Platforms {
    public class Platform : KinematicBody2D {

        [Export] public bool IsFallingPlatform = false;
        [Export] public bool IsMovingPlatform = false;

        public override void _EnterTree() {
            Configure();
        }

        private void Configure() {
            GameManager.Instance.PlatformManager.RegisterPlatform(this, IsFallingPlatform, IsMovingPlatform);
        }
    }
}