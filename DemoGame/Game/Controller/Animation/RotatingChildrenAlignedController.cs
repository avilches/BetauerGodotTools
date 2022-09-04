using System.Collections.Generic;
using System.Linq;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.DI;
using Betauer.Nodes;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenAlignedController : Node2D {
        public const float CLOCK_THREE = Mathf.Pi / 2;
        public const float CLOCK_NINE = -Mathf.Pi / 2;

        [Export] public bool IsFallingPlatform = false;
        [Export] public float Radius = 50;
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager { get; set;}

        private List<PhysicsBody2D> _platforms;
        private Sequence _sequence;
        private SceneTreeTween _tween;

        public override void _Ready() {
            Configure();
        }

        private void RotateAligned(float angle) => RotateAligned(_platforms, angle, Radius);

        private SceneTreeTween _sceneTreeTween;
        private void Configure() {
            _sequence = Sequence.Create(this)
                .AnimateSteps<float>(RotateAligned)
                .From(CLOCK_NINE).To(CLOCK_THREE, 1, Easings.QuadInOut)
                .EndAnimate()
                .AnimateSteps<float>(RotateAligned)
                .From(CLOCK_THREE).To(CLOCK_NINE, 1, Easings.QuadInOut)
                .EndAnimate()
                .SetInfiniteLoops();
            _sceneTreeTween = _sequence.PlayForever();

            _platforms = this.GetChildren<PhysicsBody2D>();
            PlatformManager.ConfigurePlatform(_platforms.Last(), IsFallingPlatform, true);
        }

        public void Start() {
            _sceneTreeTween.Play();
        }

        public void Pause() {
            _sceneTreeTween.Pause();
        }

        /*
         * Alinea las plataformas como si fueran una aguja de un reloj y la gira. La primera primera plataforma
         * mantiene su posicion y las demás se van espaciando hasta llegar al radius
         */
        public static void RotateAligned(List<PhysicsBody2D> nodes, float angle, float radius,
            float initialOffset = 20) {
            var count = nodes.Count;
            var spacing = radius / count;
            for (var i = 0; i < count; i++) {
                float offset = ((spacing * i) + initialOffset);
                var newX = Mathf.Sin(angle) * offset;
                var newY = Mathf.Cos(angle) * offset;
                var newPos = new Vector2(newX, newY);
                nodes[i].Position = newPos;
            }
        }
    }
}