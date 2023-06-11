using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.DI.Attributes;
using Betauer.Physics;
using Godot;
using Veronenger.Game;
using Veronenger.Game.Config;

namespace Veronenger.Game.Worlds.Animation; 

public partial class AnimatedPlatformController1 : StaticBody2D {
    [Inject] public PlatformConfig PlatformConfig { get; set;}
    [Inject] public DebugOverlayManager DebugOverlayManager { get; set;}
    [Inject] public KinematicPlatformMotion PlatformBody { get; set; }
    private Tween _sceneTreeTween;
    private Vector2 _original;

    public override void _Ready() {
        // DebugOverlayManager.Overlay(this).GraphSpeed().SetChartSize(200, 50);
            
        _original = Position;
        // TODO Godot 4
        // Motion__syncToPhysics = false;

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