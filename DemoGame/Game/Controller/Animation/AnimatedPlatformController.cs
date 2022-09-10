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

            _sceneTreeTween = SequenceAnimation.Create()
                .Add(SequenceAnimation
                    .Create()
                    .AnimateStepsBy<Vector2>(nameof(Follow), Easings.CubicInOut)
                    .Offset(new Vector2(100, 0), 0.25f, Easings.Linear)
                    .Offset(new Vector2(-100, 0), 0.25f)
                    .EndAnimate()
                    .Parallel()
                    .AnimateSteps(Properties.Modulate)
                    .To(new Color(1, 0, 0, 1f), 0.25f, Easings.CubicInOut)
                    .EndAnimate()
                    .AnimateSteps(Properties.Modulate).To(new Color(1, 1, 1, 1), 0.5f, Easings.CubicInOut)
                    .EndAnimate()
                    .SetLoops(2))
                .Add(SequenceAnimation
                    .Create()
                    .AnimateStepsBy<Vector2>(nameof(Follow), Easings.CubicInOut)
                    .Offset(new Vector2(0, 50), 0.25f, Easings.Linear)
                    .Offset(new Vector2(0, -50), 0.25f)
                    .EndAnimate()
                    .SetLoops(3))
                .Play(this)
                .SetLoops(3);
        }

        public void UpdatePosition() {
            Position = _original + Follow;
        }

        public void Start() {
            if (_sceneTreeTween.IsValid()) _sceneTreeTween.Play();
        }

        public void Pause() {
            if (_sceneTreeTween.IsValid()) _sceneTreeTween.Pause();
        }
    }
}