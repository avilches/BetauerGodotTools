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

    public Func<Vector2I, bool> IsValidPosition { get; set; } = _ => true;
    public Func<Vector2I, Vector2I, bool> IsValidEdge { get; set; } = (_, _) => true;
    public event Action<MazeEdge>? OnConnect;
    public event Action<MazeNode>? OnCreateNode;

    protected int LastId = 0;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// <param name="width">The width of the maze.</param>
    /// <param name="height">The height of the maze.</param>
    /// <param name="getAdjacentPositions">Optional function to determine the adjacent positions of every node.
    /// Optional, by default, it uses ortogonal positions up, down, left, right </param>
    protected BaseMazeGraph(int width, int height, Func<Vector2I, IEnumerable<Vector2I>>? getAdjacentPositions = null) {
        Width = width;
        Height = height;
        GetAdjacentPositions = getAdjacentPositions ?? GetOrtogonalPositions;
        NodeGrid = new Array2D<MazeNode>(width, height);
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


    public void ConnectNode(MazeNode from, MazeNode to, bool twoWays) {
        _ConnectNode(from, to);
        if (twoWays) _ConnectNode(to, from);
    }

    private void _ConnectNode(MazeNode from, MazeNode to) {
        if (from.HasEdgeTo(to)) return;
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