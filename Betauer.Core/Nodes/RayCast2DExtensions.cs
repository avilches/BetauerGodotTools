using Godot;

namespace Betauer.Core.Nodes;

public static class RayCast2DExtensions {
    public static Vector2 GetLocalCollisionPoint(this RayCast2D rayCast) {
        var rayCastGlobalPosition = rayCast.GlobalPosition;
        var globalCollisionPoint = rayCast.GetCollisionPoint();
        return rayCastGlobalPosition.DirectionTo(globalCollisionPoint) *
               rayCastGlobalPosition.DistanceTo(globalCollisionPoint);
    }
    public static void DrawRaycast(this RayCast2D rayCast, CanvasItem owner, Color color) {
        var targetPosition = (rayCast.Position + rayCast.TargetPosition) * rayCast.Scale;
        owner.DrawLine(rayCast.Position, targetPosition, color, 3F);
        if (rayCast.IsColliding()) {
            targetPosition = rayCast.GetLocalCollisionPoint();
            owner.DrawLine(rayCast.Position, rayCast.Position + targetPosition, Colors.White, 1F);
        }
    }

}