using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Collision;

public class SpatialGrid {
    public float CellSize { get; }
    public Dictionary<(int, int), List<Shape>> Grid { get; }
    public List<Shape> Shapes { get; }

    public SpatialGrid(float minRadius, float maxRadius) : this((int)((minRadius + maxRadius) * 0.5f / Mathf.Sqrt2)) {
    }

    public SpatialGrid(float cellSize) {
        CellSize = cellSize;
        Grid = new Dictionary<(int, int), List<Shape>>();
        Shapes = new List<Shape>();
    }

    public void Add(Shape shape) {
        Shapes.Add(shape);
        shape.SpatialGrid = this;
        var intersectingCells = shape.GetIntersectingCells(CellSize);
        foreach (var cell in intersectingCells) {
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
        var intersectingCells = shape.GetIntersectingCells(CellSize);
        foreach (var cell in intersectingCells) {
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

    public bool IntersectPoint(Vector2 point) {
        return IntersectPoint(point.X, point.Y);
    }

    public bool IntersectPoint(float px, float py) {
        return GetIntersectingShapesInPoint(px, py).GetEnumerator().MoveNext();
    }

    public bool IntersectShape(Shape shape) {
        return GetIntersectingShapesInShape(shape).GetEnumerator().MoveNext();
    }

    public bool IntersectCircle(float cx, float cy, float radius) {
        return GetIntersectingShapesInCircle(cx, cy, radius).GetEnumerator().MoveNext();
    }

    public bool IntersectRectangle(float x, float y, float width, float height) {
        return GetIntersectingShapesInRectangle(x, y, width, height).GetEnumerator().MoveNext();
    }

    public IEnumerable<Shape> GetIntersectingShapesInPoint(Vector2 point) {
        return GetIntersectingShapesInPoint(point.X, point.Y);
    }

    public IEnumerable<Shape> GetIntersectingShapesInPoint(float px, float py) {
        var cell = Geometry.GetPointIntersectingCell(px, py, CellSize);
        if (!Grid.TryGetValue(cell, out var shapesInCell)) {
            yield break;
        }
        for (var i = 0; i < shapesInCell.Count; i++) {
            var otherShape = shapesInCell[i];
            if (otherShape.IntersectPoint(px, py)) yield return otherShape;
        }
    }

    public IEnumerable<Shape> GetIntersectingShapesInCircle(float cx, float cy, float radius) {
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

    public IEnumerable<Shape> GetIntersectingShapesInRectangle(float x, float y, float width, float height) {
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

    public IEnumerable<Shape> GetIntersectingShapesInShape(Shape shape) {
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

    internal void Move(Point point, float x, float y) {
        if (!Shapes.Contains(point)) return;
        if (point.Position.X == x && point.Position.Y == y) return;
        var intersectingCells = point.GetIntersectingCells(CellSize)[0];
        var newIntersectingCells = Geometry.GetPointIntersectingCell(x, y, CellSize);
        if (intersectingCells != newIntersectingCells) {
            RemoveShapeFromCell(point, intersectingCells);
            AddShapeToCell(point, newIntersectingCells);
        }
    }

    internal void Move(Circle rectangle, float x, float y) {
        if (!Shapes.Contains(rectangle)) return;
        if (rectangle.Position.X == x && rectangle.Position.Y == y) return;
        var intersectingCells = rectangle.GetIntersectingCells(CellSize);
        var newIntersectingCells = Geometry.GetCircleIntersectingCells(x, y, rectangle.Radius, CellSize);
        RefreshCells(rectangle, intersectingCells, newIntersectingCells);
    }

    internal void Move(Rectangle rectangle, float x, float y) {
        if (!Shapes.Contains(rectangle)) return;
        if (rectangle.Position.X == x && rectangle.Position.Y == y) return;
        var intersectingCells = rectangle.GetIntersectingCells(CellSize);
        var newIntersectingCells = Geometry.GetRectangleIntersectingCells(x, y, rectangle.Width, rectangle.Height, CellSize);
        RefreshCells(rectangle, intersectingCells, newIntersectingCells);
    }

    internal void Resize(Circle circle, float radius) {
        if (!Shapes.Contains(circle)) return;
        if (circle.Radius == radius) return;
        var intersectingCells = circle.GetIntersectingCells(CellSize);
        var newIntersectingCells = Geometry.GetCircleIntersectingCells(circle.Position.X, circle.Position.Y, radius, CellSize);
        RefreshCells(circle, intersectingCells, newIntersectingCells);
    }

    internal void Resize(Rectangle rectangle, float width, float height) {
        if (!Shapes.Contains(rectangle)) return;
        if (rectangle.Size.X == width && rectangle.Size.Y == height) return;
        var intersectingCells = rectangle.GetIntersectingCells(CellSize);
        var newIntersectingCells = Geometry.GetRectangleIntersectingCells(rectangle.Position.X, rectangle.Position.Y, width, height, CellSize);
        RefreshCells(rectangle, intersectingCells, newIntersectingCells);
    }

    private void RefreshCells(Shape shape, IEnumerable<(int, int)> intersectingCells, IEnumerable<(int, int)> newIntersectingCells) {
        var toDelete = intersectingCells.Except(newIntersectingCells);
        foreach (var cell in toDelete) {
            RemoveShapeFromCell(shape, cell);
        }
        var toAdd = newIntersectingCells.Except(intersectingCells);
        foreach (var cell in toAdd) {
            AddShapeToCell(shape, cell);
        }
    }
}