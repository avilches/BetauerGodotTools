using Godot;
using Godot.Collections;
using Tools;
using Tools.Effects;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class AnimatedPlatformController : DiKinematicBody2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public bool Enabled = true;
        [Inject] public PlatformManager PlatformManager;


        private Vector2 _original;
        public Vector2 follow;

        public override void _EnterTree() {
            Configure();
        }

        public override void _PhysicsProcess(float delta) {
            if (!Enabled) return;
            UpdatePosition();
        }

        // private void bla() {
            // GD.Print("bla");
        // }

        public void Configure() {
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);

            _original = Position;

            TweenSequence seq = new TweenSequence(this);
            seq.AddOffset(this, nameof(follow), new Vector2(100, 0), 2).SetTrans(Tween.TransitionType.Cubic);
            seq.Parallel().Add(this, "modulate", new Color(1, 1, 1, 0), 2)
                .SetTrans(Tween.TransitionType.Cubic);
            // seq.AddCallback(this, nameof(bla), new Array());
            // seq.AddCallback(delegate { GD.Print("callback"); });
            // seq.AddMethod(delegate(Vector2 value) { GD.Print(value); }, Vector2.Down, Vector2.Up, 1);
            seq.AddOffset(this, nameof(follow), new Vector2(-50, 0), 2).SetTrans(Tween.TransitionType.Cubic);
            seq.Parallel().Add(this, "modulate", new Color(1, 1, 1, 1), 2).SetTrans(Tween.TransitionType.Cubic);
            seq.SetLoops().Start();
        }

        public void UpdatePosition() {
            Position = _original + follow;
        }
    }
}