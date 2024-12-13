using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

public partial class MazeGraph {
    /// <summary>
    /// Grows a maze from a starting position using the specified constraints.
    /// </summary>
    /// <param name="start">Starting position for the maze generation.</param>
    /// <param name="constraints">Constraints for the maze generation.</param>
    /// <param name="backtracker">A function to locate the next node to backtrack. By default, it takes the last one (LIFO)</param>
    /// <returns>The number of paths created.</returns>
    public void Grow(Vector2I start, BacktrackConstraints constraints, Func<List<MazeNode>, MazeNode>? backtracker = null) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (!IsValidPosition(start)) {
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

        var currentNode = Root = CreateNode(start);
        pendingNodes.Add(Root);
        while (pendingNodes.Count > 0) {
            var availableDirections = GetValidFreeAdjacentDirections(currentNode.Position).ToList();

            if (availableDirections.Count == 0 || nodesCreatedInCurrentPath >= maxCellsPerPath || totalNodesCreated == maxTotalCells) {
                // path stopped, backtracking
                pendingNodes.Remove(currentNode);
                if (pendingNodes.Count > 0) {
                    currentNode = backtracker != null ? backtracker.Invoke(pendingNodes) : pendingNodes[^1];
                    if (!GetValidFreeAdjacentDirections(currentNode.Position).Any()) {
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
            var newNode = CreateNode(nextPos, currentNode);
            currentNode.ConnectTo(newNode);
            newNode.ConnectTo(currentNode);
            pendingNodes.Add(newNode);
            totalNodesCreated++;
            nodesCreatedInCurrentPath++;

            currentNode = newNode;
        }
        // Console.WriteLine("Cells created: " + totalNodesCreated + " Paths created: " + pathsCreated);
    }

    public void GrowRandom(Vector2I start, int maxTotalNodes = -1, Random? rng = null) {
        if (maxTotalNodes == 0) return;
        if (!IsValidPosition(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        maxTotalNodes = maxTotalNodes == -1 ? int.MaxValue : maxTotalNodes;

        rng ??= new Random();
        var pendingNodes = new List<MazeNode>();
        var totalNodesCreated = 1;
        Root = CreateNode(start);
        pendingNodes.Add(Root);
        while (pendingNodes.Count > 0 && totalNodesCreated < maxTotalNodes) {
            var currentNode = rng.Next(pendingNodes);
            var adjacentPositions = GetAvailableAdjacentPositions(currentNode.Position).ToList();

            if (adjacentPositions.Count == 0) {
                // invalid cell, removing
                pendingNodes.Remove(currentNode);
            } else {
                var nextPos = rng.Next(adjacentPositions);
                var newNode = CreateNode(nextPos, currentNode);
                currentNode.ConnectTo(newNode);
                newNode.ConnectTo(currentNode);
                pendingNodes.Add(newNode);
                totalNodesCreated++;
            }
        }
    }

    /// <summary>
    /// Creates a new MazeGraph with the specified dimensions.
    /// </summary>
    public static MazeGraph Create(int width, int height) {
        return new MazeGraph {
            IsValidPositionFunc = pos => Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, width, height)
        };
    }

    /// <summary>
    /// Creates a MazeGraph from a boolean template array.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(bool[,] template) {
        return new MazeGraph {
            IsValidPositionFunc = pos =>
                Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, template.GetLength(1), template.GetLength(0))
                && template[pos.Y, pos.X]
        };
    }

    /// <summary>
    /// Creates a MazeGraph from a boolean Array2D template.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(Array2D<bool> template) {
        return new MazeGraph {
            IsValidPositionFunc = pos =>
                Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, template.Width, template.Height)
                && template[pos]
        };
    }

    /// <summary>
    /// Creates a MazeGraph from a BitArray2D template.
    /// In the template, true values represent valid positions for nodes. false values are forbidden (invalid)
    /// </summary>
    public static MazeGraph Create(BitArray2D template) {
        return new MazeGraph { //(template.Width, template.Height) {
            IsValidPositionFunc = pos =>
                Geometry.IsPointInRectangle(pos.X, pos.Y, 0, 0, template.Width, template.Height)
                && template[pos]
        };
    }
}