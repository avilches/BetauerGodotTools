using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Collision.Spatial2D;

public static class SpatialGridExtensions {
    public static void AddAll(this SpatialGrid grid, IEnumerable<Shape> shapes) {
        foreach (var shape in shapes) {
            grid.Add(shape);
        }
    }

    public static void AddPointsAsCircles(this SpatialGrid grid, IEnumerable<Vector2> points, float radius = 0.5f) {
        foreach (var point in points) {
            grid.Add(new Circle(point.X, point.Y, radius));
        }
    }

    public static void AddPointsAsRectangles(this SpatialGrid grid, IEnumerable<Vector2> points, float width = 0.5f, float height = 0.5f) {
        foreach (var point in points) {
            grid.Add(new Rectangle(point.X, point.Y, width, height));
        }
    }

    /// <summary>
    /// Check if every shape collides with any other shape, and resize it until it doesn't collide
    /// The result is a grid with no collisions
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="amount"></param>
    public static void AdjustAll(this SpatialGrid grid, float minSize = 1, float amount = 0.1f) {
        var pending = grid.FindShapes(grid.IntersectShape);
        while (pending.Count > 0) {
            pending.RemoveAll((shape) => {
                switch (shape) {
                    case Circle circle:
                        if (circle.Radius <= minSize) return true;
                        circle.Radius -= amount;
                        if (!grid.IntersectShape(circle)) {
                            return true; // circle was resized successfully, delete it
                        }
                        return false; // circle is still colliding, don't delete it
                    case Rectangle rectangle: {
                        if (rectangle.Width <= minSize || rectangle.Height <= minSize) return true;
                        rectangle.Size = new Vector2(rectangle.Width - amount, rectangle.Height);
                        if (!grid.IntersectShape(rectangle)) {
                            return true; // rectangle was resized successfully, delete it
                        }
                        rectangle.Size = new Vector2(rectangle.Width, rectangle.Height - amount);
                        if (!grid.IntersectShape(rectangle)) {
                            return true; // rectangle was resized successfully, delete it
                        }
                        rectangle.Position = new Vector2(rectangle.X + amount, rectangle.Y);
                        if (!grid.IntersectShape(rectangle)) {
                            return true; // rectangle was resized successfully, delete it
                        }
                        rectangle.Position = new Vector2(rectangle.X, rectangle.Y + amount);
                        if (!grid.IntersectShape(rectangle)) {
                            return true; // rectangle was resized successfully, delete it
                        }
                        rectangle.Size = new Vector2(rectangle.Width - amount, rectangle.Height);
                        if (!grid.IntersectShape(rectangle)) {
                            return true; // rectangle was resized successfully, delete it
                        }
                        rectangle.Size = new Vector2(rectangle.Width, rectangle.Height - amount);
                        if (!grid.IntersectShape(rectangle)) {
                            return true; // rectangle was resized successfully, delete it
                        }
                        return false; // rectangle is still colliding, don't delete it
                    }
                    default:
                        return true; // unknown shape (point), can't be shrank, delete it
                }
            });
        }
    }

    /// <summary>
    /// Try to expand all the shapes in the grid by the given amount until they can't expand anymore because they are touching each other
    /// The result is a grid with no collisions but impossible to expand again.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="amount"></param>
    /// <returns>number of expansions. 0 means it was not possible to expand even once</returns>
    public static void ExpandAll(this SpatialGrid grid, float amount, Rect2I? bounds = null) {
        List<Shape> pending = grid.FindShapes<Circle>().Cast<Shape>().ToList();
        pending.AddRange(grid.FindShapes<Rectangle>());
        while (pending.Count > 0) {
            ExpandAll(grid, pending, amount, bounds);
        }
    }

    /// <summary>
    /// Try to expand the shapes in the grid by the given amount without touching each other.
    /// It returns only the shapes expanded, so it can be called multiple times until no more shapes can be expanded.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="pending"></param>
    /// <param name="amount"></param>
    /// <returns>number of expansions. 0 means it was not possible to expand even once</returns>
    public static void ExpandAll(this SpatialGrid grid, List<Shape> pending, float amount, Rect2I? bounds = null) {
        if (pending.Count < 2) {
            pending.Clear();
            return; // at least 2 shapes are needed to expand, so they can touch each other
        }
        var expanded = 0;
        const bool KEEP_EXPANDING_THIS_SHAPE = false;
        const bool STOP_EXPANDING_THIS_SHAPE = true;
        // remove all the pending circles that can't be expanded
        pending.RemoveAll((shape) => {
            switch (shape) {
                case Circle circle:
                    if (circle.TryResize(circle.Radius + amount)) {
                        if (bounds.HasValue && ReachBounds(bounds.Value, circle)) {
                            circle.Update(circle.X, circle.Y, circle.Radius - amount);
                            // shape was resized successfully, but it reached the bounds, restore previous size and delete it
                            return STOP_EXPANDING_THIS_SHAPE;
                        }
                        expanded++;
                        return KEEP_EXPANDING_THIS_SHAPE; // shape was resized successfully, don't delete it 
                    }
                    return STOP_EXPANDING_THIS_SHAPE; // circle can't be resized, delete it
                case Rectangle rectangle: {
                    var copy = rectangle.ToRect2();
                    if (rectangle.TryResize(rectangle.Width + amount, rectangle.Height) &&
                        rectangle.TryResize(rectangle.Width, rectangle.Height + amount) &&
                        rectangle.TryUpdate(rectangle.X - amount, rectangle.Y, rectangle.Width + amount, rectangle.Height) &&
                        rectangle.TryUpdate(rectangle.X, rectangle.Y - amount, rectangle.Width, rectangle.Height + amount)) {
                        if (bounds.HasValue && ReachBounds(bounds.Value, rectangle)) {
                            rectangle.Update(copy.Position.X, copy.Position.Y, copy.Size.X, copy.Size.Y);
                            // shape was resized successfully, but it reached the bounds, restore previous size and delete it
                            return STOP_EXPANDING_THIS_SHAPE;
                        }
                        expanded++;
                        return KEEP_EXPANDING_THIS_SHAPE; // shape was resized successfully, don't delete it 
                    }
                    
                    // rectangle failed at some step when resized/moved, delete it
                    // before deleting it, restore the previous size
                    if (bounds.HasValue && ReachBounds(bounds.Value, rectangle)) {
                        rectangle.Update(copy.Position.X, copy.Position.Y, copy.Size.X, copy.Size.Y);
                    }
                    return STOP_EXPANDING_THIS_SHAPE; 
                }
                default:
                    return STOP_EXPANDING_THIS_SHAPE; // unknown shape (point), can't be expanded, delete it
            }
        });
    }

    private static bool ReachBounds(Rect2I bounds, Shape rectangle) {
        return rectangle.MaxX > bounds.Size.X ||
               rectangle.MaxY > bounds.Size.Y ||
               rectangle.MinX < bounds.Size.X ||
               rectangle.MinY < bounds.Size.Y;
    }
}