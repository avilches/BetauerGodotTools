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
public class AiGrid<T> {
    private const float SPATIAL_CELL_SIZE = 5f;

    public Array2DGraph<T> Array2DGraph { get; }
    private SpatialGrid BlockZones { get; }

    /// <summary>
    /// Creates a new AiGrid with the specified grid and optional weight function
    /// </summary>
    /// <param name="grid">The 2D grid that defines the space</param>
    /// <param name="getWeight">Optional function to determine the weight/cost of moving through each cell.
    /// If null, all walkable cells have equal weight</param>
    /// <param name="isWalkablePositionFunc">Optional function to determine if a cell is walkable based on its position.</param>
    public AiGrid(Array2D<T> grid, Func<Vector2I, float>? getWeight = null, Func<Vector2I, bool>? isWalkablePositionFunc = null) {
        BlockZones = new SpatialGrid(SPATIAL_CELL_SIZE);
        Array2DGraph = new Array2DGraph<T>(
            grid,
            getWeight,
            pos => 
                (isWalkablePositionFunc == null || isWalkablePositionFunc.Invoke(pos))
                && !BlockZones.IntersectPoint(pos.X, pos.Y)
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

    /// <summary>
    /// Finds a path from start to end position using A* algorithm with Euclidean heuristic
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="end">Target position</param>
    /// <param name="onNodeVisited">Optional callback that is invoked for each node visited during pathfinding</param>
    /// <returns>A list of positions representing the path, or an empty list if no path is found</returns>
    public IReadOnlyList<Vector2I> FindPath(Vector2I start, Vector2I end, Action<Vector2I>? onNodeVisited = null) {
        return Array2DGraph.FindPath(start, end, Heuristics.Euclidean, onNodeVisited);
    }

    /// <summary>
    /// Finds a path to the closest target based on Euclidean distance. Note that this method only considers
    /// the direct (straight line) distance to the targets, not the actual path length.
    /// 
    /// IMPORTANT: The selected target might not result in the shortest actual path! For example:
    /// - Target A might be closer in direct distance (e.g., 5 units away)
    /// - But requires a long path around obstacles (e.g., 20 steps to reach it)
    /// - While Target B might be further away (e.g., 8 units)
    /// - But has a clear path (e.g., only 8 steps to reach it)
    /// 
    /// For weighted targets, use the overload that accepts List<(Vector2I pos, float weight)>.
    /// If you need the truly shortest path, use FindShortestPath instead.
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="targets">List of potential target positions</param>
    /// <returns>A path to the closest target by Euclidean distance, or an empty list if no path is found</returns>
    public IReadOnlyList<Vector2I> FindNearestPath(Vector2I start, List<Vector2I> targets) {
        if (targets.Count == 0) return Array.Empty<Vector2I>();

        // Encontrar el mejor target basado en distancia solo
        var bestTarget = targets
            .OrderBy(t => {
                var distance = Heuristics.Euclidean(start, t);
                return distance;
            })
            .First();

        return Array2DGraph.FindPath(start, bestTarget, Heuristics.Euclidean);
    }


    /// <summary>
    /// Finds a path to the closest target based on Euclidean distance and target weights. 
    /// Higher weights make targets more attractive (shorter effective distance).
    /// The effective distance is calculated as: actual_distance / weight
    /// 
    /// For example, with two targets:
    /// - Target A: distance = 10, weight = 1.0 → effective distance = 10/1.0 = 10
    /// - Target B: distance = 15, weight = 2.0 → effective distance = 15/2.0 = 7.5
    /// In this case, Target B would be chosen despite being physically further away.
    /// 
    /// IMPORTANT: This method only considers the direct (straight line) distance and weights,
    /// not the actual path length! For example:
    /// - Target A might be closer (5 units, weight 1.0)
    /// - But requires a long path around obstacles (20 steps)
    /// - While Target B might be further (8 units, weight 1.0)
    /// - But has a clear path (8 steps)
    /// 
    /// For unweighted pathfinding, use the overload that accepts List<Vector2I>.
    /// If you need the truly shortest path considering weights, use FindShortestPath instead.
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="targets">List of target positions and their weights. Higher weights make targets more attractive</param>
    /// <returns>A path to the target with the lowest effective distance (distance/weight), or an empty list if no path is found</returns>
    public IReadOnlyList<Vector2I> FindNearestPath(Vector2I start, List<(Vector2I pos, float weight)> targets) {
        if (targets.Count == 0) return Array.Empty<Vector2I>();

        // Encontrar el mejor target basado en distancia y peso
        var bestTarget = targets
            .OrderBy(t => {
                var distance = Heuristics.Euclidean(start, t.pos);
                return distance / t.weight; // Menor distancia y mayor peso = mejor
            })
            .First();

        return Array2DGraph.FindPath(start, bestTarget.pos, Heuristics.Euclidean);
    }

    /// <summary>
    /// Finds the shortest actual path to any of the weighted targets
    /// Unlike FindNearestPath, this method calculates complete paths to all targets
    /// to find the truly shortest path, not just the closest target by distance
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="targets">List of potential target positions and their weights</param>
    /// <returns>The shortest possible path to any target, or an empty list if no path is found</returns>
    public IReadOnlyList<Vector2I> FindShortestPath(Vector2I start, List<Vector2I> targets) {
        if (targets.Count == 0) return Array.Empty<Vector2I>();

        IReadOnlyList<Vector2I>? shortestPath = null;
        var shortestPathLength = float.MaxValue;

        var astar = new Array2DAStar<T>(Array2DGraph);

        // Find paths to all targets and keep the shortest one
        foreach (var targetPos in targets) {
            var path = astar.FindPath(start, targetPos, Heuristics.Euclidean);
            if (path.Count == 0) continue;
            var length = path.Count;
            // Update if this is the shortest path so far
            if (length < shortestPathLength) {
                shortestPath = path;
                shortestPathLength = length;
            }
        }

        return shortestPath ?? Array.Empty<Vector2I>();
    }

    /// <summary>
    /// Finds the shortest actual path to any of the weighted targets, considering both path length and target weights.
    /// Unlike FindNearestPath, this method calculates complete paths to all targets
    /// to find the truly shortest effective path, not just the closest target by straight-line distance.
    /// 
    /// The effective path length is calculated as: actual_path_length / weight
    /// Higher weights make targets more attractive (shorter effective length).
    /// 
    /// For example, with two targets:
    /// - Target A: path length = 10 steps, weight = 1.0 → effective length = 10/1.0 = 10
    /// - Target B: path length = 15 steps, weight = 2.0 → effective length = 15/2.0 = 7.5
    /// In this case, the path to Target B would be chosen as it has the shortest effective length.
    /// 
    /// When two paths have the same effective length, the one with the higher weight is chosen.
    /// For unweighted pathfinding, use the overload that accepts List<Vector2I>.
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="targets">List of target positions and their weights. Higher weights make targets more attractive</param>
    /// <returns>The path with the shortest effective length (path_length/weight), or an empty list if no path is found</returns>
    public IReadOnlyList<Vector2I> FindShortestPath(Vector2I start, List<(Vector2I pos, float weight)> targets) {
        if (targets.Count == 0) return Array.Empty<Vector2I>();

        IReadOnlyList<Vector2I>? shortestPath = null;
        var shortestPathLength = float.MaxValue;
        var bestTargetWeight = 0f;

        var astar = new Array2DAStar<T>(Array2DGraph);

        // Find paths to all targets and keep the shortest one
        foreach (var (targetPos, weight) in targets) {
            var path = astar.FindPath(start, targetPos, Heuristics.Euclidean);
            if (path.Count == 0) continue;

            // Calculate the effective path length considering the target's weight
            // Shorter paths and higher weights are preferred
            var effectiveLength = path.Count / weight;

            // Update if this is the shortest path so far or if it's the same length but with a better weight
            if (effectiveLength < shortestPathLength ||
                (effectiveLength == shortestPathLength && weight > bestTargetWeight)) {
                shortestPath = path;
                shortestPathLength = effectiveLength;
                bestTargetWeight = weight;
            }
        }

        return shortestPath ?? Array.Empty<Vector2I>();
    }
}