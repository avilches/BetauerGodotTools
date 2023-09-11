using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Collision.Spatial2D;

/// <summary>
/// A Spatial Grid to store shapes and quickly find shapes that intersect with a given shape. It allows
/// - Add and remove points, circles and rectangles.
/// - Query if there is any shape in a given point, circle or rectangle.
/// - Get all the shapes intersecting a point, circle or rectangle.
/// - Shapes can be moved or resized and the grid will be updated automatically.
/// - methods shape.TryResize() and shape.TryMove() to move or resize a shape if, and only if, it doesn't collide with any other shape.
/// </summary>
public class SpatialGrid {
    public float CellSize { get; }
    public Dictionary<(int, int), List<Shape>> Grid { get; }
    protected List<Shape> Shapes { get; }

    public SpatialGrid(float cellSize) {
        CellSize = cellSize;
        Grid = new Dictionary<(int, int), List<Shape>>();
        Shapes = new List<Shape>();
    }

    public List<Shape> FindShapes() => Shapes.ToList();
    public List<Shape> FindShapes(Func<Shape, bool> match) => Shapes.Where(match).ToList();
    
    public List<T> FindShapes<T>() where T : Shape => Shapes.OfType<T>().ToList();
    public List<T> FindShapes<T>(Func<T, bool> match) where T : Shape => Shapes.OfType<T>().Where(match).ToList();

    public void ForEach(Action<Shape> action) => Shapes.ForEach(action);
    public void ForEach<T>(Action<T> action) where T : Shape => Shapes.ForEach(shape => {
        if (shape is T t) {
            action(t);
        }
    });

    public void RemoveAll() {
        Shapes.ForEach(shape => shape.SpatialGrid = null);
        Shapes.Clear();
        Grid.Clear();
    }
    
    public void RemoveAll(Func<Shape, bool> match) => Shapes.RemoveAll(shape => {
        if (!match(shape)) return false;
        if (shape is Point point) RemovePointFromCells(point);
        if (shape is Circle circle) RemoveCircleFromCells(circle);
        if (shape is Rectangle rectangle) RemoveRectangleFromCells(rectangle);
        shape.SpatialGrid = null;
        return true;
    });
    
    public void RemoveAll<T>(Func<T, bool> match) where T : Shape => RemoveAll(shape => shape is T t && match(t));

    public static SpatialGrid FromAverageDistance(float averageDistance) =>
        new SpatialGrid(averageDistance / Mathf.Sqrt2);

    public static SpatialGrid FromAverageDistance(float minRadius, float maxRadius) =>
        new SpatialGrid((int)((minRadius + maxRadius) * 0.5f / Mathf.Sqrt2));

    public void Add(Shape shape) {
        if (shape is Point point) Add(point);
        if (shape is Circle circle) Add(circle);
        if (shape is Rectangle rectangle) Add(rectangle);
    }

    public void Add(Point point) {
        Shapes.Add(point);
        point.SpatialGrid = this;
        var x = (int)Mathf.Floor(point.X / CellSize);
        var y = (int)Mathf.Floor(point.Y / CellSize);
        AddShapeToCell(point, (x, y));
    }

    public void Add(Circle circle) {
        Shapes.Add(circle);
        circle.SpatialGrid = this;
        var minCellX = (int)Mathf.Floor((circle.X - circle.Radius) / CellSize);
        var maxCellX = (int)Mathf.Floor((circle.X + circle.Radius) / CellSize);
        var minCellY = (int)Mathf.Floor((circle.Y - circle.Radius) / CellSize);
        var maxCellY = (int)Mathf.Floor((circle.Y + circle.Radius) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                AddShapeToCell(circle, (x, y));
            }
        }
    }

    public void Add(Rectangle rectangle) {
        Shapes.Add(rectangle);
        rectangle.SpatialGrid = this;
        var minCellX = (int)Mathf.Floor(rectangle.X / CellSize);
        var maxCellX = (int)Mathf.Floor((rectangle.X + rectangle.Width) / CellSize);
        var minCellY = (int)Mathf.Floor(rectangle.Y / CellSize);
        var maxCellY = (int)Mathf.Floor((rectangle.Y + rectangle.Height) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                AddShapeToCell(rectangle, (x, y));
            }
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
        if (shape is Point point) Remove(point);
        if (shape is Circle circle) Remove(circle);
        if (shape is Rectangle rectangle) Remove(rectangle);
    }

    public void Remove(Point point) {
        Shapes.Remove(point);
        point.SpatialGrid = this;
        RemovePointFromCells(point);
    }

    public void Remove(Circle circle) {
        Shapes.Remove(circle);
        circle.SpatialGrid = this;
        RemoveCircleFromCells(circle);
    }

    private void RemovePointFromCells(Point point) {
        var x = (int)Mathf.Floor(point.X / CellSize);
        var y = (int)Mathf.Floor(point.Y / CellSize);
        RemoveShapeFromCell(point, (x, y));
    }

    public void Remove(Rectangle rectangle) {
        Shapes.Remove(rectangle);
        rectangle.SpatialGrid = this;
        RemoveRectangleFromCells(rectangle);
    }

    private void RemoveCircleFromCells(Circle circle) {
        var minCellX = (int)Mathf.Floor((circle.X - circle.Radius) / CellSize);
        var maxCellX = (int)Mathf.Floor((circle.X + circle.Radius) / CellSize);
        var minCellY = (int)Mathf.Floor((circle.Y - circle.Radius) / CellSize);
        var maxCellY = (int)Mathf.Floor((circle.Y + circle.Radius) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                RemoveShapeFromCell(circle, (x, y));
            }
        }
    }

    private void RemoveRectangleFromCells(Rectangle rectangle) {
        var minCellX = (int)Mathf.Floor(rectangle.X / CellSize);
        var maxCellX = (int)Mathf.Floor((rectangle.X + rectangle.Width) / CellSize);
        var minCellY = (int)Mathf.Floor(rectangle.Y / CellSize);
        var maxCellY = (int)Mathf.Floor((rectangle.Y + rectangle.Height) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                RemoveShapeFromCell(rectangle, (x, y));
            }
        }
    }

    private void RemoveShapeFromCell(Shape shape, (int, int) cell) {
        if (Grid.TryGetValue(cell, out var shapesInCell)) {
            if (shapesInCell.Remove(shape) && shapesInCell.Count == 0) {
                Grid.Remove(cell);
            }
        }
    }

    public bool IntersectShape(Shape shape) {
        if (shape is Point point) return IntersectPoint(point.X, point.Y, point);
        if (shape is Circle circle) return IntersectCircle(circle.X, circle.Y, circle.Radius, circle);
        if (shape is Rectangle rectangle) return IntersectRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rectangle);
        return false;
    }

    public bool IntersectPoint(float px, float py, Shape exclude = null) {
        var x = (int)Mathf.Floor(px / CellSize);
        var y = (int)Mathf.Floor(py / CellSize);
        if (!Grid.TryGetValue((x, y), out var shapesInCell)) {
            return false;
        }
        for (var i = 0; i < shapesInCell.Count; i++) {
            var otherShape = shapesInCell[i];
            if (exclude != null && exclude == otherShape) continue;
            if (otherShape.IntersectPoint(px, py)) return true;
        }
        return false;
    }

    public bool IntersectCircle(float cx, float cy, float radius, Shape exclude = null) {
        var minCellX = (int)Mathf.Floor((cx - radius) / CellSize);
        var maxCellX = (int)Mathf.Floor((cx + radius) / CellSize);
        var minCellY = (int)Mathf.Floor((cy - radius) / CellSize);
        var maxCellY = (int)Mathf.Floor((cy + radius) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                if (!Grid.TryGetValue((x, y), out var shapesInCell)) {
                    continue;
                }
                for (var i = 0; i < shapesInCell.Count; i++) {
                    var otherShape = shapesInCell[i];
                    if (exclude != null && exclude == otherShape) continue;
                    if (otherShape.IntersectCircle(cx, cy, radius)) return true;
                }
            }
        }
        return false;
    }

    public bool IntersectRectangle(float rx, float ry, float width, float height, Shape exclude = null) {
        var minCellX = (int)Mathf.Floor(rx / CellSize);
        var maxCellX = (int)Mathf.Floor((rx + width) / CellSize);
        var minCellY = (int)Mathf.Floor(ry / CellSize);
        var maxCellY = (int)Mathf.Floor((ry + height) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                if (!Grid.TryGetValue((x, y), out var shapesInCell)) {
                    continue;
                }
                for (var i = 0; i < shapesInCell.Count; i++) {
                    var otherShape = shapesInCell[i];
                    if (exclude != null && exclude == otherShape) continue;
                    if (otherShape.IntersectRectangle(rx, ry, width, height)) return true;
                }
            }
        }
        return false;
    }

    public IEnumerable<Shape> GetIntersectingShapesInShape(Shape shape) {
        if (shape is Point point) return GetIntersectingShapesInPoint(point.X, point.Y, point);
        if (shape is Circle circle) return GetIntersectingShapesInCircle(circle.X, circle.Y, circle.Radius, circle);
        if (shape is Rectangle rectangle)
            return GetIntersectingShapesInRectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rectangle);
        return Enumerable.Empty<Shape>();
    }

    public IEnumerable<Shape> GetIntersectingShapesInPoint(float px, float py, Shape exclude = null) {
        var x = (int)Mathf.Floor(px / CellSize);
        var y = (int)Mathf.Floor(py / CellSize);
        if (!Grid.TryGetValue((x, y), out var shapesInCell)) {
            yield break;
        }
        for (var i = 0; i < shapesInCell.Count; i++) {
            var otherShape = shapesInCell[i];
            if (exclude != null && exclude == otherShape) continue;
            if (otherShape.IntersectPoint(px, py)) yield return otherShape;
        }
    }

    public IEnumerable<Shape> GetIntersectingShapesInCircle(float cx, float cy, float radius, Shape exclude = null) {
        var minCellX = (int)Mathf.Floor((cx - radius) / CellSize);
        var maxCellX = (int)Mathf.Floor((cx + radius) / CellSize);
        var minCellY = (int)Mathf.Floor((cy - radius) / CellSize);
        var maxCellY = (int)Mathf.Floor((cy + radius) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                if (!Grid.TryGetValue((x, y), out var shapesInCell)) {
                    continue;
                }
                for (var i = 0; i < shapesInCell.Count; i++) {
                    var otherShape = shapesInCell[i];
                    if (exclude != null && exclude == otherShape) continue;
                    if (otherShape.IntersectCircle(cx, cy, radius)) yield return otherShape;
                }
            }
        }
    }

    public IEnumerable<Shape> GetIntersectingShapesInRectangle(float rx, float ry, float width, float height, Shape exclude = null) {
        var minCellX = (int)Mathf.Floor(rx / CellSize);
        var maxCellX = (int)Mathf.Floor((rx + width) / CellSize);
        var minCellY = (int)Mathf.Floor(ry / CellSize);
        var maxCellY = (int)Mathf.Floor((ry + height) / CellSize);

        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                if (!Grid.TryGetValue((x, y), out var shapesInCell)) {
                    continue;
                }
                for (var i = 0; i < shapesInCell.Count; i++) {
                    var otherShape = shapesInCell[i];
                    if (exclude != null && exclude == otherShape) continue;
                    if (otherShape.IntersectRectangle(rx, ry, width, height)) yield return otherShape;
                }
            }
        }
    }

    internal void Update(Point point, float x, float y) {
        if (!Shapes.Contains(point)) return;
        if (point.X == x && point.Y == y) return;
        var intersectingCells = point.GetIntersectingCells(CellSize)[0];
        var newIntersectingCells = Geometry.GetPointIntersectingCell(x, y, CellSize);
        if (intersectingCells != newIntersectingCells) {
            RemoveShapeFromCell(point, intersectingCells);
            AddShapeToCell(point, newIntersectingCells);
        }
    }

    internal void Update(Circle circle, float x, float y, float radius) {
        if (!Shapes.Contains(circle)) return;
        if (circle.X == x && circle.Y == y && circle.Radius == radius) return;
        var intersectingCells = circle.GetIntersectingCells(CellSize);
        var newIntersectingCells = Geometry.GetCircleIntersectingCells(x, y, radius, CellSize);
        RefreshCells(circle, intersectingCells, newIntersectingCells);
    }

    internal void Update(Rectangle rectangle, float x, float y, float width, float height) {
        if (!Shapes.Contains(rectangle)) return;
        if (rectangle.Size.X == width && rectangle.Size.Y == height && rectangle.X == x && rectangle.Y == y) return;
        var intersectingCells = rectangle.GetIntersectingCells(CellSize);
        var newIntersectingCells = Geometry.GetRectangleIntersectingCells(x, y, width, height, CellSize);
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