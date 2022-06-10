using System.Collections.Generic;
using System.Linq;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenAlignedController : Node2D {
        public const float CLOCK_THREE = Mathf.Pi / 2;
        public const float CLOCK_NINE = -Mathf.Pi / 2;

        [Export] public bool IsFallingPlatform = false;
        [Export] public float Radius = 50;
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager;

        private List<PhysicsBody2D> _platforms;
        private readonly SingleSequencePlayer _sequence = new SingleSequencePlayer();

        public override void _Ready() {
            Configure();
        }

        private void RotateAligned(float angle) => AnimationTools.RotateAligned(_platforms, angle, Radius);

        private void Configure() {
            _sequence.WithParent(this)
                .CreateSequence(this)
                .AnimateSteps<float>(RotateAligned)
                .From(CLOCK_NINE).To(CLOCK_THREE, 1, Easing.QuadInOut)
                .EndAnimate()
                .AnimateSteps<float>(RotateAligned)
                .From(CLOCK_THREE).To(CLOCK_NINE, 1, Easing.QuadInOut)
                .EndAnimate()
                .SetInfiniteLoops()
                .EndSequence()
                .Play();

            _platforms = this.GetChildren<PhysicsBody2D>();
            PlatformManager.ConfigurePlatform(_platforms.Last(), IsFallingPlatform, true);
        }

        public void Start() {
            _sequence.Play();
        }

        public void Pause() {
            _sequence.Stop();
        }
    }
}