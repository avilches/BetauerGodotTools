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
        var affectedCells = shape.GetIntersectingCells(CellSize);
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
        var affectedCells = shape.GetIntersectingCells(CellSize);
        foreach (var cell in affectedCells) {
            if (Grid.TryGetValue(cell, out var shapesInCell)) {
                if (shapesInCell.Remove(shape) && shapesInCell.Count == 0) {
                    Grid.Remove(cell);
                }
            }
        }
    }

    public bool Intersect(IShape shape) {
        return GetIntersectingShapes(shape).GetEnumerator().MoveNext();
    }

    public bool CircleIntersect(float cx, float cy, float radius) {
        return GetIntersectingShapes(cx, cy, radius).GetEnumerator().MoveNext();
    }

    public bool RectangleIntersect(float x, float y, float width, float height) {
        return GetRectangleIntersectingShapes(x,y, width, height).GetEnumerator().MoveNext();
    }

    public IEnumerable<IShape> GetIntersectingShapes(IShape shape) {
        foreach (var cell in shape.GetIntersectingCells(CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (shape == otherShape) continue; // ignore itself
                if (shape.Intersect(otherShape)) yield return otherShape;
            }
        }
    }

    public IEnumerable<IShape> GetIntersectingShapes(float cx, float cy, float radius) {
        foreach (var cell in Circle.GetIntersectingCells(cx, cy, radius, CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (otherShape.IntersectCircle(cx, cy, radius)) yield return otherShape;
            }
        }
    }

    public IEnumerable<IShape> GetRectangleIntersectingShapes(float x, float y, float width, float height) {
        foreach (var cell in Rectangle.GetIntersectingCells(x, y, width, height, CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (otherShape.IntersectRectangle(x, y, width, height)) yield return otherShape;
            }
        }
    }
}