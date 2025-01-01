using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;

public static class MazeGraphGrowExtensions {
    /// <summary>
    /// Grows a maze from a starting position using the specified constraints.
    /// </summary>
    /// <param name="maze">The MazeGraph instance</param>
    /// <param name="start">Starting position for the maze generation.</param>
    /// <param name="constraints">Constraints for the maze generation.</param>
    /// <param name="backtracker">A function to locate the next node to backtrack. By default, it takes the last one (LIFO)</param>
    /// <returns>The number of paths created.</returns>
    public static void Grow(this MazeGraph maze, Vector2I start, BacktrackConstraints constraints, Func<List<MazeNode>, MazeNode>? backtracker = null) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (!maze.IsValidPosition(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }

        var maxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var maxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var maxTotalPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        if (maxTotalCells == 0 || maxCellsPerPath == 0 || maxTotalPaths == 0) return;

        var pendingNodes = new List<MazeNode>();
        Vector2I? lastDirection = null;

        var pathsCreated = 0;
        var totalNodesCreated = 1;
        var nodesCreatedInCurrentPath = 1;

        var currentNode = maze.CreateNode(start);
        pendingNodes.Add(currentNode);
        while (pendingNodes.Count > 0) {
            var availableDirections = maze.GetValidFreeAdjacentDirections(currentNode.Position).ToList();

            if (availableDirections.Count == 0 || nodesCreatedInCurrentPath >= maxCellsPerPath || totalNodesCreated == maxTotalCells) {
                // path stopped, backtracking
                pendingNodes.Remove(currentNode);
                if (pendingNodes.Count > 0) {
                    currentNode = backtracker != null ? backtracker.Invoke(pendingNodes) : pendingNodes[^1];
                    if (!maze.GetValidFreeAdjacentDirections(currentNode.Position).Any()) {
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
            var newNode = maze.CreateNode(nextPos, currentNode);
            currentNode.ConnectTo(newNode);
            newNode.ConnectTo(currentNode);
            pendingNodes.Add(newNode);
            totalNodesCreated++;
            nodesCreatedInCurrentPath++;

            currentNode = newNode;
        }
        // Console.WriteLine("Cells created: " + totalNodesCreated + " Paths created: " + pathsCreated);
    }

    public static void GrowRandom(this MazeGraph maze, Vector2I start, int maxTotalNodes = -1, Random? rng = null) {
        if (maxTotalNodes == 0) return;
        if (!maze.IsValidPosition(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        maxTotalNodes = maxTotalNodes == -1 ? int.MaxValue : maxTotalNodes;

        rng ??= new Random();
        var pendingNodes = new List<MazeNode>();
        var totalNodesCreated = 1;
        var root = maze.CreateNode(start);
        pendingNodes.Add(root);
        while (pendingNodes.Count > 0 && totalNodesCreated < maxTotalNodes) {
            var currentNode = rng.Next(pendingNodes);
            var adjacentPositions = maze.GetAvailableAdjacentPositions(currentNode.Position).ToList();

            if (adjacentPositions.Count == 0) {
                // invalid cell, removing
                pendingNodes.Remove(currentNode);
            } else {
                var nextPos = rng.Next(adjacentPositions);
                var newNode = maze.CreateNode(nextPos, currentNode);
                currentNode.ConnectTo(newNode);
                newNode.ConnectTo(currentNode);
                pendingNodes.Add(newNode);
                totalNodesCreated++;
            }
        }
    }
}