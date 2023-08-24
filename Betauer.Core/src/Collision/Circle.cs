using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Collision;

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
            if (SpatialGrid.GetCircleIntersectingShapes(Position.X, Position.Y, newRadius).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Resize(this, newRadius);
        }
        _radius = newRadius;
        return true;
    }

    public bool TryMove(float x, float y) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetCircleIntersectingShapes(x, y, Radius).Any(shape => shape != this)) {
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

    public override bool Intersect<T>(T other) {
        if (other is Rectangle otherRectangle) return Geometry.Intersect(this, otherRectangle);
        if (other is Circle otherCircle) return Geometry.Intersect(this, otherCircle);
        return false;
    }

    public override bool Intersect(Circle other) {
        return Geometry.Intersect(this, other);
    }

    public override bool Intersect(Rectangle other) {
        return Geometry.Intersect(this, other);
    }

    public override bool IntersectCircle(float x, float y, float radius) {
        return Geometry.IntersectCircles(Position.X, Position.Y, Radius, x, y, radius);
    }

    public override bool IntersectRectangle(float x, float y, float width, float height) {
        return Geometry.IntersectCircleRectangle(Position.X, Position.Y, Radius, x, y, width, height);
    }

    public override bool IsPointInside(Vector2 point) {
        return Geometry.IsPointInsideCircle(point, this);
    }

    public override bool IsPointInside(float px, float py) {
        return Geometry.IsPointInsideCircle(px, py, this);
    }

    public override IEnumerable<(int, int)> GetIntersectingCells(int cellSize) {
        return GetIntersectingCells(Position.X, Position.Y, Radius, cellSize);
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

    public static IEnumerable<(int, int)> GetIntersectingCells(float cx, float cy, float radius, int cellSize) {
        var minCellXf = (cx - radius) / cellSize;
        var maxCellXf = (cx + radius) / cellSize;
        var minCellYf = (cy - radius) / cellSize;
        var maxCellYf = (cy + radius) / cellSize;

        var minCellX = cx < 0 ? (int)Mathf.Floor(minCellXf) : (int)minCellXf;
        var maxCellX = cx < 0 ? (int)Mathf.Floor(maxCellXf) : (int)maxCellXf;
        var minCellY = cy < 0 ? (int)Mathf.Floor(minCellYf) : (int)minCellYf;
        var maxCellY = cy < 0 ? (int)Mathf.Floor(maxCellYf) : (int)maxCellYf;

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                yield return (x, y);
            }
        }
    }

    public bool Equals(Circle other) => Position.Equals(other.Position) && Radius == other.Radius;
}