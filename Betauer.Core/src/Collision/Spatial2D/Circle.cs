using System.Linq;
using Godot;

namespace Betauer.Core.Collision.Spatial2D;

public class Circle : Shape {
    private Vector2 _position;
    private float _radius;

    public Vector2 Position {
        get => _position;
        set {
            SpatialGrid?.Move(this, value.X, value.Y);
            _position = value;
        }
    }

    public float Radius {
        get => _radius;
        set {
            SpatialGrid?.Resize(this, value);
            _radius = value;
        }
    }

    public bool TryResize(float newRadius) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetIntersectingShapesInCircle(Position.X, Position.Y, newRadius).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Resize(this, newRadius);
        }
        _radius = newRadius;
        return true;
    }

    public override bool TryMove(float x, float y) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetIntersectingShapesInCircle(x, y, Radius).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Move(this, x, y);
        }
        _position = new Vector2(x, y);
        return true;
    }

    public override float MinX => Position.X - Radius;
    public override float MaxX => Position.X + Radius;
    public override float MinY => Position.Y - Radius;
    public override float MaxY => Position.Y + Radius;

    public Circle() {
    }

    public Circle(float x, float y, float radius) : this(new Vector2(x, y), radius) {
    }

    public Circle(Vector2 position, float radius) {
        Position = position;
        Radius = radius;
    }

    public override bool IntersectCircle(float x, float y, float radius) {
        return Geometry.IntersectCircles(Position.X, Position.Y, Radius, x, y, radius);
    }

    public override bool IntersectRectangle(float x, float y, float width, float height) {
        return Geometry.IntersectCircleRectangle(Position.X, Position.Y, Radius, x, y, width, height);
    }

    public override bool IntersectPoint(float px, float py) {
        return Geometry.IsPointInsideCircle(px, py, Position.X, Position.Y, Radius);
    }

    public override (int, int)[] GetIntersectingCells(float cellSize) {
        return Geometry.GetCircleIntersectingCells(Position.X, Position.Y, Radius, cellSize);
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

    public bool Equals(Circle other) => Position.Equals(other.Position) && Radius == other.Radius;
}