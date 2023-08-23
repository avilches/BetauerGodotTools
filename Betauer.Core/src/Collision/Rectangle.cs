using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

public class Rectangle : IShape {
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
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

    public bool Overlaps<T>(T other) where T : IShape {
        if (other is Rectangle otherRectangle) return Geometry.Overlaps(this, otherRectangle);
        if (other is Circle otherCircle) return Geometry.Overlaps(otherCircle, this);
        return false;
    }

    public bool IsPointInside(float px, float py) {
        return px >= MinX && px <= MaxX && py >= MinY && py <= MaxY;
    }

    public bool SameTypeAs(IShape other) {
        return other is Rectangle;
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

    public bool Equals(Rect2 other) => Position.Equals(other.Position) && Size.Equals(other.Size);

    public bool Equals(Rectangle other) => Position.Equals(other.Position) && Size.Equals(other.Size);
    
    public Area2D CreateArea2D() {
        var area2D = new Area2D() {
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