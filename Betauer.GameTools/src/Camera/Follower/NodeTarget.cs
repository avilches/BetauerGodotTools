using Betauer.Animation.Easing;
using Godot;

namespace Betauer.Camera.Follower;

public class NodeTarget : Target {
    public Node2D Node { get; }
    public override Vector2 GetPosition() => Node.GlobalPosition;
    
    public NodeTarget(Node2D node, float time, IEasing? easingPosition, Vector2? zoomValue = null, IEasing? easingZoom = null) : base(zoomValue, easingPosition) {
        Node = node;
        Time = time;
        EasingPosition = easingPosition ?? Easings.Linear;
    }
}