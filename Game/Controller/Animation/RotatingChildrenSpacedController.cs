using System.Collections.Generic;
using Godot;
using Tools;
using Tools.Effects;
using Veronenger.Game.Managers;
using static Tools.GodotTools;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenSpacedController : DiNode2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public Vector2 Radius = new Vector2(50, 50);
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager;


        private List<PhysicsBody2D> _platforms;
        private TweenSequence _sequence;

        public override void _EnterTree() {
            Configure();
        }

        private void Configure() {
            _sequence = new TweenSequence(true);
            _sequence.Add(0, Mathf.Tau, 4, ScaleFuncs.Linear);
            _sequence.AutoUpdate(this, delegate(float angle) {
                // var _speed = Tau / RotationDuration;
                // _angle = Wrap(_angle + _speed * delta, 0, Tau); // # Infinite rotation(in radians)
                RotateSpaced(_platforms, angle, Radius);
            });

            _platforms = PlatformManager.ConfigurePlatformList(GetChildren(), IsFallingPlatform, true);
        }
    }
}