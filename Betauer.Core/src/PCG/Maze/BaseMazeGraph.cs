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
            throw new InvalidOperationException("Can't create node at " + position + ". Invalid position");
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
        OnNodeConnected?.Invoke(edge);
    }

    public bool IsValidEdge(Vector2I from, Vector2I to) {
        // No need to validate if the nodes are valid, as the edge is created between valid nodes.
        return IsValidEdgeFunc(from, to);
    }

    public IEnumerable<MazeNode> GetAdjacentNodes(Vector2I from) {
        return GetAdjacentPositions(from)
            .Where(IsValidPosition)
            .Select(pos => NodeGrid[pos])
            .Where(node => node != null);
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
    public List<(MazeNode nodeA, MazeNode nodeB, int distance)> FindPotentialCycles(int minDistance = 3, bool useParentDistance = false) {
        var potentialConnections = new List<(MazeNode nodeA, MazeNode nodeB, int distance)>();
        var addedConnections = new HashSet<(Vector2I, Vector2I)>();

        foreach (var nodeA in Nodes.Values) {
            foreach (var nodeB in GetAdjacentNodes(nodeA.Position)) {
                // Skip if nodes are already connected
                if (nodeA.HasEdgeTo(nodeB)) continue;

                // Create ordered tuple to avoid duplicates (A->B and B->A are the same connection)
                var posA = nodeA.Position;
                var posB = nodeB.Position;
                var connection = posA.X < posB.X || (posA.X == posB.X && posA.Y < posB.Y)
                    ? (posA, posB)
                    : (posB, posA);

                if (addedConnections.Contains(connection)) continue;

                // Calculate distance based on selected mode
                var distance = useParentDistance
                    ? nodeA.GetDistanceToNode(nodeB)
                    : nodeA.GetDistanceToNodeByEdges(nodeB);

                if (distance >= minDistance) {
                    potentialConnections.Add((nodeA, nodeB, distance));
                    addedConnections.Add(connection);
                }
            }
        }

        return potentialConnections.OrderByDescending(x => x.distance).ToList();
    }

    /// <summary>
    /// Adds cycles to the maze by connecting nodes that are far apart when following parent relationships.
    /// Since adding cycles doesn't modify parent relationships, all valid connections are found at once
    /// and the longest ones are selected up to maxCycles.
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
    /// When using parent distance, new connections don't affect the parent relationships,
    /// so all potential cycles can be found at once and we can select the N longest ones
    /// if maxCycles is specified.
    /// 
    /// Usage:
    /// // Add up to 5 cycles where nodes are at least 4 steps apart through parent relationships
    /// maze.AddCyclesByParentDistance(maxCycles: 5, minDistance: 4);
    /// 
    /// // Add all possible cycles with minimum parent distance of 4
    /// maze.AddCyclesByParentDistance(minDistance: 4);
    /// </summary>
    /// <param name="maxCycles">Maximum number of cycles to add. If null, all valid cycles will be added.</param>
    /// <param name="minDistance">Minimum parent-path distance required between nodes to consider creating a cycle.</param>
    public void AddCyclesByParentDistance(int? maxCycles = null, int minDistance = 3) {
        var connections = FindPotentialCycles(minDistance, useParentDistance: true);
    
        // If maxCycles is specified, take only the N longest paths
        if (maxCycles.HasValue) {
            connections = connections.Take(maxCycles.Value).ToList();
        }
    
        foreach (var (nodeA, nodeB, _) in connections) {
            ConnectNodes(nodeA, nodeB, true);
        }
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
    public void AddCyclesByEdgeDistance(int maxCycles, int minDistance = 3) {
        var cyclesAdded = 0;
        while (cyclesAdded < maxCycles) {
            var connections = FindPotentialCycles(minDistance, useParentDistance: false);
            if (connections.Count == 0) break;

            var (nodeA, nodeB, _) = connections[0];
            ConnectNodes(nodeA, nodeB, true);
            cyclesAdded++;
        }
    }
}