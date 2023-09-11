using System.Linq;
using Godot;

namespace Betauer.Core.Collision.Spatial2D;

public class Rectangle : Shape {
    private Vector2 _position;
    private Vector2 _size;

    public override float Width => Size.X;
    public override float Height => Size.Y;
    public override float MinX => Position.X;
    public override float MaxX => Position.X + Width;
    public override float MinY => Position.Y;
    public override float MaxY => Position.Y + Height;

    public override Vector2 Position {
        get => _position;
        set {
            SpatialGrid?.Update(this, value.X, value.Y, _size.X, _size.Y);
            _position = value;
        }
    }

    public Vector2 Size {
        get => _size;
        set {
            SpatialGrid?.Update(this, _position.X, _position.Y, value.X, value.Y);
            _size = value;
        }
    }

    public bool Update(float x, float y, float width, float height) {
        SpatialGrid?.Update(this, x, y, width, height);
        _position = new Vector2(x, y);
        _size = new Vector2(width, height);
        return true;
    }

    public override bool TryMove(float x, float y) {
        return TryUpdate(x, y, _size.X, _size.Y);
    }

    public bool TryResize(float width, float height) {
        return TryUpdate(_position.X, _position.Y, width, height);
    }

    public bool TryUpdate(float x, float y, float width, float height) {
        if (SpatialGrid != null) {
            if (SpatialGrid.GetIntersectingShapesInRectangle(x, y, width, height).Any(shape => shape != this)) {
                return false;
            }
            SpatialGrid.Update(this, x, y, width, height);
        }
        _position = new Vector2(x, y);
        _size = new Vector2(width, height);
        return true;
    }

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