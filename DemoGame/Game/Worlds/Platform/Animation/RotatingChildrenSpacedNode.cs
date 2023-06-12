using System.Collections.Generic;
using System.Linq;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform;

namespace Veronenger.Game.Worlds.Platform.Animation; 

public partial class RotatingChildrenSpacedNode : Node2D {
    [Export] public bool IsFallingPlatform = false;
    [Export] public Vector2 Radius = new Vector2(50, 50);
    [Export] public float RotationDuration = 4.0f;
    [Inject] public PlatformConfig PlatformConfig { get; set;}
    [Inject] public DebugOverlayManager DebugOverlayManager { get; set;}

    private List<PhysicsBody2D> _platforms;
    private SequenceAnimation _sequence;
    private Tween _sceneTreeTween;

    // var _speed = Tau / RotationDuration;
    // _angle = Wrap(_angle + _speed * delta, 0, Tau); // Infinite rotation(in radians
    private void RotateSpaced(float angle) => RotateSpaced(_platforms, angle, Radius);

    public override void _Ready() {
        _platforms = this.GetChildren().OfType<PhysicsBody2D>().ToList();
        PlatformConfig.ConfigurePlatformList(_platforms, IsFallingPlatform, true);
        _sequence = SequenceAnimation.Create(this)
            .SetProcessMode(Tween.TweenProcessMode.Physics)
            .AnimateSteps<float>(RotateSpaced)
            .From(0).To(Mathf.Tau, 4, Easings.Linear)
            .EndAnimate()
            .SetInfiniteLoops();
        _sceneTreeTween = _sequence.Play();
        // var lastPlatform = _platforms.Last();
        // DebugOverlayManager.Overlay(lastPlatform).GraphSpeed().SetChartSize(200, 50);
    }

    /*
    * Distribuye por el circulo de manera espaciado y las gira
    */
    public static void RotateSpaced(List<PhysicsBody2D> nodes, float angle, Vector2 radius) {
        var count = nodes.Count;
        var spacing = Mathf.Tau / count;
        for (var i = 0; i < count; i++) {
            var newX = Mathf.Sin(spacing * i + angle) * radius.X;
            var newY = Mathf.Cos(spacing * i + angle) * radius.Y;
            var newPos = new Vector2(newX, newY);
            nodes[i].Position = newPos;
        }
    }

}