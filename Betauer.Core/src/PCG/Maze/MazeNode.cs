using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Betauer.Core.PCG.GridTemplate;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents a node in the maze graph, containing connections to adjacent nodes.
/// </summary>
public class MazeNode {
    /// <summary>
    /// Represents a node in the maze graph, containing connections to adjacent nodes.
    /// </summary>
    internal MazeNode(MazeGraph mazeGraph, int id, Vector2I position) {
        Graph = mazeGraph;
        Id = id;
        Position = position;
    }

    public MazeGraph Graph { get; }

    public int Id { get; }
    public Vector2I Position { get; }

    private int? _cachedDepth;
    private MazeNode? _parent;

    public MazeNode? Parent {
        get => _parent;
        set {
            if (_parent == value) return;
            // Check for circular reference before setting the parent
            if (value != null) {
                if (Graph != value.Graph) {
                    throw new InvalidOperationException($"Parent node belongs to another graph");
                }
                if (WouldCreateCircularReference(value)) {
                    throw new InvalidOperationException($"Can't set parent: would create circular reference. Node {Id} -> New Parent {value.Id}");
                }
            }
            _parent = value;
            InvalidateDepthCache();
        }
    }

    /// <summary>
    /// Checks if setting the specified node as parent would create a circular reference.
    /// A circular reference occurs when the new parent is one of the node's descendants.
    /// </summary>
    /// <param name="newParent">The node to check</param>
    /// <returns>True if setting this parent would create a circular reference</returns>
    private bool WouldCreateCircularReference(MazeNode newParent) {
        var current = newParent;
        while (current != null) {
            if (current == this) return true;
            current = current.Parent;
        }
        return false;
    }

    /// <summary>
    /// Invalidates the depth cache for this node and all its descendants
    /// </summary>
    private void InvalidateDepthCache() {
        _cachedDepth = null;
        foreach (var child in GetChildren()) {
            child.InvalidateDepthCache();
        }
    }

    /// <summary>
    /// Gets the depth of this node in the tree hierarchy.
    /// The root node has depth 0, its children have depth 1, and so on.
    /// </summary>
    public int Depth => _cachedDepth ??= CalculateDepth();

    /// <summary>
    /// Calculates the depth by counting parents up to the root
    /// </summary>
    private int CalculateDepth() {
        var depth = 0;
        var current = this;
        while (current.Parent != null) {
            depth++;
            current = current.Parent;
        }
        return depth;
    }

    public IEnumerable<MazeNode> GetChildren() {
        return Graph.GetNodes().Where(n => n.Parent == this);
    }

    private readonly List<MazeEdge> _outEdges = [];
    private readonly List<MazeEdge> _inEdges = [];

    public int ZoneId { get; set; }
    public int PartId { get; set; }

    public MazeNode? Up => GetEdgeTowards(Vector2I.Up)?.To;
    public MazeNode? UpRight => GetEdgeTowards(new Vector2I(1, -1))?.To;
    public MazeNode? UpLeft => GetEdgeTowards(new Vector2I(-1, -1))?.To;
    public MazeNode? Down => GetEdgeTowards(Vector2I.Down)?.To;
    public MazeNode? DownRight => GetEdgeTowards(new Vector2I(1, 1))?.To;
    public MazeNode? DownLeft => GetEdgeTowards(new Vector2I(-1, 1))?.To;
    public MazeNode? Right => GetEdgeTowards(Vector2I.Right)?.To;
    public MazeNode? Left => GetEdgeTowards(Vector2I.Left)?.To;

    public byte GetDirectionFlags() {
        byte directions = 0;
        if (Up != null) directions |= (byte)DirectionFlag.Up;
        if (UpRight != null) directions |= (byte)DirectionFlag.UpRight;
        if (Right != null) directions |= (byte)DirectionFlag.Right;
        if (DownRight != null) directions |= (byte)DirectionFlag.DownRight;
        if (Down != null) directions |= (byte)DirectionFlag.Down;
        if (DownLeft != null) directions |= (byte)DirectionFlag.DownLeft;
        if (Left != null) directions |= (byte)DirectionFlag.Left;
        if (UpLeft != null) directions |= (byte)DirectionFlag.UpLeft;
        return directions;
    }


    public int OutDegree => _outEdges.Count;

    public int InDegree => _inEdges.Count;

    public int Degree => OutDegree + InDegree;

    public float Weight { get; set; } = 0f;

    // Metadata object
    public object Metadata { get; set; }
    public void SetMetadata<T>(T value) => Metadata = value;
    public T GetMetadataOrDefault<T>() => Metadata is T value ? value : default;
    public T GetMetadataOrNew<T>() where T : new() => Metadata is T value ? value : new T();
    public T GetMetadataOr<T>(T defaultValue) => Metadata is T value ? value : defaultValue;
    public T GetMetadataOr<T>(Func<T> factory) => Metadata is T value ? value : factory();
    public bool HasMetadata<T>() => Metadata is T;
    public bool HasAnyMetadata => Metadata != null;
    public void ClearMetadata() => Metadata = null;

    // Attributes
    public void SetAttribute(string key, object value) => Graph.SetAttribute(this, key, value);
    public object? GetAttribute(string key) => Graph.GetAttribute(this, key);
    public T? GetAttributeAs<T>(string key) => Graph.GetAttributeAs<T>(this, key);
    public T GetAttributeOrDefault<T>(string key, T defaultValue) => Graph.GetAttributeOrDefault(this, key, defaultValue);
    public T GetAttributeOrCreate<T>(string key, Func<T> factory) => Graph.GetAttributeOrCreate(this, key, factory);
    public bool RemoveAttribute(string key) => Graph.RemoveAttribute(this, key);
    public bool HasAttribute(string key) => Graph.HasAttribute(this, key);
    public IEnumerable<KeyValuePair<string, object>> GetAttributes() => Graph.GetAttributes(this);
    public int AttributeCount => Graph.GetAttributeCount(this);
    public bool HasAnyAttribute => Graph.HasAnyAttribute(this);
    public void ClearAttributes() => Graph.ClearAttributes(this);
    
    // Edges
    public MazeEdge? GetEdgeTo(int id) => _outEdges.FirstOrDefault(edge => edge.To.Id == id);
    public MazeEdge? GetEdgeTo(Vector2I position) => _outEdges.FirstOrDefault(edge => edge.To.Position == position);
    public MazeEdge? GetEdgeTo(MazeNode to) => _outEdges.FirstOrDefault(edge => edge.To == to);
    public bool HasEdgeTo(int id) => GetEdgeTo(id) != null;
    public bool HasEdgeTo(Vector2I position) => GetEdgeTo(position) != null;
    public bool HasEdgeTo(MazeNode to) => GetEdgeTo(to) != null;

    public MazeEdge? GetEdgeFrom(int id) => _inEdges.FirstOrDefault(edge => edge.From.Id == id);
    public MazeEdge? GetEdgeFrom(Vector2I position) => _inEdges.FirstOrDefault(edge => edge.From.Position == position);
    public MazeEdge? GetEdgeFrom(MazeNode from) => _inEdges.FirstOrDefault(edge => edge.From == from);
    public bool HasEdgeFrom(int id) => GetEdgeFrom(id) != null;
    public bool HasEdgeFrom(Vector2I position) => GetEdgeFrom(position) != null;
    public bool HasEdgeFrom(MazeNode other) => GetEdgeFrom(other) != null;

    public MazeEdge? GetEdgeTowards(Vector2I direction) => _outEdges.FirstOrDefault(edge => edge.Direction == direction);
    public bool HasEdgeTowards(Vector2I direction) => GetEdgeTowards(direction) != null;

    public MazeEdge? GetEdgeFromDirection(Vector2I direction) => _inEdges.FirstOrDefault(edge => edge.Direction == -direction);
    public bool HasEdgeFromDirection(Vector2I direction) => GetEdgeFromDirection(direction) != null;

    public bool RemoveEdgeTo(int id) {
        var edge = GetEdgeTo(id);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeTo(Vector2I position) {
        var edge = GetEdgeTo(position);
        return edge != null && RemoveEdge(edge);
    }

    public bool RemoveEdgeTo(MazeNode node) {
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

    public bool RemoveEdgeFrom(MazeNode node) {
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

    public bool RemoveEdge(MazeEdge edge) {
        if (edge.From == this) {
            if (!_outEdges.Remove(edge)) return false;
            edge.To._inEdges.Remove(edge);
            Graph.ClearAttributes(edge);
            Graph.InvokeOnEdgeRemoved(edge);
            return true;
        }
        if (edge.To == this) {
            if (!_inEdges.Remove(edge)) return false;
            edge.From._outEdges.Remove(edge);
            Graph.ClearAttributes(edge);
            Graph.InvokeOnEdgeRemoved(edge);
            return true;
        }
        return false;
    }

    public ImmutableList<MazeEdge> GetOutEdges() => _outEdges.ToImmutableList();
    public ImmutableList<MazeEdge> GetInEdges() => _inEdges.ToImmutableList();
    public ImmutableList<MazeEdge> GetAllEdges() => _outEdges.Concat(_inEdges).ToImmutableList();

    public int OutEdgesCount => _outEdges.Count;
    public int InEdgesCount => _inEdges.Count;

    public bool RemoveNode() {
        if (!Graph.InternalRemoveNode(this)) return false;

        // Desconectar de todos los otros nodos
        foreach (var otherNode in Graph.GetNodes()) {
            otherNode.RemoveEdgeTo(this);
            if (otherNode.Parent == this) otherNode.Parent = null;
        }
        _outEdges.Clear();
        _inEdges.Clear();
        Parent = null;
        Graph.ClearAttributes(this);
        return true;
    }

    public MazeEdge ConnectTo(int id, object metadata = default, float weight = 0f) {
        var targetNode = Graph.GetNode(id);
        return ConnectTo(targetNode, metadata, weight);
    }

    public MazeEdge ConnectTo(Vector2I targetPos, object metadata = default, float weight = 0f) {
        var targetNode = Graph.GetNodeAt(targetPos);
        return ConnectTo(targetNode, metadata, weight);
    }

    /// <summary>
    /// Safe method to connect the node to the node in the direction, if it exists and it's allowed 
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="metadata"></param>
    /// <param name="weight"></param>
    /// <returns></returns>
    public MazeEdge? TryConnectTowards(Vector2I direction, object metadata = default, float weight = 0f) {
        var targetPos = Position + direction;
        var targetNode = Graph.GetNodeAtOrNull(targetPos);
        return targetNode != null ? ConnectTo(targetNode, metadata, weight) : null;
    }

    public MazeEdge ConnectTowards(Vector2I direction, object metadata = default, float weight = 0f) {
        var targetPos = Position + direction;
        var targetNode = Graph.GetNodeAt(targetPos);
        return ConnectTo(targetNode, metadata, weight);
    }

    public MazeEdge ConnectTo(MazeNode to, object metadata = default, float weight = 0f) {
        if (to.Graph != Graph) {
            throw new InvalidEdgeException("Cannot connect nodes from different graphs", Position, to.Position);
        }
        if (to == this) {
            throw new InvalidEdgeException("Cannot connect node to itself", Position, to.Position);
        }
        if (!Graph.CanConnect(this, to)) {
            throw new InvalidEdgeException($"Invalid edge between {Position} and {to.Position}", Position, to.Position);
        }

        var edge = GetEdgeTo(to);
        if (edge != null) {
            edge.Weight = weight;
            edge.Metadata = metadata;
            return edge;
        }

        edge = new MazeEdge(this, to, metadata, weight);
        _outEdges.Add(edge);
        to._inEdges.Add(edge);
        Graph.InvokeOnEdgeCreated(edge);
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
    public List<MazeNode> FindTreePathToRoot() {
        var path = new List<MazeNode>();
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
    public IReadOnlyList<MazeNode> FindTreePathToNode(MazeNode target)
        => MazePathFinder.GetPathToNode(this, target);

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
    public int GetTreeDistanceToNode(MazeNode target) {
        var path = FindTreePathToNode(target);
        return path.Count - 1; // -1 if no path because the path contains the start node. If no path, the list is empty, so -1
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
    /// if (path.Count > 0) {
    ///     // path contains the sequence of rooms to reach the destination
    /// }
    /// </summary>
    /// <param name="target">The target node</param>
    /// <returns>List of nodes forming the shortest path, or an empty list if no path exists. The path the start and the end node.</returns>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    public List<MazeNode> FindShortestPath(MazeNode target, Func<MazeNode, bool>? canTraverse = null)
        => MazePathFinder.FindShortestPath(this, target, PathWeightMode.None, canTraverse).Path;

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
    /// <returns>Distance in number of connections or -1 if no path exists. 1 means start node and target node are the same.</returns>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    public int GetGraphDistanceToNode(MazeNode target, Func<MazeNode, bool>? canTraverse = null) {
        var path = FindShortestPath(target, canTraverse);
        return path.Count - 1; // -1 if no path because the path contains the start node. If no path, the list is empty, so -1
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
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>Set of all reachable nodes, including the current node</returns>
    public HashSet<MazeNode> GetReachableNodes(Func<MazeNode, bool>? canTraverse = null)
        => MazePathFinder.GetReachableNodes(this, canTraverse);

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
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>Result containing the path and its total cost, or null if no path exists</returns>
    public PathResult FindWeightedPath(MazeNode target, PathWeightMode mode = PathWeightMode.Both, Func<MazeNode, bool>? canTraverse = null)
        => MazePathFinder.FindShortestPath(this, target, mode, canTraverse);

    public override string ToString() {
        return $"Id:{Id} {Position}";
    }
}