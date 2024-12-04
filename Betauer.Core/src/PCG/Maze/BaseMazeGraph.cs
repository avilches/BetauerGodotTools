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

    public void DisconnectNodes(int fromId, int toId, bool bidirectional = false) {
        var from = GetNode(fromId);
        var to = GetNode(toId);
        from.RemoveEdgeTo(to, bidirectional);
    }

    public void DisconnectNodes(Vector2I fromPos, Vector2I toPos, bool bidirectional = false) {
        var from = GetNodeAt(fromPos)!;
        var to = GetNodeAt(toPos)!;
        from.RemoveEdgeTo(to, bidirectional);
    }

    public void DisconnectNodes(MazeNode from, MazeNode to, bool bidirectional = false) {
        from.RemoveEdgeTo(to, bidirectional);
    }

    public List<MazeEdge> ConnectNodes(int fromId, int toId, bool bidirectional = false) {
        var from = GetNode(fromId);
        var to = GetNode(toId);
        _ConnectNode(from, to);
        return ConnectNodes(from, to, bidirectional);
    }

    public List<MazeEdge> ConnectNodes(Vector2I fromPos, Vector2I toPos, bool bidirectional = false) {
        var from = GetNodeAt(fromPos)!;
        var to = GetNodeAt(toPos)!;
        return ConnectNodes(from, to, bidirectional);
    }

    public List<MazeEdge> ConnectNodes(MazeNode from, MazeNode to, bool bidirectional = false) {
        var edges = new List<MazeEdge>(2) {
            _ConnectNode(from, to)
        };
        if (bidirectional) {
            edges.Add(_ConnectNode(to, from));
        }
        return edges;
    }

    public List<MazeEdge> ConnectNodeTowards(int fromId, Vector2I direction, bool bidirectional = false) {
        var from = GetNode(fromId);
        return ConnectNodeTowards(from, direction, bidirectional);
    }

    public List<MazeEdge> ConnectNodeTowards(MazeNode from, Vector2I direction, bool bidirectional = false) {
        return ConnectNodeTo(from, from.Position + direction, bidirectional);
    }

    public List<MazeEdge> ConnectNodeTo(int fromId, Vector2I target, bool bidirectional = false) {
        var from = GetNode(fromId);
        return ConnectNodeTo(from, target, bidirectional);
    }

    public List<MazeEdge> ConnectNodeTo(MazeNode from, Vector2I target, bool bidirectional = false) {
        var to = GetNodeAt(target);
        return ConnectNodes(from, to, bidirectional);
    }

    private MazeEdge _ConnectNode(MazeNode from, MazeNode to) {
        var edgeTo = from.GetEdgeTo(to);
        if (edgeTo != null) {
            // Just ignore duplicated edges
            return edgeTo;
        }
        if (IsValidEdgeFunc != null && !IsValidEdgeFunc(from.Position, to.Position)) {
            throw new InvalidEdgeException($"{nameof(IsValidEdgeFunc)} returned false", from.Position, to.Position);
        }
        var edge = from.AddEdgeTo(to);
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
    /// <param name="minDistance">Minimum distance required between nodes to consider creating a cycle.</param>
    /// <param name="useParentDistance">If true, calculates distance following parent relationships.
    /// If false, finds the shortest path using all available connections.</param>
    /// <returns>List of potential connections ordered by distance (longest paths first)</returns>
    public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> FindPotentialCycles(int minDistance = 3, bool useParentDistance = false) {
        var cycles = new PotentialCycles(this, minDistance, useParentDistance);
        return cycles.UpdateDistanceAndGetCycles();
    }

    /// <summary>
    /// Adds a limited number of cycles to the maze by connecting nodes that are far apart through the shortest path.
    /// After each connection is made, distances are recalculated as new connections can create shorter paths.
    /// 
    /// Example: Consider a maze structure where solid lines are connections and arrows show parent relationships:
    ///   A -> B -> C
    ///   ↓         ↓
    ///   D -> E    F
    /// 
    /// Initially:
    /// - B and E are 3 steps apart (B->A->D->E)
    /// - C and E are 4 steps apart (C->B->A->D->E)
    /// 
    /// After adding B-E connection:
    /// - C and E become 2 steps apart (C->B-E)
    /// 
    /// This is why we recalculate after each connection: the new B-E connection
    /// made C-E less interesting as a potential cycle.
    /// 
    /// Usage:
    /// // Add up to 5 cycles, requiring minimum path length of 3
    /// maze.AddCyclesByEdgeDistance(maxCycles: 5, minDistance: 3);
    /// </summary>
    /// <param name="maxCycles">Maximum number of cycles to add.</param>
    /// <param name="minDistance">Minimum path length required between nodes to consider creating a cycle.</param>
    public List<MazeEdge> AddCyclesByEdgeDistance(int maxCycles, int minDistance = 3) {
        var cycles = new PotentialCycles(this, minDistance, useParentDistance: false);
        var cyclesAdded = 0;
        var edges = new List<MazeEdge>(maxCycles);
        while (cyclesAdded < maxCycles) {
            var connection = cycles.UpdateDistanceAndGetCycles().FirstOrDefault();
            if (connection == default) break;

            edges.AddRange(ConnectNodes(connection.nodeA, connection.nodeB, true));
            cycles.RemoveCycle(connection.nodeA, connection.nodeB);
            cyclesAdded++;
        }
        return edges;
    }
}

public class PotentialCycles {
    private readonly int _minDistance;
    private readonly bool _useParentDistance;
    private readonly List<(MazeNode nodeA, MazeNode nodeB, (Vector2I, Vector2I) connection)> _potentialConnections;

    public PotentialCycles(BaseMazeGraph graph, int minDistance, bool useParentDistance) {
        _minDistance = minDistance;
        _useParentDistance = useParentDistance;
        // Get all potential cycles once
        _potentialConnections = graph.Nodes.Values
            .SelectMany(nodeA =>
                graph.GetAdjacentNodes(nodeA.Position)
                    .Where(nodeB => !nodeA.HasEdgeTo(nodeB))
                    .Select(nodeB => {
                        var posA = nodeA.Position;
                        var posB = nodeB.Position;
                        var connection = posA.X < posB.X || (posA.X == posB.X && posA.Y < posB.Y)
                            ? (posA, posB)
                            : (posB, posA);
                        return (nodeA, nodeB, connection);
                    }))
            .DistinctBy(x => x.connection)
            .ToList();
    }

    public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> UpdateDistanceAndGetCycles() {
        return _potentialConnections
            .Select(x => {
                var distance = _useParentDistance
                    ? x.nodeA.GetDistanceToNode(x.nodeB)
                    : x.nodeA.GetDistanceToNodeByEdges(x.nodeB);
                return (x.nodeA, x.nodeB, distance);
            })
            .Where(x => x.distance >= _minDistance)
            .OrderByDescending(x => x.distance);
    }

    public void RemoveCycle(MazeNode nodeA, MazeNode nodeB) {
        _potentialConnections.RemoveAll(x =>
            (x.nodeA == nodeA && x.nodeB == nodeB) ||
            (x.nodeA == nodeB && x.nodeB == nodeA));
    }
}