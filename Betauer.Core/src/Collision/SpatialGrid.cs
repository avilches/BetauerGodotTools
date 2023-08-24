using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Collision;

public class SpatialGrid {
    public int CellSize { get; }
    public Dictionary<(int, int), List<Shape>> Grid { get; }
    public List<Shape> Shapes { get; }

    public SpatialGrid(float minRadius, float maxRadius) : this((int)((minRadius + maxRadius) * 0.5f / Mathf.Sqrt2)) {
    }

    public SpatialGrid(int cellSize) {
        CellSize = cellSize;
        Grid = new Dictionary<(int, int), List<Shape>>();
        Shapes = new List<Shape>();
    }

    public void Add(Shape shape) {
        Shapes.Add(shape);
        shape.SpatialGrid = this;
        var affectedCells = shape.GetIntersectingCells(CellSize);
        foreach (var cell in affectedCells) {
            AddShapeToCell(shape, cell);
        }
    }

    private void AddShapeToCell(Shape shape, (int, int) cell) {
        if (Grid.TryGetValue(cell, out var shapesInCell)) {
            shapesInCell.Add(shape);
        } else {
            Grid[cell] = new List<Shape> { shape };
        }
    }

    public void Remove(Shape shape) {
        if (!Shapes.Remove(shape)) {
            return;
        }
        shape.SpatialGrid = null;
        var affectedCells = shape.GetIntersectingCells(CellSize);
        foreach (var cell in affectedCells) {
            RemoveShapeFromCell(shape, cell);
        }
    }

    private void RemoveShapeFromCell(Shape shape, (int, int) cell) {
        if (Grid.TryGetValue(cell, out var shapesInCell)) {
            if (shapesInCell.Remove(shape) && shapesInCell.Count == 0) {
                Grid.Remove(cell);
            }
        }
    }

    public bool PointIntersect(Vector2 point) {
        return PointIntersect(point.X, point.Y);
    }

    public bool PointIntersect(float px, float py) {
        return GetIntersectingShapes(px, py).GetEnumerator().MoveNext();
    }

    public bool ShapeIntersect(Shape shape) {
        return GetIntersectingShapes(shape).GetEnumerator().MoveNext();
    }

    public bool CircleIntersect(float cx, float cy, float radius) {
        return GetCircleIntersectingShapes(cx, cy, radius).GetEnumerator().MoveNext();
    }

    public bool RectangleIntersect(float x, float y, float width, float height) {
        return GetRectangleIntersectingShapes(x, y, width, height).GetEnumerator().MoveNext();
    }

    public IEnumerable<Shape> GetIntersectingShapes(Vector2 point) {
        return GetIntersectingShapes(point.X, point.Y);
    }

    public IEnumerable<Shape> GetIntersectingShapes(float px, float py) {
        var cell = Geometry.GetPointIntersectingCells(px, py, CellSize);
        if (!Grid.TryGetValue(cell, out var shapesInCell)) {
            yield break;
        }
        for (var i = 0; i < shapesInCell.Count; i++) {
            var otherShape = shapesInCell[i];
            if (otherShape.IsPointInside(px, py)) yield return otherShape;
        }
    }

    public IEnumerable<Shape> GetIntersectingShapes(Shape shape) {
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

    public IEnumerable<Shape> GetCircleIntersectingShapes(float cx, float cy, float radius) {
        foreach (var cell in Geometry.GetCircleIntersectingCells(cx, cy, radius, CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (otherShape.IntersectCircle(cx, cy, radius)) yield return otherShape;
            }
        }
    }

    public IEnumerable<Shape> GetRectangleIntersectingShapes(float x, float y, float width, float height) {
        foreach (var cell in Geometry.GetRectangleIntersectingCells(x, y, width, height, CellSize)) {
            if (!Grid.TryGetValue(cell, out var shapesInCell)) {
                continue;
            }
            for (var i = 0; i < shapesInCell.Count; i++) {
                var otherShape = shapesInCell[i];
                if (otherShape.IntersectRectangle(x, y, width, height)) yield return otherShape;
            }
        }
    }

    internal void Move(Circle rectangle, float x, float y) {
        if (!Shapes.Contains(rectangle)) return;
        if (rectangle.Position.X == x && rectangle.Position.Y == y) return;
        var affectedCells = rectangle.GetIntersectingCells(CellSize);
        var newAffectedCells = Geometry.GetCircleIntersectingCells(x, y, rectangle.Radius, CellSize);
        RefreshCells(rectangle, affectedCells, newAffectedCells);
    }

    internal void Move(Rectangle rectangle, float x, float y) {
        if (!Shapes.Contains(rectangle)) return;
        if (rectangle.Position.X == x && rectangle.Position.Y == y) return;
        var affectedCells = rectangle.GetIntersectingCells(CellSize);
        var newAffectedCells = Geometry.GetRectangleIntersectingCells(x, y, rectangle.Width, rectangle.Height, CellSize);
        RefreshCells(rectangle, affectedCells, newAffectedCells);
    }

    internal void Resize(Circle circle, float radius) {
        if (!Shapes.Contains(circle)) return;
        if (circle.Radius == radius) return;
        var affectedCells = circle.GetIntersectingCells(CellSize);
        var newAffectedCells = Geometry.GetCircleIntersectingCells(circle.Position.X, circle.Position.Y, radius, CellSize);
        RefreshCells(circle, affectedCells, newAffectedCells);
    }

    internal void Resize(Rectangle rectangle, float width, float height) {
        if (!Shapes.Contains(rectangle)) return;
        if (rectangle.Size.X == width && rectangle.Size.Y == height) return;
        var affectedCells = rectangle.GetIntersectingCells(CellSize);
        var newAffectedCells = Geometry.GetRectangleIntersectingCells(rectangle.Position.X, rectangle.Position.Y, width, height, CellSize);
        RefreshCells(rectangle, affectedCells, newAffectedCells);
    }

    private void RefreshCells(Shape shape, IEnumerable<(int, int)> affectedCells, IEnumerable<(int, int)> newAffectedCells) {
        var toDelete = affectedCells.Except(newAffectedCells);
        foreach (var cell in toDelete) {
            RemoveShapeFromCell(shape, cell);
        }
        var toAdd = newAffectedCells.Except(affectedCells);
        foreach (var cell in toAdd) {
            AddShapeToCell(shape, cell);
        }
    }
}