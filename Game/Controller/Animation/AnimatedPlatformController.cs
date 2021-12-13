using System.Diagnostics;
using Godot;
using Tools;
using Tools.Animation;
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

            _player = new TweenPlayer("Platform").NewTween(this);

            Stopwatch x = Stopwatch.StartNew();
            TweenSequenceBuilder seq = TweenSequenceBuilder.CreateTemplate();

            seq.Callback(() => x = Stopwatch.StartNew());

            seq.AnimateSteps<Vector2>(this, nameof(follow), Easing.CubicInOut)
                .Offset(new Vector2(100, 0), 1, Easing.LinearInOut, (node) => LoggerFactory.GetLogger(typeof(AnimatedPlatformController)).Debug("Volviendo"))
                .Offset(new Vector2(-50, 0), 1)
                .EndAnimate();

            seq.Callback(() =>
                LoggerFactory.GetLogger(typeof(AnimatedPlatformController)).Debug("Llegó! esperamos 1..."));
            seq.Pause(1);

            seq.AnimateSteps<Color>(this, Property.Modulate)
                .To(new Color(1, 0, 0, 1f), 1, Easing.CubicInOut)
                .EndAnimate()
                .AnimateSteps<Color>(this, Property.Modulate).To(new Color(1, 1, 1, 1), 1, Easing.CubicInOut)
                .EndAnimate();

            seq.Callback(() => {
                x.Stop();
                GD.Print("Elapsed:" + x.ElapsedMilliseconds);
                // _player.SetTween(new Tween());
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