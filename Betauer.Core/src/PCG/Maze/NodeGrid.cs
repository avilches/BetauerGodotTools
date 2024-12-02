using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.Maze;


/// <summary>
/// Represents an edge in the maze graph, connecting two nodes with a specific direction.
/// </summary>
/// <param name="from">The source node of the edge.</param>
/// <param name="to">The destination node of the edge.</param>
/// <param name="direction">The direction from the source to the destination node.</param>
public class NodeGridEdge(NodeGrid from, NodeGrid to, Vector2I direction) {
    public NodeGrid From { get; } = from ?? throw new ArgumentNullException(nameof(from));
    public NodeGrid To { get; } = to ?? throw new ArgumentNullException(nameof(to));
    public Vector2I Direction { get; } = direction;
    public object? Metadata { get; set; }
}

/// <summary>
/// Represents a node in the maze graph, containing connections to adjacent nodes.
/// </summary>
public class NodeGrid {
    private readonly Vector2I _position;

    /// <summary>
    /// Represents a node in the maze graph, containing connections to adjacent nodes.
    /// </summary>
    internal NodeGrid(BaseMazeGraph mazeGraph, int id, Vector2I position) {
        _position = position;
        MazeGraph = mazeGraph;
        Id = id;
        Position = position;
    }

    public BaseMazeGraph MazeGraph { get; }
    public int Id { get; }
    public Vector2I Position { get; }
    public NodeGrid? Parent { get; set; }
    public NodeGridEdge? Up { get; private set; }
    public NodeGridEdge? Right { get; private set; }
    public NodeGridEdge? Down { get; private set; }
    public NodeGridEdge? Left { get; private set; }
    public int Zone { get; set; }
    // public object? Metadata { get; set; }

    /// <summary>
    /// Creates a connection between this node and another node in the specified direction.
    /// </summary>
    /// <param name="direction">The direction of the connection.</param>
    /// <param name="other">The node to connect to.</param>
    /// <returns>The created edge.</returns>
    /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
    public NodeGridEdge SetEdge(Vector2I direction, NodeGrid other) {
        ArgumentNullException.ThrowIfNull(other);
        if (other.Position != Position + direction) throw new ArgumentException("Invalid MazeNode. The position is not in the direction", nameof(other));

        var edge = new NodeGridEdge(this, other, direction);

        if (direction == Vector2I.Up) Up = edge;
        else if (direction == Vector2I.Right) Right = edge;
        else if (direction == Vector2I.Down) Down = edge;
        else if (direction == Vector2I.Left) Left = edge;

        return edge;
    }

    /// <summary>
    /// Removes the connection in the specified direction.
    /// </summary>
    public void RemoveEdge(Vector2I direction) {
        if (direction == Vector2I.Up) Up = null;
        else if (direction == Vector2I.Right) Right = null;
        else if (direction == Vector2I.Down) Down = null;
        else if (direction == Vector2I.Left) Left = null;
    }

    /// <summary>
    /// Checks if there is a connection in the specified direction.
    /// </summary>
    public bool HasEdge(Vector2I direction) {
        if (direction == Vector2I.Up) return Up != null;
        if (direction == Vector2I.Right) return Right != null;
        if (direction == Vector2I.Down) return Down != null;
        if (direction == Vector2I.Left) return Left != null;
        return false;
    }

    public bool HasEdgeTo(Vector2I direction, NodeGrid other) {
        if (direction == Vector2I.Up) return Up != null && Up.To == other;
        if (direction == Vector2I.Right) return Right != null && Right.To == other;
        if (direction == Vector2I.Down) return Down != null && Down.To == other;
        if (direction == Vector2I.Left) return Left != null && Left.To == other;
        return false;
    }

    public NodeGridEdge? GetEdgeTo(NodeGrid to) {
        return GetEdges().FirstOrDefault(edge => edge.To == to);
    }

    public IEnumerable<NodeGridEdge> GetEdges() {
        return new[] { Up, Right, Down, Left }.Where(e => e != null)!;
    }

    /// <summary>
    /// Gets the edge in the specified direction, if it exists.
    /// </summary>
    public NodeGridEdge? GetEdge(Vector2I direction) {
        if (direction == Vector2I.Up) return Up;
        if (direction == Vector2I.Right) return Right;
        if (direction == Vector2I.Down) return Down;
        if (direction == Vector2I.Left) return Left;
        return null;
    }

    /// <summary>
    /// Returns the four MazeNode around this node (up, down, left, right), no matter if they are connected or not.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NodeGrid> GetNeighbors() {
        return Array2D.Directions.Select(dir => _position + dir)
            .Where(pos => MazeGraph.IsValid(pos))
            .Select(pos => MazeGraph.GetNode(pos))
            .Where(node => node != null);
    }

    public void Remove() {
        MazeGraph.Nodes.Remove(Id);
        MazeGraph.NodeGrid[Position] = null!;
        foreach (var node in MazeGraph.Nodes.Values) {
            var edgeToMe = node.GetEdgeTo(this);
            if (edgeToMe != null) {
                node.RemoveEdge(edgeToMe.Direction);
            }
            if (node.Parent == this) node.Parent = null;
        }
        Parent = null;
        Up = Right = Down = Left = null;
    }

    public Vector2I? GetDirectionToParent() {
        if (Parent == null) return null;
        return Position - Parent.Position;
    }

    public IEnumerable<NodeGrid> GetChildren() {
        return MazeGraph.Nodes.Values.Where(n => n.Parent == this);
    }

    public IEnumerable<NodeGridEdge> GetEdgesToChildren() {
        return GetEdges().Where(e => e.To.Parent == this);
    }

    public List<NodeGrid> GetPathToRoot() {
        var path = new List<NodeGrid>();
        var current = this;
        while (current != null) {
            path.Add(current);
            current = current.Parent;
        }
        return path;
    }
}