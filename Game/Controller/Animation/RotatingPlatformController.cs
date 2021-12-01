using Godot;
using Tools;
using Veronenger.Game.Managers;
using static Godot.Mathf;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingPlatformController : DiKinematicBody2D {

        [Export] public bool IsFallingPlatform = false;
        [Export] public Vector2 Radius = new Vector2(50, 50);
        [Export] public float RotationDuration = 4.0f;
        [Export] public bool Enabled = true;
        [Inject] public PlatformManager PlatformManager;
        private float _angle = 0;
        private float _speed;

        public override void Ready() {
            Configure();
        }

        public override void _PhysicsProcess(float delta) {
            if (!Enabled) return;
            UpdateAngle(delta);
            UpdatePosition();
        }

        private void Configure() {
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);
            _speed = Tau / RotationDuration;
        }

        private void UpdateAngle(float delta) {
            _angle = Wrap(_angle + _speed * delta, 0, Tau); // # Infinite rotation(in radians)
        }

        private void UpdatePosition() {
            var x = Sin(_angle) * Radius.x;
            var y = Cos(_angle) * Radius.y;
            Position = new Vector2(x, y);
        }

    }

}