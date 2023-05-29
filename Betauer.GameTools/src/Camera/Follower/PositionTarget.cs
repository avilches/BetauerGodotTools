using System;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Camera.Follower;

public class PositionTarget : Target {
    private readonly Func<Vector2> _getPosition;
    public override Vector2 GetPosition() => _getPosition();
    
    public PositionTarget(Func<Vector2> getPosition, float time, IEasing? easingPosition, Vector2? zoom = null, IEasing? easingZoom = null) : base(zoom, easingPosition) {
        _getPosition = getPosition;
        Time = time;
        EasingPosition = easingPosition ?? Easings.Linear;
    }
}