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
    public event Action<MazeEdge>? OnConnect;
    public event Action<MazeNode>? OnCreateNode;

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
        if (!IsValidPosition(position)) throw new ArgumentException("Invalid position", nameof(position));
        var node = NodeGrid[position];
        if (node != null) return node;

        return CreateNode(position, null);
    }

    public MazeNode CreateNode(Vector2I position, MazeNode? parent = null) {
        if (!IsValidPosition(position)) throw new ArgumentException("Invalid position", nameof(position));
        var node = NodeGrid[position];
        if (node != null) throw new ArgumentException($"Node already exists in position {position}", nameof(position));

        node = new MazeNode(this, LastId++, position) {
            Parent = parent
        };
        NodeGrid[position] = node;
        Nodes[node.Id] = node;
        OnCreateNode?.Invoke(node);
        return node;
    }

    public MazeNode? GetNode(Vector2I position) => NodeGrid[position];

    public void RemoveNode(int id) {
        if (!Nodes.TryGetValue(id, out var node)) return;
        RemoveNode(node);
    }

    public void RemoveNode(Vector2I position) {
        var node = GetNode(position);
        if (node == null) return;
        RemoveNode(node);
    }

    public void RemoveNode(MazeNode node) {
        Nodes.Remove(node.Id);
        NodeGrid[node.Position] = null!;
        foreach (var other in Nodes.Values) {
            other.RemoveEdgeTo(other);
            if (other.Parent == node) other.Parent = null;
        }
        node.Parent = null;
    }
    
    public bool IsValidEdge(Vector2I from, Vector2I to) {
        // No need to validate if the nodes are valid, as the edge is created between valid nodes.
        return IsValidEdgeFunc(from, to);
    }

    public void ConnectNode(MazeNode from, MazeNode to, bool twoWays) {
        _ConnectNode(from, to);
        if (twoWays) _ConnectNode(to, from);
    }

    private void _ConnectNode(MazeNode from, MazeNode to) {
        if (from.HasEdgeTo(to)) {
            // Just ignore duplicated edges
            return;
        }
        if (!IsValidEdge(from.Position, to.Position)) throw new ArgumentException("Invalid edge: "+from.Position+" -> "+to.Position);
        var edge = from.AddEdgeTo(to);
        OnConnect?.Invoke(edge);
    }

    public IEnumerable<Vector2I> GetValidFreeAdjacentPositions(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(adjacentPos => GetNode(adjacentPos) == null && IsValidPosition(adjacentPos));
    }

    public IEnumerable<Vector2I> GetValidFreeAdjacentDirections(Vector2I from) {
        return GetValidFreeAdjacentPositions(from).Select(position => position - from);
    }
    
    public IEnumerable<Vector2I> GetOrtogonalPositions(Vector2I from) {
        return Array2D.Directions.Select(dir => from + dir)
            .Where(adjacentPos => 
                Geometry.IsPointInRectangle(adjacentPos.X, adjacentPos.Y, 0, 0, Width, Height));
    }


}