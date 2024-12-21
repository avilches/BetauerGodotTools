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
/// The AiGrid handles:
/// - Pathfinding using A* algorithm with customizable heuristics
/// - Multiple target pathfinding with optional weights
/// - Dynamic collision detection through SpatialGrid system
/// - Custom node weights for path calculations
/// </summary>
/// <typeparam name="T">The type of data stored in each grid cell</typeparam>
public class NavigationGrid<T> {
    public Array2DGraph<T> Graph { get; }
    private SpatialGrid BlockZones { get; }

    /// <summary>
    /// Creates a new AiGrid with the specified grid and optional weight function
    /// </summary>
    /// <param name="grid">The 2D grid that defines the space</param>
    /// <param name="getWeight">Optional function to determine the weight/cost of moving through each cell.
    /// If null, all walkable cells have equal weight</param>
    /// <param name="isBlockedFunc">Optional function to determine if a cell is walkable based on its position.</param>
    /// <param name="spatialCellSize">The width/height size of the cell to determine the collision for the obstacles</param>
    public NavigationGrid(Array2D<T> grid, Func<Vector2I, float>? getWeight = null, Func<Vector2I, bool>? isBlockedFunc = null, int spatialCellSize = 5) {
        BlockZones = new SpatialGrid(spatialCellSize);
        Graph = new Array2DGraph<T>(
            grid,
            getWeight,
            pos => 
                (isBlockedFunc != null && isBlockedFunc.Invoke(pos)) || 
                BlockZones.IntersectPoint(pos.X, pos.Y)
        );
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