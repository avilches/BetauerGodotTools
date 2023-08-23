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
        var affectedCells = GetCellsForShape(shape);
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
        var affectedCells = GetCellsForShape(shape);
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

    public IEnumerable<IShape> GetOverlaps(IShape shape) {
        foreach (var cell in shape.GetCoveredCells(CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            // First check same type of shapes because circle/circle or rectangle/rectangle are faster
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (shape == otherShape) continue; // ignore itself
                if (!shape.SameTypeAs(otherShape)) continue; // ignore other types, only check same type
                if (shape.Overlaps(otherShape)) {
                    yield return otherShape;
                }
            }

            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (shape == otherShape) continue; // ignore itself
                if (shape.SameTypeAs(otherShape)) continue; // ignore same type, checked before
                if (shape.Overlaps(otherShape)) {
                    yield return otherShape;
                }
            }
        }
    }

    /// <summary>
    /// Returns all cells where the shape could be
    /// </summary>
    /// <param name="shape"></param>
    /// <returns></returns>
    private IEnumerable<(int, int)> GetCellsForShape(IShape shape) {
        var startX = (int)(shape.MinX / CellSize);
        var endX = (int)(shape.MaxX / CellSize);
        var startY = (int)(shape.MinY / CellSize);
        var endY = (int)(shape.MaxY / CellSize);
        for (var x = startX; x <= endX; x++) {
            for (var y = startY; y <= endY; y++) {
                yield return (x, y);
            }
        }
    }
}