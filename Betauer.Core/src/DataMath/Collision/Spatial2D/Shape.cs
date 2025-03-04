using Godot;

namespace Betauer.Core.DataMath.Collision.Spatial2D;

public abstract class Shape {
    public SpatialGrid? SpatialGrid { get; internal set; }
    
    public abstract Vector2 Position { get; set; }
    public float X => Position.X;
    public float Y => Position.Y;
    
    public abstract Vector2 Center { get; }
    public float CenterX => Position.X;
    public float CenterY => Position.Y;

    public abstract float Width { get; }
    public abstract float Height { get; }
    public abstract float MinX { get; }
    public abstract float MaxX { get; }
    public abstract float MinY { get; }
    public abstract float MaxY { get; }

    public abstract bool TryMove(float x, float y);
    public bool TryMove(Vector2 position) => TryMove(position.X, position.Y);

    public abstract bool IntersectCircle(float x, float y, float radius);
    public abstract bool IntersectRectangle(float x, float y, float width, float height);
    public abstract bool IntersectPoint(float px, float py);
    public bool Intersect<T>(T other) {
        if (other is Rectangle rectangle) return Intersect(rectangle);
        if (other is Circle circle) return Intersect(circle);
        if (other is Point point) return Intersect(point.Position);
        return false;
    }
    public bool Intersect(Circle other) => IntersectCircle(other.X, other.Y, other.Radius);
    public bool Intersect(Rectangle other) => IntersectRectangle(other.MinX, other.MinY, other.Width, other.Height);
    public bool Intersect(Vector2 point) => IntersectPoint(point.X, point.Y);

    public abstract (int, int)[] GetIntersectingCells(float cellSize);
    public abstract Area2D CreateArea2D();
}