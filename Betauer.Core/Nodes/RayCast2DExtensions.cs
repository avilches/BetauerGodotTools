using System;
using Godot;
using Godot.Collections;

namespace Betauer.Core.Nodes;

public class RaycastCollision {
    public readonly Dictionary Data;
    
    public RaycastCollision(Dictionary data) {
        Data = data;
    }

    private bool? _collision;
    private GodotObject? _collider;
    private int? _colliderId;
    private Vector2? _normal;
    private Vector2? _position;
    private Rid? _rid;
    private int? _shape;

    public bool Collision {
        get {
            _collision ??= Data.Count > 0;
            return _collision.Value;
        }
    }

    public GodotObject Collider {
        get {
            _collider ??= Data[ColliderString].AsGodotObject();
            return _collider;
        }
    }

    public T GetCollider<T>() where T : GodotObject => (T)Collider;

    public int ColliderId {
        get {
            _colliderId ??= Data[ColliderIdString].AsInt32();
            return _colliderId.Value;
        }
    }

    public Vector2 Normal {
        get {
            _normal ??= Data[NormalString].AsVector2();
            return _normal.Value;
        }
    }

    public Vector2 Position {
        get {
            _position ??= Data[PositionString].AsVector2();
            return _position.Value;
        }
    }

    public Rid Rid {
        get {
            _rid ??= Data[RidString].AsRid();
            return _rid.Value;
        }
    }

    public int Shape {
        get {
            _shape ??= Data[ShapeString].AsInt32();
            return _shape.Value;
        }
    }

    private static readonly StringName ColliderString = "collider";
    private static readonly StringName ColliderIdString = "collider_id";
    private static readonly StringName NormalString = "normal";
    private static readonly StringName PositionString = "position";
    private static readonly StringName RidString = "rid";
    private static readonly StringName ShapeString = "shape";

}


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
    
    public static RaycastCollision RaycastTo(this CanvasItem owner, Vector2 from, Vector2 to, Action<PhysicsRayQueryParameters2D>? config = null) {
        var query = PhysicsRayQueryParameters2D.Create(from, to);
        config?.Invoke(query);
        return new RaycastCollision(owner.GetWorld2D().DirectSpaceState.IntersectRay(query));
    }

    public static RaycastCollision RaycastTo(this Node2D from, Vector2 to, Action<PhysicsRayQueryParameters2D>? config = null) {
        var query = PhysicsRayQueryParameters2D.Create(from.GlobalPosition, to);
        config?.Invoke(query);
        return new RaycastCollision(from.GetWorld2D().DirectSpaceState.IntersectRay(query));
    }


}