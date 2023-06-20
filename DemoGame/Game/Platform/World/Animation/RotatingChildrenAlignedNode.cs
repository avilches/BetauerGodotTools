using System.Collections.Generic;
using System.Linq;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Application.Monitor;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.Platform.World.Animation; 

public partial class RotatingChildrenAlignedNode : Node2D {
    public const float CLOCK_THREE = Mathf.Pi / 2;
    public const float CLOCK_NINE = -Mathf.Pi / 2;

    [Export] public float Radius = 120;
    [Export] public float RotationDuration = 4.0f;
    [Inject] public DebugOverlayManager DebugOverlayManager { get; set;}

    private List<PhysicsBody2D> _platforms;
    private SequenceAnimation _sequence;
    private Tween _tween;
    private Tween _sceneTreeTween;

    private void RotateAligned(float angle) => RotateAligned(_platforms, angle, Radius);

    public override void _Ready() {
        _sequence = SequenceAnimation.Create(this)
            .AnimateSteps<float>(RotateAligned)
            .From(CLOCK_NINE).To(CLOCK_THREE, 1, Easings.QuadInOut)
            .EndAnimate()
            .AnimateSteps<float>(RotateAligned)
            .From(CLOCK_THREE).To(CLOCK_NINE, 1, Easings.QuadInOut)
            .EndAnimate()
            .SetInfiniteLoops();
        _sceneTreeTween = _sequence.Play();

        _platforms = this.GetChildren().OfType<PhysicsBody2D>().ToList();
        // var lastPlatform = _platforms.Last();
        // DebugOverlayManager.Overlay(lastPlatform).GraphSpeed().SetChartSize(200, 50);
    }

    public void Start() {
        _sceneTreeTween.Play();
    }

    public void Pause() {
        _sceneTreeTween.Pause();
    }

    /*
     * Alinea las plataformas como si fueran una aguja de un reloj y la gira. La primera primera plataforma
     * mantiene su posicion y las demás se van espaciando hasta llegar al radius
     */
    public static void RotateAligned(List<PhysicsBody2D> nodes, float angle, float radius,
        float initialOffset = 20) {
        var count = nodes.Count;
        var spacing = radius / count;
        for (var i = 0; i < count; i++) {
            float offset = ((spacing * i) + initialOffset);
            var newX = Mathf.Sin(angle) * offset;
            var newY = Mathf.Cos(angle) * offset;
            var newPos = new Vector2(newX, newY);
            nodes[i].Position = newPos;
        }
    }
}