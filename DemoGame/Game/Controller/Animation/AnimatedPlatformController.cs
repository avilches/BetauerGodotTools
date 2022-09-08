using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.DI;
using Betauer.Nodes.Property;
using Betauer.Signal;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class AnimatedPlatformController : KinematicBody2D {
        [Export] public bool IsFallingPlatform = false;
        [Inject] public PlatformManager PlatformManager { get; set;}
        private SceneTreeTween _sceneTreeTween;

        private Vector2 _original;
        public Vector2 Follow;

        public override void _Ready() {
            Configure();
        }

        public override void _PhysicsProcess(float delta) {
            UpdatePosition();
        }

        public async Task Configure() {
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);
            _original = Position;

            var x = Stopwatch.StartNew();
            var seq = SequenceAnimation
                .Create(this)
                .Callback(() => {
                    x = Stopwatch.StartNew();
                })
                .AnimateStepsBy<Vector2>(nameof(Follow), Easings.CubicInOut)
                .Offset(new Vector2(100, 0), 0.25f, Easings.Linear)
                .Offset(new Vector2(-100, 0), 0.25f)
                .EndAnimate()
                .AnimateSteps(Properties.Modulate)
                .To(new Color(1, 0, 0, 1f), 0.25f, Easings.CubicInOut)
                .EndAnimate()
                .AnimateSteps(Properties.Modulate).To(new Color(1, 1, 1, 1), 0.5f, Easings.CubicInOut)
                .EndAnimate()
                .SetLoops(1);

            var seq2 = SequenceAnimation
                .Create(this)
                .AnimateStepsBy<Vector2>(nameof(Follow), Easings.CubicInOut)
                .Offset(new Vector2(0, 50), 0.25f, Easings.Linear)
                .Offset(new Vector2(0, -50), 0.25f)
                .EndAnimate()
                .SetLoops(2);

            try {
                while (true) {
                    _sceneTreeTween = seq.Play(this);
                    await _sceneTreeTween.AwaitFinished().Timeout(GetTree(), 3);
                    _sceneTreeTween = seq2.Play(this);
                    await _sceneTreeTween.AwaitFinished().Timeout(GetTree(), 3);
                }
            } catch (TimeoutException e) {
                // Console.WriteLine(e);
            }
        }

        public void UpdatePosition() {
            Position = _original + Follow;
        }

        public void Start() {
            _sceneTreeTween.Play();
        }

        public void Pause() {
            Follow = Vector2.Zero;
            Modulate = new Color(1, 1, 1, 1);
            // TODO: this will trigger the timeout, resulting in a non-expected behaviour (the idea behind the timeout
            // TODO: is avoid a never-ending await when the scene is changed and the "finished" signal from the tween
            // TODO: is never sent
            _sceneTreeTween.Stop();
        }
    }
}