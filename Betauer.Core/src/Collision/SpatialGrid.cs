using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;

namespace Betauer.Core.Collision;

public class SpatialGrid {
    public int CellSize { get; }
    public Dictionary<(int, int), List<IShape>> Grid { get; }
    public List<IShape> Shapes { get; }

    public SpatialGrid(float minRadius, float maxRadius) : this((int)((minRadius + maxRadius) * 0.5f / Mathf.Sqrt2)) {
    }

    public SpatialGrid(int cellSize) {
        CellSize = cellSize;
        Grid = new Dictionary<(int, int), List<IShape>>();
        Shapes = new List<IShape>();
    }

    public void Add(IShape shape) {
        Shapes.Add(shape);
        var affectedCells = shape.GetCoveredCells(CellSize);
        foreach (var cell in affectedCells) {
            if (Grid.TryGetValue(cell, out var shapesInCell)) {
                shapesInCell.Add(shape);
            } else {
                Grid[cell] = new List<IShape> { shape };
            }
        }
    }

    public void Remove(IShape shape) {
        if (!Shapes.Remove(shape)) {
            return;
        }
        var affectedCells = shape.GetCoveredCells(CellSize);
        foreach (var cell in affectedCells) {
            if (Grid.TryGetValue(cell, out var shapesInCell)) {
                if (shapesInCell.Remove(shape) && shapesInCell.Count == 0) {
                    Grid.Remove(cell);
                }
            }
        }
    }

    public bool Overlaps(IShape shape) {
        return GetOverlaps(shape).GetEnumerator().MoveNext();
    }

    public bool CircleOverlaps(float cx, float cy, float radius) {
        return GetCircleOverlaps(cx, cy, radius).GetEnumerator().MoveNext();
    }

    public bool RectangleOverlaps(float x, float y, float width, float height) {
        return GetRectangleOverlaps(x,y, width, height).GetEnumerator().MoveNext();
    }

    public IEnumerable<IShape> GetOverlaps(IShape shape) {
        foreach (var cell in shape.GetCoveredCells(CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (shape == otherShape) continue; // ignore itself
                if (shape.Overlaps(otherShape)) yield return otherShape;
            }
        }
    }

    public IEnumerable<IShape> GetCircleOverlaps(float cx, float cy, float radius) {
        foreach (var cell in Circle.GetCoveredCells(cx, cy, radius, CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (otherShape.OverlapsCircle(cx, cy, radius)) yield return otherShape;
            }
        }
    }

    public IEnumerable<IShape> GetRectangleOverlaps(float x, float y, float width, float height) {
        foreach (var cell in Rectangle.GetCoveredCells(x, y, width, height, CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (otherShape.OverlapsRectangle(x, y, width, height)) yield return otherShape;
            }
        }
    }
}