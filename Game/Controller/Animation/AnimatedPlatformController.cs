using Godot;
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
        private TweenSequence seqMove;

        public override void Ready() {
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

            seqMove = new TweenSequence(this);
            seqMove.AddOffset(this, nameof(follow), new Vector2(100, 0), 2).SetTrans(Tween.TransitionType.Cubic);
            // seqMove.Parallel().AddMethod(delegate(Vector2 value) { GD.Print(value); }, Vector2.Down, Vector2.Up, 0.3f);
            seqMove.Parallel().Add(this, "modulate", new Color(1, 1, 1, 0), 2)
                .SetTrans(Tween.TransitionType.Cubic);
            // seqMove.AddCallback(this, nameof(bla), new Array());
            // seqMove.AddCallback(delegate { GD.Print("callback"); });
            seqMove.AddOffset(this, nameof(follow), new Vector2(-50, 0), 2).SetTrans(Tween.TransitionType.Cubic);
            seqMove.Parallel().Add(this, "modulate", new Color(1, 1, 1, 1), 2).SetTrans(Tween.TransitionType.Cubic);
            seqMove.SetLoops();
        }

        public void UpdatePosition() {
            Position = _original + follow;
        }

        public void Start() {
            seqMove.Start();
        }

        public void Pause() {
            follow = Vector2.Zero;
            Modulate = new Color(1, 1, 1, 1);
            seqMove.Reset();
        }

    }
}