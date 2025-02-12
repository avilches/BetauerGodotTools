using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.GridTools;

/// <summary>
/// The Array2DEdge struct represents a weighted edge in an edge-weighted grid graph. 
/// </summary>
public readonly record struct Array2DEdge(Vector2I From, Vector2I To, float Weight) {
    public override string ToString() {
        return $"From: {From}, To: {To}, Weight: {Weight}";
    }
}

/// <summary>
/// The Array2DGraph class represents an edge-weighted directed graph based on a grid where vertices are Vector2I coordinates.
/// The graph structure is implicit in the grid, where each cell can be connected to its orthogonal neighbors if they are walkable.
/// Edge weights are determined by the cost function provided.
/// </summary>
public class Array2DGraph<T> {
    public Array2D<T> Array2D { get; }
    public Func<Vector2I, float>? GetWeightFunc { get; set; }
    public Func<Vector2I, bool>? IsBlockedFunc { get; set; }

    /// <summary>
    /// Weight multiplier for diagonal movements.
    /// - -1: diagonal movement disabled
    /// - 1.0: diagonal movement costs the same as orthogonal
    /// - 1.414 (√2): physically accurate diagonal movement
    /// </summary>
    public float DiagonalWeight { get; private set; } = -1;

    public int Width => Array2D.Width;
    public int Height => Array2D.Height;

    /// <summary>
    /// Constructs a grid graph from an Array2D with walkability and weight functions
    /// </summary>
    /// <param name="array2D">The grid that defines the graph structure</param>
    /// <param name="getWeightFunc">Function that determines the movement cost for a cell (must be >= 1). If null, all weights will be 1</param>
    /// <param name="isBlockedFunc">Function that determines if a cell is not accesible based on its position. If null, all cells will be accesible</param>
    public Array2DGraph(Array2D<T> array2D, Func<Vector2I, float>? getWeightFunc = null, Func<Vector2I, bool>? isBlockedFunc = null) {
        Array2D = array2D ?? throw new ArgumentNullException(nameof(array2D));
        GetWeightFunc = getWeightFunc;
        IsBlockedFunc = isBlockedFunc;
    }

    /// <summary>
    /// Enables diagonal movement with same cost as orthogonal movement (weight = 1.0)
    /// This means moving diagonally will cost the same as moving horizontally or vertically
    /// </summary>
    public void EnableDiagonalMovement(bool enabled = true) {
        DiagonalWeight = enabled ? 1f : -1f;
    }

    /// <summary>
    /// Enables diagonal movement with physically accurate cost (weight = √2 ≈ 1.414)
    /// This provides more realistic pathfinding as diagonal distance is actually longer
    /// than orthogonal distance
    /// </summary>
    public void EnablePhysicalDiagonalMovement() {
        DiagonalWeight = 1.414f; // √2
    }

    /// <summary>
    /// Enables diagonal movement with a custom weight
    /// Higher weights make diagonal movement more expensive
    /// </summary>
    /// <param name="weight">The weight for diagonal movement. Must be >= 1.0</param>
    /// <exception cref="ArgumentException">If weight is less than 1.0</exception>
    public void SetDiagonalMovementWeight(float weight) {
        if (weight < 1.0f) throw new ArgumentException("Diagonal weight must be >= 1.0", nameof(weight));
        DiagonalWeight = weight;
    }

    /// <summary>
    /// Disables diagonal movement completely
    /// Pathfinding will only use orthogonal movements (up, down, left, right)
    /// </summary>
    public void DisableDiagonalMovement() {
        DiagonalWeight = -1f;
    }

    /// <summary>
    /// Returns whether diagonal movement is currently enabled
    /// </summary>
    /// <returns>true if diagonal movement is enabled, false otherwise</returns>
    public bool IsDiagonalMovementEnabled() {
        return DiagonalWeight > 0;
    }

    /// <summary>
    /// Returns an IEnumerable of the Array2DEdges incident from the specified vertex
    /// The edges are computed on-demand based on the walkable orthogonal neighbors
    /// </summary>
    /// <param name="vertex">The vertex to find incident Array2DEdges from</param>
    /// <returns>IEnumerable of the Array2DEdges incident from the specified vertex</returns>
    public IEnumerable<Array2DEdge> Adjacent(Vector2I vertex) {
        if (IsBlocked(vertex)) yield break;

        // Orthogonal movements
        foreach (var neighbor in Array2D.GetVonNeumannPositions(vertex)) {
            if (IsAccesible(neighbor)) {
                var weight = GetWeight(neighbor);
                yield return new Array2DEdge(vertex, neighbor, weight);
            }
        }

        // Diagonal movements
        if (IsDiagonalMovementEnabled()) {
            foreach (var neighbor in Array2D.GetDiagonalPositions(vertex)) {
                if (IsAccesible(neighbor)) {
                    var cellWeight = GetWeight(neighbor);
                    var finalWeight = cellWeight * DiagonalWeight;
                    yield return new Array2DEdge(vertex, neighbor, finalWeight);
                }
            }
        }
    }

    public float GetWeight(Vector2I neighbor) {
        return GetWeightFunc?.Invoke(neighbor) ?? 1f;
    }

    /// <summary>
    /// Returns whether the specified position is valid and walkable in the grid
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>True if the position is valid and walkable, false otherwise</returns>
    public bool IsAccesible(Vector2I pos) {
        return !IsBlocked(pos);
    }
    
    /// <summary>
    /// Returns whether the specified position is blocked or invalid in the grid, so paths can't use it
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>True if the position is blocked or not value, false otherwise</returns>
    public bool IsBlocked(Vector2I pos) {
        return !Array2D.IsValidPosition(pos) || (IsBlockedFunc != null && IsBlockedFunc(pos));
    }

    /// <summary>
    /// Returns an IEnumerable of all directed edges in the edge-weighted grid graph
    /// The edges are computed on-demand based on the walkable cells and their walkable neighbors
    /// </summary>
    /// <returns>IEnumerable of all directed edges in the edge-weighted grid graph</returns>
    public IEnumerable<Array2DEdge> Edges() {
        for (var y = 0; y < Array2D.Height; y++) {
            for (var x = 0; x < Array2D.Width; x++) {
                var pos = new Vector2I(x, y);
                if (IsAccesible(pos)) {
                    foreach (var edge in Adjacent(pos)) {
                        yield return edge;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns the number of directed edges incident from the specified vertex
    /// This is known as the out-degree of the vertex
    /// </summary>
    /// <param name="vertex">The vertex to find the out-degree of</param>
    /// <returns>The number of directed edges incident from the specified vertex</returns>
    public int OutDegree(Vector2I vertex) {
        return IsAccesible(vertex) ? Array2D.GetVonNeumannPositions(vertex).Count(IsAccesible) : 0;
    }

    /// <summary>
    /// Finds a path from start to end position using A* algorithm (Euclidean heuristic by default)
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="target">Target position</param>
    /// <param name="heuristic">The heuristic function</param>
    /// <param name="onNodeVisited">Optional callback that is invoked for each node visited during pathfinding</param>
    /// <returns>A list of positions representing the path, or an empty list if no path is found</returns>
    public IReadOnlyList<Vector2I> FindPath(
        Vector2I start, Vector2I target,
        Func<Vector2I, Vector2I, float>? heuristic = null, Action<Vector2I>? onNodeVisited = null) {
        return new Array2DAStar<T>(this).FindPath(start, target, heuristic, onNodeVisited);
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
            .OrderBy(t => Heuristics.Euclidean(start, t))
            .First();

        return FindPath(start, bestTarget, Heuristics.Euclidean);
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

        return FindPath(start, bestTarget.pos, Heuristics.Euclidean);
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

        var astar = new Array2DAStar<T>(this);

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

        var astar = new Array2DAStar<T>(this);

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

    /// <summary>
    /// Returns all connected nodes that can be reached from the starting position,
    /// expanding in a circular pattern up to a maximum number of nodes.
    /// 
    /// The search uses a breadth-first approach, which means it will find all nodes
    /// at distance 1 before moving to distance 2, and so on, creating a circular-like expansion pattern.
    /// 
    /// If maxNodes is reached, the method will return only the nodes found so far.
    /// A value of -1 for maxNodes means no limit.
    /// 
    /// Note: The starting position is included in the result if it's walkable.
    /// </summary>
    /// <param name="start">The starting position for the search</param>
    /// <param name="maxNodes">Maximum number of nodes to return. Use -1 for unlimited</param>
    /// <returns>A HashSet containing all reachable positions within the limit</returns>
    public HashSet<Vector2I> GetReachableZone(Vector2I start, int maxNodes = -1) {
        var visited = new HashSet<Vector2I>();
        if (IsBlocked(start)) return visited;

        var queue = new Queue<Vector2I>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0 && (maxNodes == -1 || visited.Count < maxNodes)) {
            var current = queue.Dequeue();

            foreach (var edge in Adjacent(current)) {
                var neighbor = edge.To;
                if (visited.Add(neighbor)) {
                    queue.Enqueue(neighbor);

                    // Check limit after adding each node
                    if (maxNodes != -1 && visited.Count >= maxNodes) {
                        return visited;
                    }
                }
            }
        }

        return visited;
    }

    /// <summary>
    /// Returns all connected nodes that can be reached from the starting position within a specific distance.
    /// The distance is measured in steps (grid movements) from the starting position.
    /// 
    /// The search uses a breadth-first approach, which means it will find all nodes
    /// at distance 1 before moving to distance 2, and so on, creating a circular-like expansion pattern.
    /// 
    /// Note: The starting position is included in the result if it's walkable.
    /// </summary>
    /// <param name="start">The starting position for the search</param>
    /// <param name="maxDistance">Maximum distance (in steps) from the starting position</param>
    /// <returns>A HashSet containing all reachable positions within the distance limit</returns>
    public HashSet<Vector2I> GetReachableZoneInRange(Vector2I start, int maxDistance) {
        if (maxDistance < 0) throw new ArgumentException("maxDistance must be non-negative", nameof(maxDistance));

        var visited = new HashSet<Vector2I>();
        if (IsBlocked(start)) return visited;

        var queue = new Queue<(Vector2I pos, int distance)>();
        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0) {
            var (current, distance) = queue.Dequeue();

            // If we're at max distance, don't explore neighbors
            if (distance >= maxDistance) continue;

            foreach (var edge in Adjacent(current)) {
                var neighbor = edge.To;
                if (visited.Add(neighbor)) {
                    queue.Enqueue((neighbor, distance + 1));
                }
            }
        }

        return visited;
    }


    /// <summary>
    /// Returns a string that represents the current edge-weighted grid graph
    /// </summary>
    /// <returns>
    /// A string that represents the current edge-weighted grid graph
    /// </returns>
    public override string ToString() {
        var formattedString = new StringBuilder();
        formattedString.AppendLine($"Grid size: {Array2D.Width}x{Array2D.Height}");

        for (var y = 0; y < Array2D.Height; y++) {
            for (var x = 0; x < Array2D.Width; x++) {
                var pos = new Vector2I(x, y);
                if (IsAccesible(pos)) {
                    formattedString.Append($"{pos}:");
                    foreach (var edge in Adjacent(pos)) {
                        formattedString.Append($" {edge.To}");
                    }
                    formattedString.AppendLine();
                }
            }
        }
        return formattedString.ToString();
    }
}