using Godot;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.Application.Monitor;
using Betauer.DI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class AnimatedPlatformController1 : KinematicBody2D {
        [Export] public bool IsFallingPlatform = false;
        [Inject] public PlatformManager PlatformManager { get; set;}
        [Inject] public DebugOverlayManager DebugOverlayManager { get; set;}
        private SceneTreeTween _sceneTreeTween;
        private Vector2 _original;

        public override void _Ready() {
            // DebugOverlayManager.Overlay(this).GraphSpeed().SetChartSize(200, 50);
            
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);
            _original = Position;

            _sceneTreeTween = SequenceAnimation.Create()
                .AnimateStepsBy<Vector2>(UpdatePosition, Easings.CubicInOut)
                .Offset(new Vector2(0, 50), 1f, Easings.Linear)
                .Offset(new Vector2(0, -50), 1f)
                .EndAnimate()
                .Play(this)
                .SetLoops();
        }

        public void UpdatePosition(Vector2 pos) {
            Position = _original + pos;
        }

        public void Start() {
            if (_sceneTreeTween.IsValid()) _sceneTreeTween.Play();
        }

        public void Pause() {
            if (_sceneTreeTween.IsValid()) _sceneTreeTween.Pause();
        }
    }
}