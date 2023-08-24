using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

public interface IShape {
    public float MinX { get; }
    public float MaxX { get; }
    public float MinY { get; }
    public float MaxY { get; }

    public bool Intersect<T>(T other) where T : IShape;
    public bool Intersect(Circle other);
    public bool Intersect(Rectangle other);
    public bool IntersectCircle(float x, float y, float radius);
    public bool IntersectRectangle(float x, float y, float width, float height);

    public bool IsPointInside(Vector2 point);
    public bool IsPointInside(float px, float py);

    public IEnumerable<(int, int)> GetIntersectingCells(int cellSize);
    public Area2D CreateArea2D();
}