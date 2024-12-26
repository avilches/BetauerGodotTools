using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents a maze as a graph structure with nodes and edges in a 2d grid.
/// </summary>
public partial class MazeGraph {
    protected readonly Dictionary<Vector2I, MazeNode> NodeGrid = [];
    protected readonly Dictionary<int, MazeNode> Nodes = [];

    public Func<Vector2I, IEnumerable<Vector2I>> GetAdjacentPositions { get; set; }

    private readonly List<Func<Vector2I, bool>> _positionValidators = [];
    private readonly List<Func<MazeNode, MazeNode, bool>> _edgeValidators = [];

    public void AddPositionValidator(Func<Vector2I, bool> validator) => _positionValidators.Add(validator);
    public void RemovePositionValidator(Func<Vector2I, bool> validator) => _positionValidators.Remove(validator);
    public void AddEdgeValidator(Func<MazeNode, MazeNode, bool> validator) => _edgeValidators.Add(validator);
    public void RemoveEdgeValidator(Func<MazeNode, MazeNode, bool> validator) => _edgeValidators.Remove(validator);
    public bool IsValidPosition(Vector2I position) => _positionValidators.Count == 0 || _positionValidators.All(validator => validator(position));
    public bool CanConnect(MazeNode from, MazeNode to) => _edgeValidators.Count == 0 || _edgeValidators.All(validator => validator(from, to));
    public void ClearPositionValidators() => _positionValidators.Clear();
    public void ClearEdgeValidators() => _edgeValidators.Clear();

    public event Action<MazeEdge>? OnEdgeCreated;
    public event Action<MazeEdge>? OnEdgeRemoved;
    public event Action<MazeNode>? OnNodeCreated;
    public event Action<MazeNode>? OnNodeRemoved;

    protected int LastId = 0;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// Optional, by default, it uses ortogonal positions up, down, left, right </param>
    public MazeGraph() {
        GetAdjacentPositions = GetOrtogonalPositions;
    }

    public readonly record struct AttributeKey(object Instance, string Key);
    
    private readonly Dictionary<AttributeKey, object> _attributes = [];
    
    internal void SetAttribute(object instance, string key, object value) => _attributes[new(instance, key)] = value;
    internal object? GetAttribute(object instance, string key) => _attributes.GetValueOrDefault(new(instance, key));
    internal object GetAttributeOr(object instance, string key, object defaultValue) => _attributes.GetValueOrDefault(new(instance, key), defaultValue);
    internal T? GetAttributeAs<T>(object instance, string key) => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : default;
    internal T GetAttributeAsOrDefault<T>(object instance, string key, T defaultValue) => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : defaultValue;
    internal T GetAttributeAsOrNew<T>(object instance, string key) where T : new() => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : new T();
    internal T GetAttributeAsOr<T>(object instance, string key, Func<T> factory) => _attributes.TryGetValue(new(instance, key), out var value) && value is T typedValue ? typedValue : factory();
    internal bool RemoveAttribute(object instance, string key) => _attributes.Remove(new(instance, key));
    internal bool HasAttribute(object instance, string key) => _attributes.ContainsKey(new(instance, key));
    internal bool HasAttributeWithValue(object instance, string key, object value) => _attributes.TryGetValue(new(instance, key), out var existingValue) && Equals(existingValue, value);
    internal bool HasAttributeOfType<T>(object instance, string key) => _attributes.TryGetValue(new(instance, key), out var value) && value is T;
    internal IEnumerable<KeyValuePair<string, object>> GetAttributes(object instance) {
        return _attributes
            .Where(kv => kv.Key.Instance == instance)
            .Select(kv => new KeyValuePair<string, object>(kv.Key.Key, kv.Value));
    }
    internal int GetAttributeCount(object instance) => _attributes.Count(kv => kv.Key.Instance == instance);
    internal bool HasAnyAttribute(object instance) => _attributes.Keys.Any(k => k.Instance == instance);
    
    internal void ClearAttributes(object instance) {
        var keys = _attributes.Keys.Where(k => k.Instance == instance).ToList();
        foreach (var key in keys) {
            _attributes.Remove(key);
        }
    }

    public void Clear() {
        NodeGrid.Clear();
        Nodes.Clear();
        _attributes.Clear();
        LastId = 0;
    }
    
    public MazeNode GetNode(int id) {
        return Nodes[id];
    }

    public MazeNode? GetNodeOrNull(int id) {
        return Nodes.GetValueOrDefault(id);
    }

    public IReadOnlyCollection<MazeNode> GetNodes() {
        return Nodes.Values;
    }

    public MazeNode GetOrCreateNode(Vector2I position) {
        ValidatePosition(position, nameof(GetOrCreateNode));
        return NodeGrid.TryGetValue(position, out var node)
            ? node
            : CreateNode(position);
    }

    private void ValidatePosition(Vector2I position, string message) {
        if (!IsValidPosition(position)) {
            throw new InvalidNodeException($"Invalid position {position} in {message}", position);
        }
    }

    public MazeNode CreateNode(Vector2I position, MazeNode? parent = null, object metadata = default, float weight = 0f) {
        ValidatePosition(position, nameof(CreateNode));
        if (NodeGrid.TryGetValue(position, out var node)) {
            throw new InvalidOperationException($"Can't create node at {position}: there is already a node there with id {node.Id}");
        }

        node = new MazeNode(this, LastId++, position) {
            Parent = parent,
            Metadata = metadata!,
            Weight = weight
        };
        NodeGrid[position] = node;
        Nodes[node.Id] = node;
        OnNodeCreated?.Invoke(node);
        return node;
    }

    public MazeNode GetNodeAt(Vector2I position) {
        ValidatePosition(position, nameof(GetNodeAt));
        return NodeGrid[position];
    }

    public bool HasNodeAt(Vector2I position) {
        return NodeGrid.ContainsKey(position);
    }

    public bool HasNode(int id) {
        return Nodes.ContainsKey(id);
    }

    public MazeNode? GetNodeAtOrNull(Vector2I position) {
        return IsValidPosition(position) ? NodeGrid.GetValueOrDefault(position) : null;
    }

    internal bool InternalRemoveNode(MazeNode node) {
        if (!Nodes.Remove(node.Id)) return false;
        NodeGrid.Remove(node.Position);
        OnNodeRemoved?.Invoke(node);
        return true;
    }

    public bool RemoveNode(int id) {
        return Nodes.TryGetValue(id, out var node) && node.RemoveNode();
    }

    public bool RemoveNodeAt(Vector2I position) {
        var node = GetNodeAtOrNull(position);
        return node != null && node.RemoveNode();
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

    public MazeEdge ConnectNodes(int fromId, int toId, object metadata = default, float weight = 0f) {
        var from = GetNode(fromId);
        var to = GetNode(toId);
        return from.ConnectTo(to, metadata, weight);
    }

    public MazeEdge ConnectNodes(Vector2I fromPosition, Vector2I toPosition, object metadata = default, float weight = 0f) {
        var from = GetNodeAt(fromPosition)!;
        var to = GetNodeAt(toPosition)!;
        return from.ConnectTo(to, metadata, weight);
    }

    public MazeEdge ConnectNodes(MazeNode from, MazeNode to, object metadata = default, float weight = 0f) {
        return from.ConnectTo(to, metadata, weight);
    }

    internal void InvokeOnEdgeCreated(MazeEdge edge) {
        OnEdgeCreated?.Invoke(edge);
    }

    internal void InvokeOnEdgeRemoved(MazeEdge edge) {
        OnEdgeRemoved?.Invoke(edge);
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
            .Select(pos => NodeGrid.GetValueOrDefault(pos)!)
            .Where(node => node != null!);
    }

    /// <summary>
    /// Returns the adjacent positions to the specified node that are free (no node in that position) and valid, so a
    /// new node could be placed there.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetAvailableAdjacentPositions(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(adjacentPos => IsValidPosition(adjacentPos) && !NodeGrid.ContainsKey(adjacentPos));
    }

    /// <summary>
    /// Returns the directions to the adjacent positions to the specified node that are free (no node in that position)
    /// and valid, so a new node could be placed there.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public IEnumerable<Vector2I> GetValidFreeAdjacentDirections(Vector2I from) {
        return GetAvailableAdjacentPositions(from).Select(position => position - from);
    }

    public IEnumerable<Vector2I> GetOrtogonalPositions(Vector2I from) {
        return Array2D.Directions.Select(dir => from + dir);
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

    public Vector2I GetOffset() {
        return new Vector2I(GetNodes().Min(v => v.Position.X), GetNodes().Min(v => v.Position.Y));
    }

    public Vector2I GetSize() {
        return new Vector2I(
            GetNodes().Max(v => v.Position.X) - GetNodes().Min(v => v.Position.X),
            GetNodes().Max(v => v.Position.Y) - GetNodes().Min(v => v.Position.Y));
    }
    
    public string Draw() {
        var allCanvas = new TextCanvas();
        foreach (var node in GetNodes()) {
            var canvas = new TextCanvas();
            canvas.Write(1, 1, node.Id.ToString());
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "--");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");
            allCanvas.Write(node.Position.X * 4, node.Position.Y * 3, canvas.ToString());
        }
        return allCanvas.ToString();
    }
    
    public string Print(char c = '\u2588') {
        var allCanvas = new TextCanvas();
        foreach (var node in GetNodes()) {
            var canvas = new TextCanvas();
            canvas.Write(1, 1, "\u2588");
            if (node.Up != null) canvas.Write(1, 0, "\u2588");
            if (node.Left != null) canvas.Write(0, 1, "\u2588");
            allCanvas.Write(node.Position.X * 2, node.Position.Y * 2, canvas.ToString());
        }
        return allCanvas.ToString();
    }

    /// <summary>
    /// Returns all the nodes without Parent
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode> GetRoots() {
        return GetNodes().Where(n => n.Parent == null);
    }
}


