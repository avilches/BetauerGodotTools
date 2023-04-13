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
        var targetPosition = rayCast.GlobalPosition + rayCast.TargetPosition * rayCast.Scale;
        owner.DrawLine(rayCast.GlobalPosition, targetPosition, color, 2F);
        if (rayCast.IsColliding()) {
            targetPosition = rayCast.GetLocalCollisionPoint();
            owner.DrawLine(rayCast.GlobalPosition, rayCast.GlobalPosition + targetPosition, Colors.White);
        }
    }
}