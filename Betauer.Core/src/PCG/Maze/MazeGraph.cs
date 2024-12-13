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
public partial class MazeGraph {
    protected readonly Dictionary<Vector2I, MazeNode> NodeGrid  = [];
    protected readonly Dictionary<int, MazeNode> Nodes  = [];
    public MazeNode Root { get; protected set; }

    public Func<Vector2I, IEnumerable<Vector2I>> GetAdjacentPositions { get; set; }

    /// <summary>
    /// Called to determine if a position is valid before creating a node, rejecting with an exception if the position is not valid.
    /// Called to filter the adjacent positions of a node, ignoring invalid positions. 
    /// </summary>
    public Func<Vector2I, bool>? IsValidPositionFunc { get; set; } = _ => true;

    /// <summary>
    /// Called to determine if an edge is valid before creating it.
    /// </summary>
    public Func<Vector2I, Vector2I, bool>? IsValidEdgeFunc { get; set; } = (_, _) => true;

    public event Action<MazeEdge>? OnEdgeCreated;
    public event Action<MazeEdge>? OnEdgeRemoved;
    public event Action<MazeNode>? OnNodeCreated;
    public event Action<MazeNode>? OnNodeRemoved;

    protected int LastId = 0;

    /// <summary>
    /// Initializes a new instance of the MazeGraph class.
    /// </summary>
    /// <param name="width">The width of the maze.</param>
    /// <param name="height">The height of the maze.</param>
    /// Optional, by default, it uses ortogonal positions up, down, left, right </param>
    public MazeGraph() {
        GetAdjacentPositions = GetOrtogonalPositions;
    }

    public void Clear() {
        Root = null;
        NodeGrid.Clear();
        Nodes.Clear();
        LastId = 0;
    }

    public bool IsValidPosition(Vector2I position) {
        return IsValidPositionFunc == null || IsValidPositionFunc(position);
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
        if (IsValidPositionFunc != null && !IsValidPositionFunc(position)) {
            throw new InvalidNodeException($"Invalid position {position} in {message}: {nameof(IsValidPositionFunc)} returned false", position);
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
        if (IsValidPositionFunc != null && !IsValidPositionFunc(position)) return null;
        return NodeGrid.GetValueOrDefault(position);
    }
    
    internal bool InternalRemoveNode(MazeNode node) {
        if (!Nodes.Remove(node.Id)) return false;
        NodeGrid.Remove(node.Position);
        OnNodeRemoved?.Invoke(node);
        return true;
    }

    public bool RemoveNode(int id) {
        if (!Nodes.TryGetValue(id, out var node)) return false;
        return node.RemoveNode();
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

    public bool IsValidEdge(Vector2I from, Vector2I to) {
        return IsValidPosition(from) && IsValidPosition(to) &&
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
}