using System.Collections.Generic;
using Godot;
using Tools;
using Tools.Animation;
using Tools.Effects.Deprecated;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenSpacedController : DiNode2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public Vector2 Radius = new Vector2(50, 50);
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager;


        private List<PhysicsBody2D> _platforms;
        private TinyTweenSequence _sequence;

        public override void Ready() {
            Configure();
        }

        // var _speed = Tau / RotationDuration;
        // _angle = Wrap(_angle + _speed * delta, 0, Tau); // # Infinite rotation(in radians)

        private void Configure() {
            _sequence = new TinyTweenSequence(true);
            _sequence.Add(0, Mathf.Tau, 4, ScaleFuncs.Linear,
                delegate(float angle) {
                    AnimationTools.RotateSpaced(_platforms, angle, Radius);
                }
            );
            _sequence.AutoUpdate(this);

            _platforms = PlatformManager.ConfigurePlatformList(GetChildren(), IsFallingPlatform, true);
        }
    }
}