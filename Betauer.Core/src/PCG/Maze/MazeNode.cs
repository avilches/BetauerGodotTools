using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;


/// <summary>
/// Represents an edge in the maze graph, connecting two nodes
/// </summary>
/// <param name="from">The source node of the edge.</param>
/// <param name="to">The destination node of the edge.</param>
public class MazeEdge(MazeNode from, MazeNode to) {
    public MazeNode From { get; } = from ?? throw new ArgumentNullException(nameof(from));
    public MazeNode To { get; } = to ?? throw new ArgumentNullException(nameof(to));
    public Vector2I Direction => To.Position - From.Position;
}

/// <summary>
/// Represents a node in the maze graph, containing connections to adjacent nodes.
/// </summary>
public class MazeNode {

    /// <summary>
    /// Represents a node in the maze graph, containing connections to adjacent nodes.
    /// </summary>
    internal MazeNode(BaseMazeGraph mazeGraph, int id, Vector2I position) {
        MazeGraph = mazeGraph;
        Id = id;
        Position = position;
    }

    public BaseMazeGraph MazeGraph { get; }
    public int Id { get; }
    public Vector2I Position { get; }
    public MazeNode? Parent { get; set; }
    private readonly List<MazeEdge> _edges = [];
    public int Zone { get; set; }
    
    public MazeNode? Up => GetEdgeTo(Vector2I.Up)?.To;
    public MazeNode? Down => GetEdgeTo(Vector2I.Down)?.To;
    public MazeNode? Right => GetEdgeTo(Vector2I.Right)?.To;
    public MazeNode? Left => GetEdgeTo(Vector2I.Left)?.To;

    /// <summary>
    /// Creates a connection between this node and another node in the specified direction.
    /// </summary>
    /// <param name="node">The node to connect to.</param>
    /// <returns>The created edge.</returns>
    public MazeEdge AddEdgeTo(MazeNode node) {
        var edge = GetEdgeTo(node);
        if (edge != null) return edge;
        edge = new MazeEdge(this, node);
        _edges.Add(edge);
        return edge;
    }
    
    public MazeEdge? GetEdgeTo(MazeNode to) {
        return _edges.FirstOrDefault(edge => edge.To == to);
    }

    public bool HasEdgeTo(MazeNode other) {
        return GetEdgeTo(other) != null;
    }

    public void RemoveEdgeTo(MazeNode node) {
        var edge = GetEdgeTo(node);
        if (edge != null) {
            _edges.Remove(edge);
        }
    }

    public MazeEdge? GetEdgeTo(Vector2I direction) {
        return _edges.FirstOrDefault(edge => edge.Direction == direction);
    }
    
    public bool HasEdgeDirection(Vector2I direction) {
        return GetEdgeTo(direction) != null;
    }
    
    public void RemoveEdgeDirection(Vector2I direction) {
        var edge = GetEdgeTo(direction);
        if (edge != null) {
            _edges.Remove(edge);
        }
    }
    
    public void RemoveEdge(MazeEdge edge) {
        _edges.Remove(edge);
    }

    public IEnumerable<MazeEdge> GetEdges() {
        return _edges.ToImmutableList();
    }

    public Vector2I? GetDirectionToParent() {
        if (Parent == null) return null;
        return Position - Parent.Position;
    }

    public IEnumerable<MazeNode> GetChildren() {
        return MazeGraph.Nodes.Values.Where(n => n.Parent == this);
    }

    public IEnumerable<MazeEdge> GetEdgesToChildren() {
        return _edges.Where(e => e.To.Parent == this);
    }

    public List<MazeNode> GetPathToRoot() {
        var path = new List<MazeNode>();
        var current = this;
        while (current != null) {
            path.Add(current);
            current = current.Parent;
        }
        return path;
    }
}