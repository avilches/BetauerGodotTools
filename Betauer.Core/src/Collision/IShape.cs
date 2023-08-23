using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

public interface IShape {
    public float MinX { get; }
    public float MaxX { get; }
    public float MinY { get; }
    public float MaxY { get; }

    public bool Overlaps<T>(T other) where T : IShape;
    public bool IsPointInside(float px, float py);
    public IEnumerable<(int, int)> GetCoveredCells(int cellSize);
    bool SameTypeAs(IShape other);
    public Area2D CreateArea2D();

}