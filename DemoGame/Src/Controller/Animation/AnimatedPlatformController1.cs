using Godot;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Nodes;
using Veronenger.Character;
using Veronenger.Managers;

namespace Veronenger.Controller.Animation {
    public class AnimatedPlatformController1 : CharacterBody2D {
        [Export] public bool IsFallingPlatform = false;
        [Inject] public PlatformManager PlatformManager { get; set;}
        [Inject] public DebugOverlayManager DebugOverlayManager { get; set;}
        [Inject] public KinematicPlatformMotion PlatformBody { get; set; }
        private Tween _sceneTreeTween;
        private Vector2 _original;

        public override void _Ready() {
            // DebugOverlayManager.Overlay(this).GraphSpeed().SetChartSize(200, 50);
            
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);
            _original = Position;
            Motion__syncToPhysics = false;

            // tween the _newPosition property and use it like this:
            // this.OnPhysicsProcess((delta) => {
                // var speed = _newPosition - Position;
                // MoveAndCollide(speed);
            // });

            _sceneTreeTween = SequenceAnimation.Create()
                .AnimateStepsBy<Vector2>((newPosition) => Position = _original + newPosition, Easings.CubicInOut)
                .Offset(new Vector2(0, -300), 2.5f)
                .Offset(new Vector2(0, 300), 2.5f)
                .EndAnimate()
                .Play(this)
                .SetLoops();
        }

        public void Start() {
            if (_sceneTreeTween.IsValid()) _sceneTreeTween.Play();
        }

        public void Pause() {
            if (_sceneTreeTween.IsValid()) _sceneTreeTween.Pause();
        }
    }
}