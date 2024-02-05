using Betauer.Animation;
using Betauer.Application.Monitor;
using Betauer.Core.Easing;
using Betauer.Core.Nodes.Property;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.Platform.World.Animation; 

public partial class AnimatedPlatformController2 : CharacterBody2D {
    [Inject] public PlatformConfig PlatformConfig { get; set;}
    [Inject] public DebugOverlayManager DebugOverlayManager { get; set;}
    private Tween _sceneTreeTween;
    private Vector2 _original;

    public override void _Ready() {
        // DebugOverlayManager.Overlay(this).GraphSpeed().SetChartSize(200, 50);
            
        _original = Position;

        _sceneTreeTween = SequenceAnimation.Create()
            .AnimateStepsBy<Vector2>(UpdatePosition, Interpolation.CubicInOut)
            .Offset(new Vector2(250, 0), 2.5f)
            .Offset(new Vector2(-250, 0), 2.5f)
            .EndAnimate()
            .Parallel()
            .AnimateSteps(Properties.Modulate)
            .To(new Color(1, 0, 0, 1f), 0.25f, Interpolation.CubicInOut)
            .EndAnimate()
            .AnimateSteps(Properties.Modulate).To(new Color(1, 1, 1, 1), 0.5f, Interpolation.CubicInOut)
            .EndAnimate()
            .Play(this)
            .SetLoops();
    }

    public void UpdatePosition(Vector2 pos) {
        Position = _original + pos;
    }
}