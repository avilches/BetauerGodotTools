using System;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Bus.Signal;

public class Area2DInArea2DCollision {
    private readonly int _layer;

    public Area2DInArea2DCollision(int layer) {
        _layer = layer;
    }

    public void OnArea2DEnteredIn(Area2D area2D, Action<Area2D> onEnter) {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnAreaEntered(onEnter);
    }

    public void OnOtherArea2DExitedIn(Area2D area2D, Action<Area2D> onEnter) {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnAreaExited(onEnter);
    }

    public void Detect(Area2D area2D) {
        area2D.SetCollisionLayerValue(_layer, true);
    }
}