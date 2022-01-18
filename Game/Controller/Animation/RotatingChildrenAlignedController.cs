using System.Collections.Generic;
using System.Linq;
using Godot;
using Betauer;
using Betauer.Animation;
using Veronenger.Game.Managers;
using static Betauer.GodotConstants;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenAlignedController : DiNode2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public float Radius = 50;
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager;

        private List<PhysicsBody2D> _platforms;
        private readonly SingleSequencePlayer _sequence = new SingleSequencePlayer();

        public override void Ready() {
            Configure();
        }

        private void RotateAligned(float angle) => AnimationTools.RotateAligned(_platforms, angle, Radius);

        private void Configure() {
            _sequence.CreateNewTween(this)
                .CreateSequence(this)
                .AnimateSteps<float>(RotateAligned)
                .From(CLOCK_NINE).To(CLOCK_THREE, 1, Easing.QuadInOut)
                .EndAnimate()
                .AnimateSteps<float>(RotateAligned)
                .From(CLOCK_THREE).To(CLOCK_NINE, 1, Easing.QuadInOut)
                .EndAnimate()
                .SetInfiniteLoops()
                .EndSequence()
                .Start();

            _platforms = this.GetChildrenFilter<PhysicsBody2D>();
            PlatformManager.ConfigurePlatform(_platforms.Last(), IsFallingPlatform, true);
        }

        public void Start() {
            _sequence.Start();
        }

        public void Pause() {
            _sequence.Stop();
        }

        protected override void Dispose(bool disposing) {
            _sequence?.Dispose();
            base.Dispose(disposing);
        }
    }
}