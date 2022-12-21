using Godot;
using Betauer;
using Betauer.DI;
using Veronenger.Managers;
using static Godot.Mathf;

namespace Veronenger.Controller.Animation {
    public partial class RotatingPlatformController : CharacterBody2D {

        [Export] public Vector2 Radius = new Vector2(50, 50);
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager { get; set;}
        private float _angle = 0;
        private float _speed;

        public override void _Ready() {
            Configure();
        }

        public override void _PhysicsProcess(double delta) {
            UpdateAngle(delta);
            UpdatePosition();
        }

        private void Configure() {
            _speed = Tau / RotationDuration;
        }

        private void UpdateAngle(double delta) {
            _angle = Wrap(_angle + _speed * (float)delta, 0, Tau); // # Infinite rotation(in radians)
        }

        private void UpdatePosition() {
            var x = Sin(_angle) * Radius.x;
            var y = Cos(_angle) * Radius.y;
            Position = new Vector2(x, y);
        }

    }

}