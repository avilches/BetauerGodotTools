using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

public class Rectangle : IShape {
    public Vector2 Position { get; init; }
    public Vector2 Size { get; init; }

    public float Width => Size.X;
    public float Height => Size.Y;

    public float MinX => Position.X;
    public float MaxX => Position.X + Width;
    public float MinY => Position.Y;
    public float MaxY => Position.Y + Height;

    public Rectangle() {
    }

    public Rectangle(Rect2 rect) {
        Position = rect.Position;
        Size = rect.Size;
    }

    public Rectangle(Rect2I rect) {
        Position = rect.Position;
        Size = rect.Size;
    }

    public Rectangle(Vector2 position, float width, float height) {
        Position = position;
        Size = new Vector2(width, height);
    }

    public Rectangle(float x, float y, Vector2 size) {
        Position = new Vector2(x, y);
        Size = size;
    }

    public Rectangle(float x, float y, float width, float height) {
        Position = new Vector2(x, y);
        Size = new Vector2(width, height);
    }

    public Rectangle(Vector2 position, Vector2 size) {
        Position = position;
        Size = size;
    }

    public bool Intersect<T>(T other) where T : IShape {
        if (other is Rectangle otherRectangle) return Geometry.Intersect(this, otherRectangle);
        if (other is Circle otherCircle) return Geometry.Intersect(otherCircle, this);
        return false;
    }

    public bool Intersect(Circle other) {
        return Geometry.Intersect(this, other);
    }

    public bool Intersect(Rectangle other) {
        return Geometry.Intersect(this, other);
    }

    public bool IntersectCircle(float x, float y, float radius) {
        return Geometry.IntersectCircleRectangle(x, y, radius, Position.X, Position.Y, Width, Height);
    }

    public bool IntersectRectangle(float x, float y, float width, float height) {
        return Geometry.IntersectRectangles(Position.X, Position.Y, Width, Height, x, y, width, height);
    }

    public bool IsPointInside(float px, float py) {
        return Geometry.IsPointInsideRectangle(px, py, this);
    }

    public bool IsPointInside(Vector2 point) {
        return Geometry.IsPointInsideRectangle(point, this);
    }

    public IEnumerable<(int, int)> GetIntersectingCells(int cellSize) {
        return GetIntersectingCells(Position.X, Position.Y, Width, Height, cellSize);
    }

    public static IEnumerable<(int, int)> GetIntersectingCells(float rx, float ry, float width, float height, int cellSize) {
        var minCellX = (int)(rx / cellSize);
        var maxCellX = (int)((rx + width) / cellSize);
        var minCellY = (int)(ry / cellSize);
        var maxCellY = (int)((ry + height) / cellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                yield return (x, y);
            }
        }
    }

    public bool Equals(Rect2 other) => Position.Equals(other.Position) && Size.Equals(other.Size);

    public bool Equals(Rectangle other) => Position.Equals(other.Position) && Size.Equals(other.Size);

    public Area2D CreateArea2D() {
        var area2D = new Area2D {
            Position = Position
        };
        area2D.AddChild(new CollisionShape2D {
            Shape = new RectangleShape2D() {
                Size = Size
            }
        });
        return area2D;
    }
}