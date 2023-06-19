using System;
using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Camera.Control;

public abstract class CameraTransition {
    public float Time { get; set; }
    public IEasing? Easing { get; set; }
}

public class PositionCameraTransition : CameraTransition {
    public Vector2 Position { get; }

    public PositionCameraTransition(Vector2 position, float time, IEasing? easingPosition = null) {
        Position = position;
        Time = time;
        Easing = easingPosition ?? Easings.Linear;
    }
}

public class GetPositionCameraTransition : CameraTransition {
    public Func<Vector2> GetPosition { get; set; }

    public GetPositionCameraTransition(Func<Vector2> getPosition, float time, IEasing? easingPosition = null) {
        GetPosition = getPosition;
        Time = time;
        Easing = easingPosition ?? Easings.Linear;
    }
}

public class ZoomCameraTransition : CameraTransition {
    public Vector2 Zoom { get; set; }
    public Func<Vector2>? GetZoomPoint { get; set; }

    public ZoomCameraTransition(Vector2 zoom, float time, IEasing? easingZoom = null, Func<Vector2>? getZoomPoint = null) {
        Zoom = zoom;
        Time = time;
        Easing = easingZoom ?? Easings.Linear;
        GetZoomPoint = getZoomPoint;
    }
}