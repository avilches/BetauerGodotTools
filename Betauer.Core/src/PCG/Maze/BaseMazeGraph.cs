using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents a maze as a graph structure with nodes and edges in a 2d grid.
/// </summary>
public class BaseMazeGraph {
    public int Width { get; }
    public int Height { get; }
    public Array2D<MazeNode> NodeGrid { get; }
    public Dictionary<int, MazeNode> Nodes { get; } = [];
    public MazeNode Root { get; protected set; }

    public Func<Vector2I, IEnumerable<Vector2I>> GetAdjacentPositions { get; set; }

    /// <summary>
    /// Called to determine if a position is valid before creating a node, rejecting with an exception if the position is not valid.
    /// Called to filter the adjacent positions of a node, ignoring invalid positions. 
    /// </summary>
    public Func<Vector2I, bool> IsValidPositionFunc { get; set; } = _ => true;

    /// <summary>
    /// Called to determine if an edge is valid before creating it.
    /// </summary>
    public Func<Vector2I, Vector2I, bool> IsValidEdgeFunc { get; set; } = (_, _) => true;

    public event Action<MazeEdge>? OnNodeConnected;
    public event Action<MazeNode>? OnNodeCreated;

    protected int LastId = 0;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// <param name="width">The width of the maze.</param>
    /// <param name="height">The height of the maze.</param>
    /// Optional, by default, it uses ortogonal positions up, down, left, right </param>
    protected BaseMazeGraph(int width, int height) {
        Width = width;
        Height = height;
        GetAdjacentPositions = GetOrtogonalPositions;
        NodeGrid = new Array2D<MazeNode>(width, height);
    }

    public bool IsValidPosition(Vector2I position) {
        return Geometry.IsPointInRectangle(position.X, position.Y, 0, 0, Width, Height) && IsValidPositionFunc(position);
    }

    public MazeNode GetNode(int id) {
        return Nodes[id];
    }

    public IReadOnlyCollection<MazeNode> GetNodes() {
        return Nodes.Values;
    }

    public MazeNode GetOrCreateNode(Vector2I position) {
        ValidatePosition(position, nameof(GetOrCreateNode));
        var node = NodeGrid[position];
        if (node != null) return node;

        return CreateNode(position, null);
    }

    private void ValidatePosition(Vector2I position, string message) {
        if (!Geometry.IsPointInRectangle(position.X, position.Y, 0, 0, Width, Height)) {
            throw new InvalidNodeException($"Invalid position {position} in {message}: position out of bounds (0, 0, {Width}, {Height})", position);
        }
        if (!IsValidPositionFunc(position)) {
            throw new InvalidNodeException($"Invalid position {position} in {message}: {nameof(IsValidPositionFunc)} returned false", position);
        }
    }

    public MazeNode CreateNode(Vector2I position, MazeNode? parent = null) {
        ValidatePosition(position, nameof(CreateNode));
        var node = NodeGrid[position];
        if (node != null) {
            throw new InvalidOperationException($"Can't create node at {position}: there is already a node there with id {node.Id}");
        }

        node = new MazeNode(LastId++, position) {
            Parent = parent
        };
        NodeGrid[position] = node;
        Nodes[node.Id] = node;
        OnNodeCreated?.Invoke(node);
        return node;
    }

    public MazeNode? GetNodeAt(Vector2I position) {
        ValidatePosition(position, nameof(GetNodeAt));
        return NodeGrid[position];
    }

    public MazeNode? GetNodeAtOrNull(Vector2I position) {
        if (!Geometry.IsPointInRectangle(position.X, position.Y, 0, 0, Width, Height) ||
            !IsValidPositionFunc(position)) return null;
        return NodeGrid[position];
    }

    public bool RemoveNode(int id) {
        if (!Nodes.TryGetValue(id, out var node)) return false;
        return RemoveNode(node);
    }

    public bool RemoveNodeAt(Vector2I position) {
        var node = GetNodeAt(position);
        if (node == null) return false;
        return RemoveNode(node);
    }

    public bool RemoveNode(MazeNode node) {
        if (!Nodes.Remove(node.Id)) return false;
        NodeGrid[node.Position] = null!;
        foreach (var other in Nodes.Values) {
            other.RemoveEdgeTo(node);
            if (other.Parent == node) other.Parent = null;
        }
        node.Parent = null;
        return true;
    }

    public void DisconnectNodes(int fromId, int toId) {
        var from = GetNode(fromId);
        var to = GetNode(toId);
        from.RemoveEdgeTo(to);
    }

    public void DisconnectNodes(Vector2I fromPos, Vector2I toPos) {
        var from = GetNodeAt(fromPos)!;
        var to = GetNodeAt(toPos)!;
        from.RemoveEdgeTo(to);
    }

    public void DisconnectNodes(MazeNode from, MazeNode to) {
        from.RemoveEdgeTo(to);
    }

    public MazeEdge ConnectNodeTo(int fromId, Vector2I target) {
        var from = GetNode(fromId);
        return ConnectNodeTo(from, target);
    }

    public MazeEdge ConnectNodeTo(Vector2I fromPosition, Vector2I target) {
        var from = GetNodeAt(fromPosition);
        return ConnectNodeTo(from, target);
    }

    public MazeEdge ConnectNodeTo(MazeNode from, Vector2I target) {
        var to = GetNodeAt(target);
        return ConnectNodes(from, to);
    }

    public MazeEdge ConnectNodeTowards(int fromId, Vector2I direction) {
        var from = GetNode(fromId);
        return ConnectNodeTowards(from, direction);
    }

    public MazeEdge ConnectNodeTowards(Vector2I fromPosition, Vector2I direction) {
        var from = GetNodeAt(fromPosition);
        return ConnectNodeTowards(from, direction);
    }

    public MazeEdge ConnectNodeTowards(MazeNode from, Vector2I direction) {
        return ConnectNodeTo(from, from.Position + direction);
    }

    public MazeEdge ConnectNodes(int fromId, int toId) {
        var from = GetNode(fromId);
        var to = GetNode(toId);
        return ConnectNodes(from, to);
    }

    public MazeEdge ConnectNodes(Vector2I fromPosition, Vector2I toPosition) {
        var from = GetNodeAt(fromPosition)!;
        var to = GetNodeAt(toPosition)!;
        return ConnectNodes(from, to);
    }

    public MazeEdge ConnectNodes(MazeNode from, MazeNode to, float weight = 0f) {
        if (IsValidEdgeFunc != null && !IsValidEdgeFunc(from.Position, to.Position)) {
            throw new InvalidEdgeException($"{nameof(IsValidEdgeFunc)} returned false", from.Position, to.Position);
        }
        var edge = from.AddEdgeTo(to, weight);
        OnNodeConnected?.Invoke(edge);
        return edge;
    }
    
    public bool IsValidEdge(Vector2I from, Vector2I to) {
        return (IsValidPositionFunc == null || (IsValidPositionFunc(from) && IsValidPositionFunc(to))) &&
               (IsValidEdgeFunc == null || IsValidEdgeFunc(from, to));
    }

    /// <summary>
    /// Returns all the adjacent nodes to the specified node, no matter if they are connected or not, or if one
    /// is the parent of the other.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<MazeNode> GetAdjacentNodes(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(IsValidPosition)
            .Select(pos => NodeGrid[pos])
            .Where(node => node != null);
    }

    /// <summary>
    /// Returns the adjacent positions to the specified node that are free (no node in that position) and valid, so a
    /// new node could be placed there.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetValidFreeAdjacentPositions(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(adjacentPos => IsValidPosition(adjacentPos) && NodeGrid[adjacentPos] == null);
    }

    /// <summary>
    /// Returns the directions to the adjacent positions to the specified node that are free (no node in that position)
    /// and valid, so a new node could be placed there.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetValidFreeAdjacentDirections(Vector2I from) {
        return GetValidFreeAdjacentPositions(from).Select(position => position - from);
    }

    public IEnumerable<Vector2I> GetOrtogonalPositions(Vector2I from) {
        return Array2D.Directions.Select(dir => from + dir)
            .Where(adjacentPos =>
                Geometry.IsPointInRectangle(adjacentPos.X, adjacentPos.Y, 0, 0, Width, Height));
    }

    public IEnumerable<MazeNode> GetChildren(MazeNode parent) {
        return Nodes.Values.Where(n => n.Parent == parent);
    }

    /// <summary>
    /// Finds all potential cycle connections between adjacent nodes that are not directly connected,
    /// calculating distances either through parent relationships or shortest path through edges.
    /// 
    /// Example: Consider a maze structure where solid lines are connections only and arrows
    /// show parent+connection relationships:
    ///   A -> B -> C
    ///   ↓    |    ↓
    ///   D -> E    F
    /// 
    /// For nodes B and E:
    /// - Parent distance is 4 (B->A->D->E)
    /// - Edge distance is 1 (B-E directly)
    /// 
    /// For nodes C and E:
    /// - Parent distance is 5 (C->B->A->D->E)
    /// - Edge distance is 2 (C-B-E)
    /// 
    /// Usage:
    /// var cycles = maze.FindPotentialCycles(minDistance: 3, useParentDistance: true);
    /// // Returns list of tuples: (nodeA, nodeB, distance)
    /// </summary>
    /// <param name="useParentDistance">If true, calculates distance following parent relationships.
    /// If false, finds the shortest path using all available connections.</param>
    /// <returns>List of potential connections ordered by distance (longest paths first)</returns>
    public PotentialCycles GetPotentialCycles(bool useParentDistance = false) {
        return new PotentialCycles(this, useParentDistance);
    }
}