using Godot;

namespace Betauer.Core.Nodes;

public static class RayCast2DExtensions {
    public static Vector2 GetLocalCollisionPoint(this RayCast2D rayCast) {
        var rayCastGlobalPosition = rayCast.GlobalPosition;
        var globalCollisionPoint = rayCast.GetCollisionPoint();
        return rayCastGlobalPosition.DirectionTo(globalCollisionPoint) *
               rayCastGlobalPosition.DistanceTo(globalCollisionPoint);
    }
}