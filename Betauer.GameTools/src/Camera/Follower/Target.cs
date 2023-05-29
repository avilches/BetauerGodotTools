using System;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Camera.Follower;

public class ZoomTarget {
    public IEasing? EasingZoom { get; protected set;}
    public Vector2? ZoomValue { get; protected set; }
    public float Time { get; protected set; }

    public ZoomTarget(Vector2? zoomValue, IEasing? easingZoom = null) {
        ZoomValue = zoomValue;
        EasingZoom = easingZoom ?? Easings.Linear;
    }
}

public abstract class Target : ZoomTarget {
    public abstract Vector2 GetPosition();
    public IEasing? EasingPosition { get; protected set;}

    protected Target(Vector2? zoomValue, IEasing? easingZoom = null) : base(zoomValue, easingZoom) {
    }

    public static NodeTarget Create(Node2D node, float time = 0.5f, IEasing? easingPosition = null) {
        return new NodeTarget(node, time, easingPosition);
    }
    
    public static Target Create(Func<Vector2> func, float time = 0.5f, IEasing? easingPosition = null) {
        return new PositionTarget(func, time, easingPosition);
    }

    public Target Zoom(Vector2 zoom, IEasing? easingZoom = null) {
        ZoomValue = zoom;
        EasingZoom = easingZoom ?? Easings.Linear;
        return this;
    }

    public Target Zoom(float v, IEasing? easingZoom = null) {
        ZoomValue = new Vector2(v, v);
        EasingZoom = easingZoom ?? Easings.Linear;
        return this;
    }
}
