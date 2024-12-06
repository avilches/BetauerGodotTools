using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents a node in the maze graph, containing connections to adjacent nodes.
/// </summary>
public class MazeNode<T> {
    private static readonly PathFinder<T> PathFinder = new();

    /// <summary>
    /// Represents a node in the maze graph, containing connections to adjacent nodes.
    /// </summary>
    internal MazeNode(BaseMazeGraph<T> mazeGraph, int id, Vector2I position) {
        _mazeGraph = mazeGraph;
        Id = id;
        Position = position;
    }

    private readonly BaseMazeGraph<T> _mazeGraph;

    public int Id { get; }
    public Vector2I Position { get; }
    public MazeNode<T>? Parent { get; set; }
    private readonly List<MazeEdge<T>> _outEdges = [];
    private readonly List<MazeEdge<T>> _inEdges = [];
    public int Zone { get; set; }

    public MazeNode<T>? Up => GetEdgeTowards(Vector2I.Up)?.To;
    public MazeNode<T>? Down => GetEdgeTowards(Vector2I.Down)?.To;
    public MazeNode<T>? Right => GetEdgeTowards(Vector2I.Right)?.To;
    public MazeNode<T>? Left => GetEdgeTowards(Vector2I.Left)?.To;

    public int OutDegree => _outEdges.Count;

    public int InDegree => _inEdges.Count;

    public int Degree => OutDegree + InDegree;

    public T Metadata { get; set; }

    public float Weight { get; set; } = 0f;

    public MazeEdge<T>? GetEdgeTo(int id) => _outEdges.FirstOrDefault(edge => edge.To.Id == id);
    public MazeEdge<T>? GetEdgeTo(Vector2I position) => _outEdges.FirstOrDefault(edge => edge.To.Position == position);
    public MazeEdge<T>? GetEdgeTo(MazeNode<T> to) => _outEdges.FirstOrDefault(edge => edge.To == to);
    public bool HasEdgeTo(int id) => GetEdgeTo(id) != null;
    public bool HasEdgeTo(Vector2I position) => GetEdgeTo(position) != null;
    public bool HasEdgeTo(MazeNode<T> to) => GetEdgeTo(to) != null;

    public MazeEdge<T>? GetEdgeFrom(int id) => _inEdges.FirstOrDefault(edge => edge.From.Id == id);
    public MazeEdge<T>? GetEdgeFrom(Vector2I position) => _inEdges.FirstOrDefault(edge => edge.From.Position == position);
    public MazeEdge<T>? GetEdgeFrom(MazeNode<T> from) => _inEdges.FirstOrDefault(edge => edge.From == from);
    public bool HasEdgeFrom(int id) => GetEdgeFrom(id) != null;
    public bool HasEdgeFrom(Vector2I position) => GetEdgeFrom(position) != null;
    public bool HasEdgeFrom(MazeNode<T> other) => GetEdgeFrom(other) != null;

    public MazeEdge<T>? GetEdgeTowards(Vector2I direction) => _outEdges.FirstOrDefault(edge => edge.Direction == direction);
    public bool HasEdgeTowards(Vector2I direction) => GetEdgeTowards(direction) != null;

    public MazeEdge<T>? GetEdgeFromDirection(Vector2I direction) => _inEdges.FirstOrDefault(edge => edge.Direction == -direction);
    public bool HasEdgeFromDirection(Vector2I direction) => GetEdgeFromDirection(direction) != null;

    public bool RemoveEdgeTo(int id) {
        var edge = GetEdgeTo(id);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeTo(Vector2I position) {
        var edge = GetEdgeTo(position);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeTo(MazeNode<T> node) {
        var edge = GetEdgeTo(node);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeFrom(int id) {
        var edge = GetEdgeFrom(id);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeFrom(Vector2I position) {
        var edge = GetEdgeFrom(position);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeFrom(MazeNode<T> node) {
        var edge = GetEdgeTo(node);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeTowards(Vector2I direction) {
        var edge = GetEdgeTowards(direction);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeFromDirection(Vector2I direction) {
        var edge = GetEdgeFromDirection(direction);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdge(MazeEdge<T> edge) {
        if (edge.From == this) {
            if (!_outEdges.Remove(edge)) return false;
            edge.To._inEdges.Remove(edge);
            _mazeGraph.InvokeOnEdgeRemoved(edge);
            return true;
        }
        if (edge.To == this) {
            if (!_inEdges.Remove(edge)) return false;
            edge.From._outEdges.Remove(edge);
            _mazeGraph.InvokeOnEdgeRemoved(edge);
            return true;
        }
        return false;
    }

    public ImmutableList<MazeEdge<T>> GetOutEdges() => _outEdges.ToImmutableList();

    public ImmutableList<MazeEdge<T>> GetInEdges() => _inEdges.ToImmutableList();

    public ImmutableList<MazeEdge<T>> GetAllEdges() => _outEdges.Concat(_inEdges).ToImmutableList();


    public bool RemoveNode() {
        if (!_mazeGraph.InternalRemoveNode(this)) return false;

        // Desconectar de todos los otros nodos
        foreach (var otherNode in _mazeGraph.GetNodes()) {
            otherNode.RemoveEdgeTo(this);
            if (otherNode.Parent == this) otherNode.Parent = null;
        }
        _outEdges.Clear();
        _inEdges.Clear();
        Parent = null;
        return true;
    }

    public MazeEdge<T> ConnectTo(int id, T? metadata = default, float weight = 0f) {
        var targetNode = _mazeGraph.GetNode(id);
        return ConnectTo(targetNode, metadata, weight);
    }

    public MazeEdge<T> ConnectTo(Vector2I targetPos, T? metadata = default, float weight = 0f) {
        var targetNode = _mazeGraph.GetNodeAt(targetPos);
        return ConnectTo(targetNode, metadata, weight);
    }

    public MazeEdge<T> ConnectTowards(Vector2I direction, T? metadata = default, float weight = 0f) {
        var targetPos = Position + direction;
        var targetNode = _mazeGraph.GetNodeAt(targetPos);
        return ConnectTo(targetNode, metadata, weight);
    }

    public MazeEdge<T> ConnectTo(MazeNode<T> to, T? metadata = default, float weight = 0f) {
        if (to._mazeGraph != _mazeGraph) {
            throw new InvalidEdgeException("Cannot connect nodes from different graphs", Position, to.Position);
        }
        if (to == this) {
            throw new InvalidEdgeException("Cannot connect node to itself", Position, to.Position);
        }
        if (!_mazeGraph.IsValidEdge(Position, to.Position)) {
            throw new InvalidEdgeException($"Invalid edge between {Position} and {to.Position}", Position, to.Position);
        }

        var edge = GetEdgeTo(to);
        if (edge != null) {
            edge.Weight = weight;
            edge.Metadata = metadata;
            return edge;
        }

        edge = new MazeEdge<T>(this, to, metadata, weight);
        _outEdges.Add(edge);
        to._inEdges.Add(edge);
        _mazeGraph.InvokeOnEdgeCreated(edge);
        return edge;
    }

    /// <summary>
    /// Finds the path from the current node to the root by following parent references.
    /// Useful for tracing the lineage or hierarchy of a node in a tree structure.
    /// 
    /// Example: In a dialogue tree, getting the sequence of choices 
    /// that led to the current dialogue option.
    /// 
    /// Usage:
    /// var path = node.GetPathToRoot();
    /// // path contains [currentNode, parentNode, grandparentNode, ..., rootNode]
    /// </summary>
    /// <returns>List of nodes from current to root, including both endpoints.</returns>
    public List<MazeNode<T>> GetPathToRoot() {
        var path = new List<MazeNode<T>>();
        var current = this;
        while (current != null) {
            path.Add(current);
            current = current.Parent;
        }
        return path;
    }

    /// <summary>
    /// Finds a path between two nodes using the parent-child hierarchy.
    /// Useful when nodes are organized in a tree structure and you need to
    /// find a path through common ancestors.
    /// 
    /// Example: In an AI decision tree, finding how to transition from one
    /// state to another through valid intermediate states.
    /// 
    /// Usage:
    /// var path = startNode.GetPathToNode(targetNode);
    /// if (path != null) {
    ///     // path contains the sequence of nodes to reach the target
    /// }
    /// </summary>
    /// <param name="target">The target node</param>
    /// <returns>List of nodes forming the path, or null if no path exists</returns>
    public List<MazeNode<T>>? GetPathToNode(MazeNode<T> target)
        => PathFinder.GetPathToNode(this, target);

    /// <summary>
    /// Calculates the distance to another node using parent references.
    /// Useful for measuring the "genealogical distance" between two nodes in a tree.
    /// 
    /// Example: In a tech tree, determining how many research steps
    /// separate two technologies.
    /// 
    /// Usage:
    /// int distance = nodeA.GetDistanceToNode(nodeB);
    /// // distance: 0 = same node, -1 = no path, n = number of steps
    /// </summary>
    /// <param name="target">The target node</param>
    /// <returns>Distance in number of nodes or -1 if no path exists</returns>
    public int GetDistanceToNode(MazeNode<T> target) {
        var path = GetPathToNode(target);
        return path != null ? path.Count - 1 : -1;
    }

    /// <summary>
    /// Finds the shortest path to another node using direct connections (edges).
    /// Unlike GetPathToNode, this considers all available connections,
    /// not just the parent-child hierarchy.
    /// 
    /// Example: In a game level map, finding the shortest route
    /// between two rooms connected by corridors.
    /// 
    /// Usage:
    /// var path = room1.FindShortestPath(room2);
    /// if (path != null) {
    ///     // path contains the sequence of rooms to reach the destination
    /// }
    /// </summary>
    /// <param name="target">The target node</param>
    /// <returns>List of nodes forming the shortest path, or null if no path exists</returns>
    public List<MazeNode<T>>? FindShortestPath(MazeNode<T> target)
        => PathFinder.FindShortestPath(this, target);

    /// <summary>
    /// Calculates the shortest distance to another node using direct connections.
    /// Useful for determining the minimum number of steps needed to reach
    /// one point from another.
    /// 
    /// Example: In a board game, calculating how many moves are needed
    /// to reach one square from another.
    /// 
    /// Usage:
    /// int steps = square1.GetDistanceToNodeByEdges(square2);
    /// // steps: -1 if no path exists, or the number of moves needed
    /// </summary>
    /// <param name="target">The target node</param>
    /// <returns>Distance in number of connections or -1 if no path exists</returns>
    public int GetDistanceToNodeByEdges(MazeNode<T> target) {
        var path = FindShortestPath(target);
        return path != null ? path.Count - 1 : -1;
    }

    /// <summary>
    /// Gets all nodes that can be reached from this node using available connections.
    /// Useful for determining which areas are accessible from a given position.
    /// 
    /// Example: In a game map, finding all zones accessible
    /// from the player's current position.
    /// 
    /// Usage:
    /// var reachable = currentPosition.GetReachableNodes();
    /// // reachable contains all nodes that can be reached
    /// </summary>
    /// <returns>Set of all reachable nodes, including the starting node</returns>
    public HashSet<MazeNode<T>> GetReachableNodes()
        => PathFinder.GetReachableNodes(this);

    /// <summary>
    /// Finds the most efficient path considering node and/or connection weights.
    /// Useful when different paths have varying costs or difficulties.
    /// 
    /// Example: In a navigation system, finding the most efficient route
    /// considering factors like distance (edge weights) and points of
    /// interest (node weights).
    /// 
    /// Usage:
    /// var result = start.FindWeightedPath(destination, PathWeightMode.Both);
    /// if (result != null) {
    ///     // result.Path = sequence of nodes
    ///     // result.TotalCost = total path cost
    /// }
    /// </summary>
    /// <param name="target">Destination node</param>
    /// <param name="mode">Weight calculation mode: nodes only, edges only, or both</param>
    /// <returns>Result containing the path and its total cost, or null if no path exists</returns>
    public PathResult<T>? FindWeightedPath(MazeNode<T> target, PathWeightMode mode = PathWeightMode.Both)
        => PathFinder.FindWeightedPath(this, target, mode);

    public override string ToString() {
        return $"Id:{Id} {Position}";
    }
}

public class PathResult<T>(List<MazeNode<T>> path, float totalCost) {
    public List<MazeNode<T>> Path { get; } = path;
    public float TotalCost { get; } = totalCost;

    // Helper method to calculate only the edges cost
    public float GetEdgesCost() {
        float cost = 0;
        for (var i = 0; i < Path.Count - 1; i++) {
            var edge = Path[i].GetEdgeTo(Path[i + 1]);
            if (edge != null) cost += edge.Weight;
        }
        return cost;
    }

    // Helper method to calculate only the nodes cost
    public float GetNodesCost() {
        return Path.Sum(node => node.Weight);
    }
}