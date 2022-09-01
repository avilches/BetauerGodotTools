using System.Collections.Generic;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Nodes;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenSpacedController : Node2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public Vector2 Radius = new Vector2(50, 50);
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager { get; set;}


        private List<PhysicsBody2D> _platforms;
        private Sequence _sequence;
        private SceneTreeTween _sceneTreeTween;

        public override void _Ready() {
            Configure();
        }

        // var _speed = Tau / RotationDuration;
        // _angle = Wrap(_angle + _speed * delta, 0, Tau); // Infinite rotation(in radians
        private void RotateSpaced(float angle) => RotateSpaced(_platforms, angle, Radius);

        private void Configure() {
            _platforms = this.GetChildren<PhysicsBody2D>();
            PlatformManager.ConfigurePlatformList(_platforms, IsFallingPlatform, true);
            _sequence = Sequence.Create(this)
                .AnimateSteps<float>(RotateSpaced)
                .From(0).To(Mathf.Tau, 4, Easing.Linear)
                .EndAnimate()
                .SetInfiniteLoops();
            _sceneTreeTween = _sequence.PlayForever();
        }

        /*
        * Distribuye por el circulo de manera espaciado y las gira
        */
        public static void RotateSpaced(List<PhysicsBody2D> nodes, float angle, Vector2 radius) {
            var count = nodes.Count;
            var spacing = Mathf.Tau / count;
            for (var i = 0; i < count; i++) {
                var newX = Mathf.Sin(spacing * i + angle) * radius.x;
                var newY = Mathf.Cos(spacing * i + angle) * radius.y;
                var newPos = new Vector2(newX, newY);
                nodes[i].Position = newPos;
            }
        }

    }
}