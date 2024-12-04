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
    public event Action<MazeEdge>? NodeConnected;
    public event Action<MazeNode>? NodeCreated;

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

    public MazeNode GetOrCreateNode(Vector2I position) {
        if (!IsValidPosition(position)) {
            throw new InvalidNodeException($"Can't get node at {position}. Invalid position", position);
        }
        var node = NodeGrid[position];
        if (node != null) return node;

        return CreateNode(position, null);
    }

    public MazeNode CreateNode(Vector2I position, MazeNode? parent = null) {
        if (!IsValidPosition(position)) {
            throw new InvalidNodeException($"Can't create node at {position}. Invalid position", position);
        }
        var node = NodeGrid[position];
        if (node != null) {
            throw new InvalidOperationException("Can't create node at "+position+". Invalid position");
        }

        node = new MazeNode(LastId++, position) {
            Parent = parent
        };
        NodeGrid[position] = node;
        Nodes[node.Id] = node;
        NodeCreated?.Invoke(node);
        return node;
    }

    public MazeNode? GetNodeAt(Vector2I position) {
        if (!IsValidPosition(position)) {
            throw new InvalidNodeException($"Can't get node at {position}. Invalid position", position);
        }
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

    public void ConnectNodes(MazeNode from, MazeNode to, bool twoWays) {
        _ConnectNode(from, to);
        if (twoWays) _ConnectNode(to, from);
    }

    public void ConnectNodeTowards(MazeNode from, Vector2I direction, bool twoWays) {
        ConnectNodeAt(from, from.Position + direction, twoWays);
    }

    public void ConnectNodeAt(MazeNode from, Vector2I target, bool twoWays) {
        var to = GetNodeAt(target);
        if (to == null) return;
        ConnectNodes(from, to, twoWays);
    }

    private void _ConnectNode(MazeNode from, MazeNode to) {
        if (to == null || from == null) return;
        if (from.HasEdgeTo(to)) {
            // Just ignore duplicated edges
            return;
        }
        if (!IsValidEdge(from.Position, to.Position)) {
            throw new InvalidEdgeException(from.Position, to.Position);
        }
        var edge = from.AddEdgeTo(to);
        NodeConnected?.Invoke(edge);
    }

    public bool IsValidEdge(Vector2I from, Vector2I to) {
        // No need to validate if the nodes are valid, as the edge is created between valid nodes.
        return IsValidEdgeFunc(from, to);
    }

    public IEnumerable<Vector2I> GetValidFreeAdjacentPositions(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(adjacentPos => IsValidPosition(adjacentPos) && NodeGrid[adjacentPos] == null);
    }

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

}