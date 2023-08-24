using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

public abstract class Shape {
    public SpatialGrid? SpatialGrid { get; internal set; }
    
    public abstract float MinX { get; }
    public abstract float MaxX { get; }
    public abstract float MinY { get; }
    public abstract float MaxY { get; }

    public abstract bool Intersect<T>(T other) where T : Shape;
    public abstract bool Intersect(Circle other);
    public abstract bool Intersect(Rectangle other);
    public abstract bool IntersectCircle(float x, float y, float radius);
    public abstract bool IntersectRectangle(float x, float y, float width, float height);

    public abstract bool IsPointInside(Vector2 point);
    public abstract bool IsPointInside(float px, float py);

    public abstract IEnumerable<(int, int)> GetIntersectingCells(int cellSize);
    public abstract Area2D CreateArea2D();
}