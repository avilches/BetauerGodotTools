using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze;

public partial class MazeGraph {
    /// <summary>
    /// Grows a maze from a starting position using the specified constraints.
    /// </summary>
    /// <param name="start">Starting position for the maze generation.</param>
    /// <param name="constraints">Constraints for the maze generation.</param>
    /// <param name="backtracker">A function to locate the next cell to backtrack. By default, it takes the last one (LIFO)</param>
    /// <returns>The number of paths created.</returns>
    public void Grow(Vector2I start, BacktrackConstraints constraints, Func<List<NodeGrid>, NodeGrid>? backtracker = null) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        _lastId = 0;

        var maxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var maxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var maxTotalPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        if (maxTotalCells == 0 || maxCellsPerPath == 0 || maxTotalPaths == 0) return;

        var pendingNodes = new List<NodeGrid>();
        Vector2I? lastDirection = null;

        var pathsCreated = 0;
        var totalNodesCreated = 1;
        var nodesCreatedInCurrentPath = 1;

        var currentNode = NodeGridRoot = GetOrCreateNode(start);
        pendingNodes.Add(NodeGridRoot);
        while (pendingNodes.Count > 0) {
            var availableDirections = GetAvailableDirections(currentNode.Position);

            if (availableDirections.Count == 0 || nodesCreatedInCurrentPath >= maxCellsPerPath || totalNodesCreated == maxTotalCells) {
                // path stopped, backtracking
                pendingNodes.Remove(currentNode);
                if (pendingNodes.Count > 0) {
                    currentNode = backtracker != null ? backtracker.Invoke(pendingNodes) : pendingNodes[pendingNodes.Count - 1];
                    if (GetAvailableDirections(currentNode.Position).Count == 0) {
                        continue;
                    }
                }
                // No more nodes to backtrack (end of the path and the maze) or next node has no available directions (end of the path)
                pathsCreated++;
                // Console.WriteLine($"Path #{pathsCreated} finished: Cells: {nodesCreatedInCurrentPath}");
                if (pathsCreated == maxTotalPaths || totalNodesCreated == maxTotalCells) break;
                nodesCreatedInCurrentPath = 1;
                lastDirection = null;
                continue;
            }

            var validCurrentDir = lastDirection.HasValue && availableDirections.Contains(lastDirection.Value)
                ? lastDirection
                : null;

            var nextDir = constraints.DirectionSelector(validCurrentDir, availableDirections);
            lastDirection = nextDir;

            var nextPos = currentNode.Position + nextDir;
            var nextNode = GetOrCreateNode(nextPos);
            nextNode.Parent = currentNode;
            ConnectNode(currentNode, nextDir, true);
            pendingNodes.Add(nextNode);
            totalNodesCreated++;
            nodesCreatedInCurrentPath++;

            currentNode = nextNode;
        }
        // Console.WriteLine("Cells created: " + totalNodesCreated + " Paths created: " + pathsCreated);
    }

    public void GrowRandom(Vector2I start, int maxTotalCells = -1, Random? rng = null) {
        if (maxTotalCells == 0) return;
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));
        maxTotalCells = maxTotalCells == -1 ? int.MaxValue : maxTotalCells;

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        _lastId = 0;
        rng ??= new Random();
        var pendingNodes = new List<NodeGrid>();
        var totalNodesCreated = 1;
        NodeGridRoot = GetOrCreateNode(start);
        pendingNodes.Add(NodeGridRoot);
        while (pendingNodes.Count > 0 && totalNodesCreated < maxTotalCells) {
            var currentNode = rng.Next(pendingNodes);
            var availableDirections = GetAvailableDirections(currentNode.Position);

            if (availableDirections.Count == 0) {
                // invalid cell, removing
                pendingNodes.Remove(currentNode);
            } else {
                var nextDir = rng.Next(availableDirections);
                var nextPos = currentNode.Position + nextDir;
                var nextNode = GetOrCreateNode(nextPos);
                nextNode.Parent = currentNode;
                ConnectNode(currentNode, nextDir, true);
                pendingNodes.Add(nextNode);
                totalNodesCreated++;
            }
        }
    }

    public void GrowZoned(Vector2I start, Func<int, int> cellsPerZone, int maxTotalCells = -1, Random? rng = null) {
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        _lastId = 0;

        if (maxTotalCells == 0) return;
        maxTotalCells = maxTotalCells == -1 ? int.MaxValue : maxTotalCells;
        rng ??= new Random();

        var pendingNodes = new List<NodeGrid>();

        var totalNodesCreated = 1;

        NodeGridRoot = GetOrCreateNode(start);
        NodeGridRoot.Metadata = 0;
        pendingNodes.Add(NodeGridRoot);
        var currentZone = 0;
        var pendingNodesPerZone = new List<List<NodeGrid>> { new() { NodeGridRoot } };
        var nodesPerZone = new List<int> { 1 };
        while (pendingNodes.Count > 0 && totalNodesCreated < maxTotalCells) {
            var currentNode = pendingNodesPerZone[currentZone].Count == 0
                ? rng.Next(pendingNodes)
                : rng.Next(pendingNodesPerZone[currentZone]);
            var availableDirections = GetAvailableDirections(currentNode.Position);

            if (availableDirections.Count == 0) {
                // invalid cell, removing
                pendingNodes.Remove(currentNode);
                pendingNodesPerZone[currentZone].Remove(currentNode);
            } else {
                var nextDir = rng.Next(availableDirections);
                var nextPos = currentNode.Position + nextDir;
                var nextNode = GetOrCreateNode(nextPos);
                nextNode.Parent = currentNode;
                nextNode.Metadata = currentZone;
                ConnectNode(currentNode, nextDir, true);
                pendingNodes.Add(nextNode);
                pendingNodesPerZone[currentZone].Add(nextNode);
                nodesPerZone[currentZone] += 1;
                totalNodesCreated++;
                if (nodesPerZone[currentZone] >= cellsPerZone(currentZone)) {
                    currentZone++;
                    pendingNodesPerZone.Add([]);
                    nodesPerZone.Add(0);
                }
            }
        }
    }
}