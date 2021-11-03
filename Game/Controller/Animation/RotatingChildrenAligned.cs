using System.Collections.Generic;
using System.Linq;
using Godot;
using Veronenger.Game.Tools;
using Veronenger.Game.Tools.Effects;
using static Veronenger.Game.Tools.GodotConstants;
using static Veronenger.Game.Tools.GodotTools;

namespace Veronenger.Game.Controller.Animation {
    public class RotatingChildrenAligned : Node2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public float Radius = 50;
        [Export] public float RotationDuration = 4.0f;

        private List<PhysicsBody2D> _platforms;
        private TweenSequence _sequence;

        public override void _EnterTree() {
            Configure();
        }

        public override void _PhysicsProcess(float delta) {
            // var _speed = Tau / RotationDuration;
            // _angle = Wrap(_angle + _speed * delta, 0, Tau); // # Infinite rotation(in radians)
            var angle = _sequence.Update(delta);
            RotateAligned(_platforms, angle, Radius);
        }

        private void Configure() {
            _sequence = new TweenSequence(true);
            _sequence.Add(CLOCK_NINE, CLOCK_THREE, 1, ScaleFuncs.QuadraticEaseInOut);
            _sequence.AddReverseAll();

            _platforms = Filter<PhysicsBody2D>(GetChildren());
            GameManager.Instance.PlatformManager.RegisterPlatform(_platforms.Last(), IsFallingPlatform, true);
        }

        public void Start() {
            _sequence.Start();
        }

        public void Pause() {
            _sequence.Pause();
        }
    }
}