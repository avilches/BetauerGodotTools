using System.Collections.Generic;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenSpacedController : Node2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public Vector2 Radius = new Vector2(50, 50);
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager;


        private List<PhysicsBody2D> _platforms;
        private readonly SingleSequencePlayer _sequence = new SingleSequencePlayer();

        public override void _Ready() {
            Configure();
        }

        // var _speed = Tau / RotationDuration;
        // _angle = Wrap(_angle + _speed * delta, 0, Tau); // Infinite rotation(in radians
        private void RotateSpaced(float angle) => AnimationTools.RotateSpaced(_platforms, angle, Radius);

        private void Configure() {
            _platforms = this.GetChildren<PhysicsBody2D>();
            PlatformManager.ConfigurePlatformList(_platforms, IsFallingPlatform, true);
            _sequence.WithParent(this)
                .CreateSequence(this)
                .AnimateSteps<float>(RotateSpaced)
                .From(0).To(Mathf.Tau, 4, Easing.LinearInOut)
                .EndAnimate()
                .SetInfiniteLoops()
                .EndSequence()
                .Play();
        }
    }
}