using System.Diagnostics;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class AnimatedPlatformController : KinematicBody2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public bool Enabled = true;
        [Inject] public PlatformManager PlatformManager;


        private Vector2 _original;
        public Vector2 follow;
        private MultipleSequencePlayer _sequencePlayer;

        public override void _Ready() {
            Configure();
        }

        public override void _PhysicsProcess(float delta) {
            if (!Enabled) return;
            UpdatePosition();
        }

        public void Configure() {
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);

            _original = Position;

            _sequencePlayer = new MultipleSequencePlayer().WithParent(this);

            Stopwatch x = Stopwatch.StartNew();
            SequenceBuilder seq = SequenceBuilder.Create();

            seq.Callback(() => x = Stopwatch.StartNew());

            seq.AnimateStepsBy(this, new IndexedProperty<Vector2>(nameof(follow)), Easing.CubicInOut)
                .Offset(new Vector2(100, 0), 1, Easing.LinearInOut)
                .Offset(new Vector2(-50, 0), 1)
                .EndAnimate();

            seq.Callback(() =>
                LoggerFactory.GetLogger(typeof(AnimatedPlatformController)).Debug("Llegó! esperamos 1..."));
            seq.Pause(1);

            seq.AnimateSteps(this, Property.Modulate)
                .To(new Color(1, 0, 0, 1f), 1, Easing.CubicInOut)
                .EndAnimate()
                .AnimateSteps(this, Property.Modulate).To(new Color(1, 1, 1, 1), 1, Easing.CubicInOut)
                .EndAnimate();

            _sequencePlayer.AddSequence(seq);
            _sequencePlayer.SetInfiniteLoops();
        }

        public void UpdatePosition() {
            Position = _original + follow;
        }

        public void Start() {
            _sequencePlayer.Play();
        }

        public void Pause() {
            follow = Vector2.Zero;
            Modulate = new Color(1, 1, 1, 1);
            _sequencePlayer.Reset();
        }
    }
}