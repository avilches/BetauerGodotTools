using Godot;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Nodes.Property;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Animation {
    public class AnimatedPlatformController2 : KinematicBody2D {
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
                .Offset(new Vector2(250, 0), 2.5f)
                .Offset(new Vector2(-250, 0), 2.5f)
                .EndAnimate()
                .Parallel()
                .AnimateSteps(Properties.Modulate)
                .To(new Color(1, 0, 0, 1f), 0.25f, Easings.CubicInOut)
                .EndAnimate()
                .AnimateSteps(Properties.Modulate).To(new Color(1, 1, 1, 1), 0.5f, Easings.CubicInOut)
                .EndAnimate()
                .Play(this)
                .SetLoops();
        }

        public void UpdatePosition(Vector2 pos) {
            Position = _original + pos;
        }
    }
}