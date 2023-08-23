using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;

namespace Betauer.Core.Collision;

public class SpatialGrid {
    public int CellSize { get; }
    public Dictionary<(int, int), List<IShape>> Grid { get; }
    public List<IShape> Shapes { get; }

    public SpatialGrid(float minRadius, float maxRadius) {
        CellSize = (int)((minRadius + maxRadius) * 0.5f / Mathf.Sqrt2);
    }

    public SpatialGrid(int cellSize) {
        CellSize = cellSize;
        Grid = new Dictionary<(int, int), List<IShape>>();
        Shapes = new List<IShape>();
    }

    public void AddShape(IShape shape) {
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

    public void RemoveShape(IShape shape) {
        if (Shapes.Remove(shape)) {
            var affectedCells = GetCellsForShape(shape);
            foreach (var cell in affectedCells) {
                if (Grid.TryGetValue(cell, out var shapesInCell)) {
                    if (shapesInCell.Remove(shape) && shapesInCell.Count == 0) {
                        Grid.Remove(cell);
                    }
                }
            }
        }
    }

    public bool Overlaps(IShape shape) {
        foreach (var cell in shape.GetCoveredCells(CellSize)) {
            if (Grid.TryGetValue(cell, out var shapesInCell)) {
                var span = CollectionsMarshal.AsSpan(shapesInCell);
                // First check same type of shapes because circle/circle or rectangle/rectangle are faster
                for (var i = 0; i < span.Length; i++) {
                    var otherShape = span[i];
                    if (shape == otherShape) continue; // ignore itself
                    if (!shape.SameTypeAs(otherShape)) continue; // ignore other types, only check same type
                    if (shape.Overlaps(otherShape)) {
                        return true;
                    }
                }

                for (var i = 0; i < span.Length; i++) {
                    var otherShape = span[i];
                    if (shape == otherShape) continue; // ignore itself
                    if (shape.SameTypeAs(otherShape)) continue; // ignore same type, checked before
                    if (shape.Overlaps(otherShape)) {
                        return true;
                    }
                }
            }
        }
        return false;
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