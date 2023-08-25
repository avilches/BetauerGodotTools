using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Collision;

public class Rectangle : Shape {
    private Vector2 _position;
    private Vector2 _size;

    public Vector2 Position {
        get => _position;
        set {
            SpatialGrid?.Move(this, value.X, value.Y);
            _position = value;
        }
    }

    public Vector2 Size {
        get => _size;
        set {
            SpatialGrid?.Resize(this, value.X, value.Y);
            _size = value;
        }
    }

    public bool TryResize(float width, float height) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetIntersectingShapesInRectangle(Position.X, Position.Y, width, height).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Resize(this, width, height);
        }
        _size = new Vector2(width, height);
        return true;
    }

    public override bool TryMove(float x, float y) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetIntersectingShapesInRectangle(x, y, Width, Height).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Move(this, x, y);
        }
        _position = new Vector2(x, y);
        return true;
    }

    public float Width => Size.X;
    public float Height => Size.Y;

    public override float MinX => Position.X;
    public override float MaxX => Position.X + Width;
    public override float MinY => Position.Y;
    public override float MaxY => Position.Y + Height;

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

    public override bool IntersectCircle(float x, float y, float radius) {
        return Geometry.IntersectCircleRectangle(x, y, radius, Position.X, Position.Y, Width, Height);
    }

    public override bool IntersectRectangle(float x, float y, float width, float height) {
        return Geometry.IntersectRectangles(Position.X, Position.Y, Width, Height, x, y, width, height);
    }

    public override bool IntersectPoint(float px, float py) {
        return Geometry.IsPointInsideRectangle(px, py, this);
    }

    public override (int, int)[] GetIntersectingCells(float cellSize) {
        return Geometry.GetRectangleIntersectingCells(Position.X, Position.Y, Width, Height, cellSize);
    }

    public override Area2D CreateArea2D() {
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

    public bool Equals(Rect2 other) => Position.Equals(other.Position) && Size.Equals(other.Size);
    
    public bool Equals(Rectangle other) => Position.Equals(other.Position) && Size.Equals(other.Size);
}