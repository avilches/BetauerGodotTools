using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Collision.Spatial2D;
using Godot;

namespace Betauer.Core.PCG.GridTools;

/// <summary>
/// A grid-based pathfinding system that integrates with SpatialGrid for efficient collision detection and obstacle handling.
/// The grid supports dynamic obstacles of different shapes (Point, Circle, Rectangle) which can be moved, added, or removed at runtime.
///
/// The NavigationGrid handles:
/// - Pathfinding using A* algorithm with customizable heuristics
/// - Multiple target pathfinding with optional weights
/// - Dynamic collision detection through SpatialGrid system
/// - Custom node weights for path calculations
/// </summary>
public class NavigationGrid {
    private SpatialGrid BlockZones { get; }

    /// <summary>
    /// Creates a new NavigationGrid with the specified spatial cell size
    /// </summary>
    public NavigationGrid(int spatialCellSize = 5) {
        BlockZones = new SpatialGrid(spatialCellSize);
    }

    /// <summary>
    /// Creates a new GridGraph which has attached the obstacles from this NavigationGrid
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="isBlockedFunc">A function to determine if a cell is walkable based on its position.</param>
    /// <param name="getWeightFunc">Optional function to determine the weight/cost of moving through each cell.
    ///     If null, all walkable cells have equal weight</param>
    public GridGraph CreateGridGraph(Rect2I bounds, Func<Vector2I, bool> isBlockedFunc, Func<Vector2I, float>? getWeightFunc = null) {
        return new GridGraph(
            bounds,
            pos => (isBlockedFunc?.Invoke(pos) ?? false) || IsBlocked(pos),
            getWeightFunc
        );
    }

    /// <summary>
    /// Creates a new GridGraph which has attached the obstacles from this NavigationGrid
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="isBlockedFunc">A function to determine if a cell is walkable based on its position.</param>
    /// <param name="getWeightFunc">Optional function to determine the weight/cost of moving through each cell.
    ///     If null, all walkable cells have equal weight</param>
    public GridGraph CreateGridGraph(int width, int height, Func<Vector2I, bool> isBlockedFunc, Func<Vector2I, float>? getWeightFunc = null) {
        return new GridGraph(
            width,
            height,
            pos => (isBlockedFunc?.Invoke(pos) ?? false) || IsBlocked(pos),
            getWeightFunc
        );
    }

    /// <summary>
    /// Creates a new Array2DGraph which has attached the obstacles from this NavigationGrid
    /// </summary>
    /// <param name="array2D"></param>
    /// <param name="isBlockedFunc">A function to determine if a cell is walkable based on its position.</param>
    /// <param name="getWeightFunc">Optional function to determine the weight/cost of moving through each cell.
    ///     If null, all walkable cells have equal weight</param>
    public Array2DGraph<T> CreateArray2DGraph<T>(Array2D<T> array2D, Func<Vector2I, T, bool> isBlockedFunc, Func<Vector2I, T, float>? getWeightFunc = null) {
        return new Array2DGraph<T>(
            array2D,
            (pos, value) => (isBlockedFunc?.Invoke(pos, value) ?? false) || IsBlocked(pos),
            getWeightFunc
        );
    }

    public bool IsBlocked(Vector2I position) {
        return BlockZones.IntersectPoint(position.X, position.Y);
    }

    public bool IsAccesible(Vector2I pos) {
        return !IsBlocked(pos);
    }

    /// <summary>
    /// Adds a point obstacle to the grid.
    /// The Point.Position can be updated, and it will be refreshed immediately in the grid.
    /// The Point can be deleted with RemoveObstacle()
    /// </summary>
    /// <param name="position">The position of the point obstacle</param>
    /// <returns>The created Point shape that can be moved or removed later</returns>
    public Point AddObstacle(Vector2I position) {
        var point = new Point(position.X, position.Y);
        BlockZones.Add(point);
        return point;
    }

    /// <summary>
    /// Adds a circular obstacle to the grid
    /// The Circle.Position and Radius can be updated, and it will be refreshed immediately in the grid.
    /// The Circle can be deleted with RemoveObstacle()
    /// </summary>
    /// <param name="position">The center position of the circle</param>
    /// <param name="radius">The radius of the circle</param>
    /// <returns>The created Circle shape that can be moved, resized or removed later</returns>
    public Circle AddObstacle(Vector2I position, float radius) {
        var circle = new Circle(position.X, position.Y, radius);
        BlockZones.Add(circle);
        return circle;
    }

    /// <summary>
    /// Adds a rectangular obstacle to the grid
    /// The Rectangle.Position and Size can be updated, and it will be refreshed immediately in the grid.
    /// The Rectangle can be deleted with RemoveObstacle()
    /// </summary>
    /// <param name="position">The top-left position of the rectangle</param>
    /// <param name="size">The size of the rectangle</param>
    /// <returns>The created Circle shape that can be moved, resized or removed later</returns>
    public Rectangle AddObstacle(Vector2I position, Vector2I size) {
        var circle = new Rectangle(position, size);
        BlockZones.Add(circle);
        return circle;
    }

    public void RemoveObstacle(Shape shape) {
        BlockZones.Remove(shape);
    }

    public void ClearObstacles() {
        BlockZones.RemoveAll();
    }
}