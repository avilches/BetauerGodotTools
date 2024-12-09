using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class PotentialCycles {
    private readonly bool _useParentDistance;
    private readonly MazeGraph _graph; 
    private readonly List<(MazeNode nodeA, MazeNode nodeB, (Vector2I, Vector2I) connection)> _potentialConnections;

    public PotentialCycles(MazeGraph graph, bool useParentDistance) {
        _graph = graph;
        _useParentDistance = useParentDistance;
        // Get all potential cycles once
        _potentialConnections = graph.GetNodes()
            .SelectMany(nodeA =>
                graph.GetAdjacentNodes(nodeA.Position)
                    .Where(nodeB => !nodeA.HasEdgeTo(nodeB))
                    .Select(nodeB => {
                        var posA = nodeA.Position;
                        var posB = nodeB.Position;
                        var connection = posA.X < posB.X || (posA.X == posB.X && posA.Y < posB.Y)
                            ? (posA, posB)
                            : (posB, posA);
                        return (nodeA, nodeB, connection);
                    }))
            .DistinctBy(x => x.connection)
            .ToList();
    }

    /// <summary>
    /// Base method that applies filtering and ordering to potential cycles
    /// </summary>
    /// <param name="filter">Function to filter cycles based on distance</param>
    /// <param name="descending">If true, orders cycles from longest to shortest distance. If false, shortest to longest.</param>
    private IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> GetCyclesWithFilter(
        Func<int, bool> filter,
        bool descending = true) {
        var query = _potentialConnections
            .Where(x=> _graph.IsValidEdge(x.nodeA.Position, x.nodeB.Position))
            .Select(x => {
                var distance = _useParentDistance
                    ? x.nodeA.GetDistanceToNode(x.nodeB)
                    : x.nodeA.GetDistanceToNodeByEdges(x.nodeB);
                return (x.nodeA, x.nodeB, distance);
            })
            .Where(x => filter(x.distance));

        return descending ? query.OrderByDescending(x => x.distance) : query.OrderBy(x => x.distance);
    }

    public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> GetAllCycles(bool descending = true) {
        return GetCyclesWithFilter(_ => true, descending);
    }

    /// <summary>
    /// Returns potential cycles with distance greater than or equal to the specified minimum,
    /// ordered by distance.
    /// </summary>
    /// <param name="min">Minimum distance (inclusive)</param>
    /// <param name="descending">If true (default), orders from longest to shortest. If false, shortest to longest.</param>
    /// <returns>Sequence of potential cycles matching the criteria</returns>
    public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> GetCyclesGreaterThan(int min, bool descending = true) {
        return GetCyclesWithFilter(distance => distance >= min, descending);
    }

    /// <summary>
    /// Returns potential cycles with distance less than the specified maximum,
    /// ordered by distance.
    /// </summary>
    /// <param name="max">Maximum distance (exclusive)</param>
    /// <param name="descending">If true (default), orders from longest to shortest. If false, shortest to longest.</param>
    /// <returns>Sequence of potential cycles matching the criteria</returns>
    public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> GetCyclesLessThan(int max, bool descending = true) {
        return GetCyclesWithFilter(distance => distance <= max, descending);
    }

    /// <summary>
    /// Returns potential cycles with distance within the specified range,
    /// ordered by distance.
    /// </summary>
    /// <param name="min">Minimum distance (inclusive)</param>
    /// <param name="max">Maximum distance (exclusive)</param>
    /// <param name="descending">If true (default), orders from longest to shortest. If false, shortest to longest.</param>
    /// <returns>Sequence of potential cycles matching the criteria</returns>
    public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> GetCyclesBetween(int min, int max, bool descending = true) {
        return GetCyclesWithFilter(distance => distance >= min && distance < max, descending);
    }

    /// <summary>
    /// Returns potential cycles with exactly the specified distance,
    /// ordered by node positions.
    /// </summary>
    /// <param name="distance">The exact distance to match</param>
    /// <param name="descending">If true (default), orders from longest to shortest. If false, shortest to longest.</param>
    /// <returns>Sequence of potential cycles matching the criteria</returns>
    public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> GetCyclesEquals(int distance, bool descending = true) {
        return GetCyclesWithFilter(d => d == distance, descending);
    }

    /// <summary>
    /// Removes a specific cycle from the potential cycles collection.
    /// This is useful when you've used a cycle and want to prevent it from appearing in future queries.
    /// </summary>
    /// <param name="nodeA">First node of the cycle</param>
    /// <param name="nodeB">Second node of the cycle</param>
    public void RemoveCycle(MazeNode nodeA, MazeNode nodeB) {
        _potentialConnections.RemoveAll(x =>
            (x.nodeA == nodeA && x.nodeB == nodeB) ||
            (x.nodeA == nodeB && x.nodeB == nodeA));
    }
}