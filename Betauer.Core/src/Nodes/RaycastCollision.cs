using Godot;
using Godot.Collections;

namespace Betauer.Core.Nodes;

public class RaycastCollision {
    public Dictionary Data { get; private set; }
    
    private GodotObject? _collider;
    private int? _colliderId;
    private Vector2? _normal;
    private Vector2? _position;
    private Rid? _rid;
    private int? _shape;

    public RaycastCollision(Dictionary? data) {
        if (data == null || data.Count == 0) {
            IsColliding = false;
        } else {
            Data = data;
            IsColliding = true;
        }
    }

    public RaycastCollision Recycle(Dictionary? data) {
        if (data == null || data.Count == 0) {
            IsColliding = false;
        } else {
            Data = data;
            IsColliding = true;
            _collider = null;
            _colliderId = null;
            _normal = null;
            _position = null;
            _rid = null;
            _shape = null;
        }
        return this;
    }
    
    public bool IsColliding { get; private set; }

    public GodotObject Collider {
        get {
            if (!IsColliding) return null;
            _collider ??= Data[ColliderString].AsGodotObject();
            return _collider;
        }
    }

    public T GetCollider<T>() where T : GodotObject => (T)Collider;

    public int ColliderId {
        get {
            if (!IsColliding) return -1;
            _colliderId ??= Data[ColliderIdString].AsInt32();
            return _colliderId.Value;
        }
    }

    public Vector2 Normal {
        get {
            if (!IsColliding) return Vector2.Zero;
            _normal ??= Data[NormalString].AsVector2();
            return _normal.Value;
        }
    }

    public Vector2 Position {
        get {
            if (!IsColliding) return Vector2.Zero;
            _position ??= Data[PositionString].AsVector2();
            return _position.Value;
        }
    }

    public Rid Rid {
        get {
            if (!IsColliding) return default;
            _rid ??= Data[RidString].AsRid();
            return _rid.Value;
        }
    }

    public int Shape {
        get {
            if (!IsColliding) return -1;
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