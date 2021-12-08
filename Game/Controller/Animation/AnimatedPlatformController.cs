using System.Diagnostics;
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
        private TweenPlayer _player;

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

            _player = new TweenPlayer(this);

            Stopwatch x = Stopwatch.StartNew();
            TweenSequence seq = new TweenSequence();

            seq.Callback(() => x = Stopwatch.StartNew());

            seq.AnimateVector2(this, nameof(follow), Tween.TransitionType.Cubic)
                .AddOffset(new Vector2(100, 0), 1)
                .AddOffset(new Vector2(-50, 0), 1,
                    () => LoggerFactory.GetLogger(typeof(AnimatedPlatformController)).Debug("Volviendo"))
                .End();
            // seq.Callback(() =>
                // LoggerFactory.GetLogger(typeof(AnimatedPlatformController)).Debug("Llegó! esperamos 1..."));
            // seq.Pause(3).Callback(() => LoggerFactory.GetLogger(typeof(AnimatedPlatformController)).Debug("Ya"));
            seq.Parallel().AnimateColor(this, "modulate")
                .To(new Color(1, 0, 0, 1f), 1, Tween.TransitionType.Cubic);
            seq.AnimateColor(this, "modulate").To(new Color(1, 1, 1, 1), 1, Tween.TransitionType.Cubic);
            seq.Callback(() => {
                x.Stop();
                GD.Print("Elapsed:" + x.ElapsedMilliseconds);
            });

            _player.AddSequence(seq);
            _player.SetInfiniteLoops();
        }

        public void UpdatePosition() {
            Position = _original + follow;
        }

        public void Start() {
            _player.Start();
        }

        public void Pause() {
            follow = Vector2.Zero;
            Modulate = new Color(1, 1, 1, 1);
            _player.Reset();
        }
    }
}