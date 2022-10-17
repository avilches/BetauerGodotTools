using System;
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
        public Vector2 Follow;

        public override void _Ready() {
            Configure();
        }

        public override void _Process(float delta) {
            UpdatePosition();
        }
        public float Speed { get; private set; } = 0f;
        public float MaxSpeed { get; private set; } = 0f;
        private Vector2 _prevPosition = Vector2.Zero;

        public override void _PhysicsProcess(float delta) {
            var position = Position * 60;
            Speed = (_prevPosition - position).Length();
            _prevPosition = position;
            MaxSpeed = Math.Max(MaxSpeed, Speed);
        }

        public void Configure() {
            DebugOverlayManager.Overlay(this).Text("Speed", () => $"{Speed:000} (max: {MaxSpeed:000})");
            DebugOverlayManager.Overlay(this).Graph("Speed", () => Speed);
            
            PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);
            _original = Position;

            _sceneTreeTween = SequenceAnimation.Create()
                .AnimateStepsBy<Vector2>(nameof(Follow), Easings.CubicInOut)
                .Offset(new Vector2(0, 50), 1f, Easings.Linear)
                .Offset(new Vector2(0, -50), 1f)
                .EndAnimate()
                .Play(this)
                .SetLoops();
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