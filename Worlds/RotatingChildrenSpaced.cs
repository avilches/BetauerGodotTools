using System.Collections.Generic;
using Game.Tools;
using static Game.Tools.GodotConstants;
using static Game.Tools.GodotTools;
using Game.Tools.Effects;
using Godot;

namespace Game.Worlds {
    public class RotatingChildrenSpaced : Node2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public Vector2 Radius = new Vector2(50, 50);
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
            RotateSpaced(_platforms, angle, Radius);
        }

        private void Configure() {
            _sequence = new TweenSequence(true);
            _sequence.Add(0, Mathf.Tau, 4, ScaleFuncs.Linear);

            _platforms = GameManager.Instance.PlatformManager.RegisterPlatforms(GetChildren(), IsFallingPlatform, true);
        }
    }
}