using System.Collections.Generic;
using System.Linq;
using Godot;
using Tools;
using Tools.Animation;
using Tools.Effects.Deprecated;
using Veronenger.Game.Managers;
using static Tools.GodotConstants;
using static Tools.GodotTools;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenAlignedController : DiNode2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public float Radius = 50;
        [Export] public float RotationDuration = 4.0f;
        [Inject] public PlatformManager PlatformManager;


        private List<PhysicsBody2D> _platforms;
        private TinyTweenSequence _sequence;

        public override void Ready() {
            Configure();
        }

        private void Configure() {
            _sequence = new TinyTweenSequence(true);
            _sequence.Add(CLOCK_NINE, CLOCK_THREE, 1, ScaleFuncs.QuadraticEaseInOut);
            _sequence.AddReverseAll();
            _sequence.AutoUpdate(this, delegate(float angle) {
                // var _speed = Tau / RotationDuration;
                // _angle = Wrap(_angle + _speed * delta, 0, Tau); // # Infinite rotation(in radians)
                AnimationTools.RotateAligned(_platforms, angle, Radius);
            });

            _platforms = this.GetChildrenFilter<PhysicsBody2D>();
            PlatformManager.ConfigurePlatform(_platforms.Last(), IsFallingPlatform, true);
        }

        public void Start() {
            _sequence.Start();
        }

        public void Pause() {
            _sequence.Pause();
        }
    }
}