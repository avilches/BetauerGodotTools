using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Collision.Spatial2D;

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
                        rectangle.Position = new Vector2(rectangle.Position.X + amount, rectangle.Position.Y);
                        if (!grid.IntersectShape(rectangle)) {
                            return true; // rectangle was resized successfully, delete it
                        }
                        rectangle.Position = new Vector2(rectangle.Position.X, rectangle.Position.Y + amount);
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
    public static void ExpandAll(this SpatialGrid grid, float amount) {
        var pending = grid.FindShapes();
        if (pending.Count < 2) return; // at least 2 shapes are needed to expand, so they can touch each other
        while (pending.Count > 0) {
            // remove all the pending circles that can't be expanded
            pending.RemoveAll((shape) => {
                switch (shape) {
                    case Circle circle:
                        if (circle.TryResize(circle.Radius + amount)) {
                            return false; // circle was resized successfully, don't delete it
                        }
                        return true; // circle can't be resized, delete it
                    case Rectangle rectangle: {
                        if (rectangle.TryResize(rectangle.Width + amount, rectangle.Height) &&
                            rectangle.TryResize(rectangle.Width, rectangle.Height + amount) &&
                            rectangle.TryUpdate(rectangle.Position.X - amount, rectangle.Position.Y, rectangle.Width + amount, rectangle.Height) &&
                            rectangle.TryUpdate(rectangle.Position.X, rectangle.Position.Y - amount, rectangle.Width, rectangle.Height + amount)) {
                            return false; // shape was resized successfully, don't delete it 
                        }
                        return true; // rectangle failed at some step when resized/moved, delete it
                    }
                    default:
                        return true; // unknown shape (point), can't be expanded, delete it
                }
            });
        }
    }
}