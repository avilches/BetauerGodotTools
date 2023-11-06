using Godot;

namespace Betauer.Core.Nodes;

public static class RayCast2DExtensions {
    public static Vector2 GetLocalCollisionPoint(this RayCast2D rayCast) {
        var rayCastGlobalPosition = rayCast.GlobalPosition;
        var globalCollisionPoint = rayCast.GetCollisionPoint();
        return rayCastGlobalPosition.DirectionTo(globalCollisionPoint) *
               rayCastGlobalPosition.DistanceTo(globalCollisionPoint);
    }

    public static void DrawRaycast(this CanvasItem owner, RayCast2D rayCast, Color color) {
        var start = owner.ToLocal(rayCast.GlobalPosition);
        var targetPosition = start + rayCast.TargetPosition * rayCast.Scale;
        owner.DrawLine(start, targetPosition, color, 2F);
        if (rayCast.IsColliding()) {
            targetPosition = rayCast.GetLocalCollisionPoint();
            owner.DrawLine(start, start + targetPosition, Colors.White);
        }
    }
}