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

        private void bla() {
            // GD.Print("bla");
        }

        public void Configure() {
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);

            _original = Position;

            TweenSequence seq = new TweenSequence(GetTree());
            seq.append( this, nameof(follow), new Vector2(100, 0), 2).set_trans(Tween.TransitionType.Cubic);
            seq.parallel().append( this, "modulate", new Color(1, 1, 1, 0), 2).set_trans(Tween.TransitionType.Cubic);
            seq.append_callback( this, nameof(bla), new Array());
            seq.append( this, nameof(follow), new Vector2(50, 0), 2).set_trans(Tween.TransitionType.Cubic);
            seq.parallel().append( this, "modulate", new Color(1, 1, 1, 1), 2).set_trans(Tween.TransitionType.Cubic);
            seq.set_loops();
        }

        public void UpdatePosition() {
            Position = _original + follow;
        }
    }
}