using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

public class Circle : IShape {
    public Vector2 Position { get; init; }
    public float Radius { get; init; }

    public float MinX => Position.X - Radius;
    public float MaxX => Position.X + Radius;
    public float MinY => Position.Y - Radius;
    public float MaxY => Position.Y + Radius;

    public Circle() {
    }

    public Circle(float x, float y, float radius) : this(new Vector2(x, y), radius) {
    }

    public Circle(Vector2 position, float radius) {
        Position = position;
        Radius = radius;
    }

    public bool Overlaps<T>(T other) where T : IShape {
        if (other is Rectangle otherRectangle) return Geometry.Overlaps(this, otherRectangle);
        if (other is Circle otherCircle) return Geometry.Overlaps(this, otherCircle);
        return false;
    }

    public bool Overlaps(Circle other) {
        return Geometry.Overlaps(this, other);
    }

    public bool Overlaps(Rectangle other) {
        return Geometry.Overlaps(this, other);
    }

    public bool OverlapsCircle(float x, float y, float radius) {
        return Geometry.CirclesOverlap(Position.X, Position.Y, Radius, x, y, radius);
    }

    public bool OverlapsRectangle(float x, float y, float width, float height) {
        return Geometry.CircleRectangleOverlap(Position.X, Position.Y, Radius, x, y, width, height);
    }

    public bool IsPointInside(Vector2 point) {
        return Geometry.IsPointInsideCircle(point, this);
    }

    public bool IsPointInside(float px, float py) {
        return Geometry.IsPointInsideCircle(px, py, this);
    }

    public bool SameTypeAs(IShape other) {
        return other is Circle;
    }

    public IEnumerable<(int, int)> GetCoveredCells(int cellSize) {
        var minCellX = (int)Math.Floor(MinX / cellSize);
        var maxCellX = (int)Math.Ceiling(MaxX / cellSize);
        var minCellY = (int)Math.Floor(MinY / cellSize);
        var maxCellY = (int)Math.Ceiling(MaxY / cellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                yield return (x, y);
            }
        }
    }

    public bool Equals(Circle other) => Position.Equals(other.Position) && Radius == other.Radius;
    
    public Area2D CreateArea2D() {
        var area2D = new Area2D() {
            Position = Position
        };
        area2D.AddChild(new CollisionShape2D {
            Shape = new CircleShape2D {
                Radius = Radius
            }
        });
        return area2D;
    }
}