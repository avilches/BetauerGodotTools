using System.Linq;
using Godot;

namespace Betauer.Core.Collision.Spatial2D;

public class Point : Shape {
    private Vector2 _position;

    public Vector2 Position {
        get => _position;
        set {
            SpatialGrid?.Update(this, value.X, value.Y);
            _position = value;
        }
    }

    public override bool TryMove(float x, float y) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetIntersectingShapesInPoint(x, y).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Update(this, x, y);
        }
        _position = new Vector2(x, y);
        return true;
    }

    public override float MinX => Position.X;
    public override float MaxX => Position.X;
    public override float MinY => Position.Y;
    public override float MaxY => Position.Y;

    public Point() {
    }

    public Point(float x, float y) : this(new Vector2(x, y)) {
    }

    public Point(Vector2 position) {
        Position = position;
    }

    public override bool IntersectCircle(float x, float y, float radius) {
        return Geometry.IsPointInsideCircle(Position.X, Position.Y, x, y, radius);
    }

    public override bool IntersectRectangle(float x, float y, float width, float height) {
        return Geometry.IsPointInsideRectangle(Position.X, Position.Y, x, y, width, height);
    }

    public override bool IntersectPoint(float px, float py) {
        return px == _position.X && py == _position.Y;
    }

    public override (int, int)[] GetIntersectingCells(float cellSize) {
        var cell = Geometry.GetPointIntersectingCell(Position.X, Position.Y, cellSize);
        return new[] { cell };
    }

    public override Area2D CreateArea2D() {
        var area2D = new Area2D {
            Position = Position
        };
        area2D.AddChild(new CollisionShape2D {
            Shape = new CircleShape2D {
                Radius = 0.5f
            }
        });
        return area2D;
    }

    public bool Equals(Point other) => Position.Equals(other.Position);

    public bool Equals(Vector2 other) => Position.Equals(other);
}