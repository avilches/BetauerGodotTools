using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Collision.Spatial2D;

public class Circle : Shape {
    private Vector2 _position;
    private float _radius;

    public override float Width => Radius * 2f;
    public override float Height => Radius * 2f;
    public override float MinX => Position.X - Radius;
    public override float MaxX => Position.X + Radius;
    public override float MinY => Position.Y - Radius;
    public override float MaxY => Position.Y + Radius;
    public override Vector2 Center => Position;

    public sealed override Vector2 Position {
        get => _position;
        set {
            SpatialGrid?.Update(this, value.X, value.Y, _radius);
            _position = value;
        }
    }

    public float Radius {
        get => _radius;
        set {
            SpatialGrid?.Update(this, _position.X, _position.Y, value);
            _radius = value;
        }
    }
    
    public bool Update(float x, float y, float radius) {
        SpatialGrid?.Update(this, x, y, radius);
        _position = new Vector2(x, y);
        _radius = radius;
        return true;
    }

    public override bool TryMove(float x, float y) {
        return TryUpdate(x, y, _radius);
    }

    public bool TryResize(float radius) {
        return TryUpdate(_position.X, _position.Y, radius);
    }

    public bool TryUpdate(float x, float y, float radius) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetIntersectingShapesInCircle(x, y, radius).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Update(this, x, y, radius);
        }
        _position = new Vector2(x, y);
        _radius = radius;
        return true;
    }

    public Circle() {
    }

    public Circle(float x, float y, float radius) : this(new Vector2(x, y), radius) {
    }

    public Circle(Vector2 position, float radius) {
        Position = position;
        Radius = radius;
    }

    public override bool IntersectCircle(float x, float y, float radius) {
        return Geometry.Geometry.IntersectCircles(Position.X, Position.Y, Radius, x, y, radius);
    }

    public override bool IntersectRectangle(float x, float y, float width, float height) {
        return Geometry.Geometry.IntersectCircleRectangle(Position.X, Position.Y, Radius, x, y, width, height);
    }

    public override bool IntersectPoint(float px, float py) {
        return Geometry.Geometry.IsPointInCircle(px, py, Position.X, Position.Y, Radius);
    }

    public override (int, int)[] GetIntersectingCells(float cellSize) {
        return SpatialGrid.GetCircleIntersectingCells(Position.X, Position.Y, Radius, cellSize);
    }

    public override Area2D CreateArea2D() {
        var area2D = new Area2D {
            Position = Position
        };
        area2D.AddChild(new CollisionShape2D {
            Shape = new CircleShape2D {
                Radius = Radius
            }
        });
        return area2D;
    }

    public override bool Equals(object obj) => obj is Circle other && Equals(other);

    public bool Equals(Circle other) => Position.Equals(other.Position) && Radius == other.Radius;
}