using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

public interface IShape {
    public float MinX { get; }
    public float MaxX { get; }
    public float MinY { get; }
    public float MaxY { get; }

    public bool Overlaps<T>(T other) where T : IShape;
    public bool Overlaps(Circle other);
    public bool Overlaps(Rectangle other);
    public bool OverlapsCircle(float x, float y, float radius);
    public bool OverlapsRectangle(float x, float y, float width, float height);

    public bool IsPointInside(Vector2 point);
    public bool IsPointInside(float px, float py);

    public IEnumerable<(int, int)> GetCoveredCells(int cellSize);
    bool SameTypeAs(IShape other);
    public Area2D CreateArea2D();
}