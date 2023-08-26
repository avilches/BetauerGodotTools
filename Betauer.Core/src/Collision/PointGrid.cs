using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision;

/// <summary>
/// A basic but fast 2D grid for simple spatial queries. It only allows:
/// - Add and remove points
/// - Query if there is a point in a given circle
/// - Get all the points inside a circle
/// 
/// It's very efficient and it has a very low memory footprint. If you need more features, like adding and querying points, circles and rectangles
/// Use <see cref="PointGrid"/> instead.
/// </summary>
public sealed class PointGrid {
    public float Width { get; }
    public float Height { get; }
    public float CellSize { get; }
    public int CellsPerX { get; }
    public int CellsPerY { get; }
    public List<Vector2>?[] GridCells;
    public readonly List<Vector2> Items;

    public PointGrid(float width, float height, float minRadius, float maxRadius) {
        Width = width;
        Height = height;
        CellSize = ((minRadius + maxRadius) * 0.5f) / Mathf.Sqrt2;
        CellsPerX = (int)Math.Ceiling(Width / CellSize);
        CellsPerY = (int)Math.Ceiling(Height / CellSize);
        GridCells = new List<Vector2>[CellsPerX * CellsPerY];
        Items = new List<Vector2>();
    }

    public bool Add(float x, float y) {
        var index = GetCellIndexFromPosition(x, y);
        if (index == -1) {
            return false;
        }

        var vector2 = new Vector2(x, y);
        Items.Add(vector2);
        var cellItems = GridCells[index];
        if (cellItems != null) {
            cellItems.Add(vector2);
        } else {
            GridCells[index] = new List<Vector2> { vector2 };
        }
        return true;
    }

    public bool Remove(float x, float y) {
        var index = GetCellIndexFromPosition(x, y);
        if (index == -1) {
            return false;
        }

        var vector2 = new Vector2(x, y);
        var cellItems = GridCells[index];
        if (cellItems != null) {
            cellItems!.Remove(vector2);
        }
        return Items.Remove(vector2);
    }

    public void Clear() {
        Items.Clear();
        for (var i = 0; i < GridCells.Length; i++) {
            GridCells[i]?.Clear();
        }
    }

    public bool Intersects(float x, float y, float radius) {
        var minCellX = Math.Max((int)((x - radius) / CellSize), 0);
        var maxCellX = Math.Min((int)((x + radius) / CellSize), CellsPerX -1);
        var minCellY = Math.Max((int)((y - radius) / CellSize), 0);
        var maxCellY = Math.Min((int)((y + radius) / CellSize), CellsPerY -1);
        var radii = radius * radius;
        for (var cellX = minCellX; cellX <= maxCellX; cellX++) {
            for (var cellY = minCellY; cellY <= maxCellY; cellY++) {
                var cellIndex = GetCellIndexFromCell(cellX, cellY);
                var cellItems = GridCells[cellIndex];
                if (cellItems == null) continue;
                for (var i = 0; i < cellItems.Count; ++i) {
                    var point = cellItems[i];
                    if (DistanceSq(x, y, point.X, point.Y) <= radii) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Gets all items in the radius around the specified point.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public IEnumerable<Vector2> GetIntersectingPoints(float x, float y, float radius) {
        var minCellX = Math.Max((int)((x - radius) / CellSize), 0);
        var maxCellX = Math.Min((int)((x + radius) / CellSize), CellsPerX -1);
        var minCellY = Math.Max((int)((y - radius) / CellSize), 0);
        var maxCellY = Math.Min((int)((y + radius) / CellSize), CellsPerY -1);
        var radii = radius * radius;
        for (var cellX = minCellX; cellX <= maxCellX; cellX++) {
            for (var cellY = minCellY; cellY <= maxCellY; cellY++) {
                var cellIndex = GetCellIndexFromCell(cellX, cellY);
                var cellItems = GridCells[cellIndex];
                if (cellItems == null) continue;
                for (var i = 0; i < cellItems.Count; ++i) {
                    var point = cellItems[i];
                    if (DistanceSq(x, y, point.X, point.Y) <= radii) {
                        yield return point;
                    }
                }
            }
        }
    }

    private static float DistanceSq(float ax, float ay, float bx, float by) {
        var dx = bx - ax;
        var dy = by - ay;
        return dx * dx + dy * dy;
    }

    private int GetCellIndexFromCell(int cellX, int cellY) {
        return cellY * CellsPerX + cellX;
    }

    private int GetCellIndexFromPosition(float x, float y) {
        if (x < 0.0f || x > Width || y < 0.0f || y > Height) {
            return -1;
        }
        var dx = (int)(x / CellSize);
        var dy = (int)(y / CellSize);
        return dy * CellsPerX + dx;
    }
}