using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class MazeGraph(int width, int height, Func<Vector2I, bool>? isValidNode = null, Action<NodeGrid>? onCreateNode = null, Action<NodeGridEdge>? onConnect = null)
    : BaseMazeGraph(width, height, isValidNode, onCreateNode, onConnect) {
    /// <summary>
    /// Grows a maze from a starting position using the specified constraints.
    /// </summary>
    /// <param name="start">Starting position for the maze generation.</param>
    /// <param name="constraints">Constraints for the maze generation.</param>
    /// <param name="backtracker">A function to locate the next node to backtrack. By default, it takes the last one (LIFO)</param>
    /// <returns>The number of paths created.</returns>
    public void Grow(Vector2I start, BacktrackConstraints constraints, Func<List<NodeGrid>, NodeGrid>? backtracker = null) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (!IsValidNode(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        LastId = 0;

        var maxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var maxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var maxTotalPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        if (maxTotalCells == 0 || maxCellsPerPath == 0 || maxTotalPaths == 0) return;

        var pendingNodes = new List<NodeGrid>();
        Vector2I? lastDirection = null;

        var pathsCreated = 0;
        var totalNodesCreated = 1;
        var nodesCreatedInCurrentPath = 1;

        var currentNode = NodeGridRoot = CreateNode(start);
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
            var nextNode = CreateNode(nextPos);
            nextNode.Parent = currentNode;
            ConnectNode(currentNode, nextDir, true);
            pendingNodes.Add(nextNode);
            totalNodesCreated++;
            nodesCreatedInCurrentPath++;

            currentNode = nextNode;
        }
        // Console.WriteLine("Cells created: " + totalNodesCreated + " Paths created: " + pathsCreated);
    }

    public void GrowRandom(Vector2I start, int maxTotalNodes = -1, Random? rng = null) {
        if (maxTotalNodes == 0) return;
        if (!IsValidNode(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        maxTotalNodes = maxTotalNodes == -1 ? int.MaxValue : maxTotalNodes;

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        LastId = 0;
        rng ??= new Random();
        var pendingNodes = new List<NodeGrid>();
        var totalNodesCreated = 1;
        NodeGridRoot = CreateNode(start);
        pendingNodes.Add(NodeGridRoot);
        while (pendingNodes.Count > 0 && totalNodesCreated < maxTotalNodes) {
            var currentNode = rng.Next(pendingNodes);
            var availableDirections = GetAvailableDirections(currentNode.Position);

            if (availableDirections.Count == 0) {
                // invalid cell, removing
                pendingNodes.Remove(currentNode);
            } else {
                var nextDir = rng.Next(availableDirections);
                var nextPos = currentNode.Position + nextDir;
                var nextNode = CreateNode(nextPos);
                nextNode.Parent = currentNode;
                ConnectNode(currentNode, nextDir, true);
                pendingNodes.Add(nextNode);
                totalNodesCreated++;
            }
        }
    }

    
}