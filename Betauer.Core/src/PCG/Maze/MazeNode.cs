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
public class MazeEdge(MazeNode from, MazeNode to, float weight = 0f) {
    public MazeNode From { get; } = from ?? throw new ArgumentNullException(nameof(from));
    public MazeNode To { get; } = to ?? throw new ArgumentNullException(nameof(to));
    public Vector2I Direction => To.Position - From.Position;
    public object Metadata { get; set; }
    public float Weight { get; set; } = weight;
}

/// <summary>
/// Used by the FindWeightedPath method to determine which weights to consider
/// </summary>
public enum PathWeightMode {
    NodesOnly,
    EdgesOnly,
    Both
}

/// <summary>
/// Represents a node in the maze graph, containing connections to adjacent nodes.
/// </summary>
public class MazeNode {
    /// <summary>
    /// Represents a node in the maze graph, containing connections to adjacent nodes.
    /// </summary>
    internal MazeNode(int id, Vector2I position) {
        Id = id;
        Position = position;
    }

    public int Id { get; }
    public Vector2I Position { get; }
    public MazeNode? Parent { get; set; }
    private readonly List<MazeEdge> _edges = [];
    public int Zone { get; set; }

    public MazeNode? Up => GetEdgeTowards(Vector2I.Up)?.To;
    public MazeNode? Down => GetEdgeTowards(Vector2I.Down)?.To;
    public MazeNode? Right => GetEdgeTowards(Vector2I.Right)?.To;
    public MazeNode? Left => GetEdgeTowards(Vector2I.Left)?.To;

    public object Metadata { get; set; }

    public float Weight { get; set; } = 0f;

    /// <summary>
    /// Creates a connection between this node and another node in the specified direction.
    /// </summary>
    /// <param name="node">The node to connect to.</param>
    /// <param name="weight">The weigth.</param>
    /// <returns>The created edge.</returns>
    public MazeEdge AddEdgeTo(MazeNode node, float weight = 0f) {
        var edge = GetEdgeTo(node);
        if (edge != null) {
            edge.Weight = weight;
            return edge;
        }
        edge = new MazeEdge(this, node, weight);
        _edges.Add(edge);
        return edge;
    }

    public MazeEdge? GetEdgeTo(MazeNode to) {
        return _edges.FirstOrDefault(edge => edge.To == to);
    }

    public bool HasEdgeTo(MazeNode other) {
        return GetEdgeTo(other) != null;
    }

    public MazeEdge? GetEdgeTowards(Vector2I direction) {
        return _edges.FirstOrDefault(edge => edge.Direction == direction);
    }

    public bool HasEdgeTowards(Vector2I direction) {
        return GetEdgeTowards(direction) != null;
    }

    public void RemoveEdgeTo(MazeNode node) {
        var edge = GetEdgeTo(node);
        if (edge != null) {
            _edges.Remove(edge);
        }
    }

    public void RemoveEdgeTowards(Vector2I direction) {
        var edge = GetEdgeTowards(direction);
        if (edge != null) {
            _edges.Remove(edge);
        }
    }

    public void RemoveEdge(MazeEdge edge) {
        _edges.Remove(edge);
    }

    public ImmutableList<MazeEdge> GetEdges() {
        return _edges.ToImmutableList();
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
    public List<MazeNode> GetPathToRoot() {
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
    public List<MazeNode>? GetPathToNode(MazeNode target) {
        if (target == this) return [this];

        // Obtener el camino hasta la raíz para ambos nodos
        var thisPath = GetPathToRoot();
        var targetPath = target.GetPathToRoot();

        // Encontrar el ancestro común
        MazeNode? commonAncestor = null;
        int thisIndex = 0, targetIndex = 0;

        // Buscar el ancestro común buscando en todos los nodos del path
        for (var i = 0; i < thisPath.Count; i++) {
            var nodeInThisPath = thisPath[i];
            for (var j = 0; j < targetPath.Count; j++) {
                if (nodeInThisPath == targetPath[j]) {
                    commonAncestor = nodeInThisPath;
                    thisIndex = i;
                    targetIndex = j;
                    goto CommonAncestorFound; // Salir de ambos loops cuando encontremos el primer ancestro común
                }
            }
        }

        CommonAncestorFound:

        if (commonAncestor == null) return null;

        // Construir el camino: subir desde this hasta el ancestro común y bajar hasta target
        var path = new List<MazeNode>();

        // Añadir el camino desde this hasta el ancestro común (en orden inverso)
        for (var i = 0; i <= thisIndex; i++) {
            path.Add(thisPath[i]);
        }

        // Añadir el camino desde el ancestro común hasta target (en orden normal)
        for (var i = targetIndex - 1; i >= 0; i--) {
            path.Add(targetPath[i]);
        }
        return path;
    }

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
    public int GetDistanceToNode(MazeNode target) {
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
    public List<MazeNode>? FindShortestPath(MazeNode target) {
        if (target == this) return [this];

        // Inicializamos el diccionario con todos los nodos alcanzables
        var visited = GetReachableNodes()
            .ToDictionary(node => node, _ => (MazeNode?)null);

        var queue = new Queue<MazeNode>();
        queue.Enqueue(this);

        while (queue.Count > 0) {
            var current = queue.Dequeue();

            if (current == target) {
                // Reconstruir el camino
                var path = new List<MazeNode>();
                var node = current;

                while (node != null) {
                    path.Add(node);
                    node = visited[node];
                }

                path.Reverse();
                return path;
            }

            // Explorar todos los nodos conectados por edges
            foreach (var edge in current.GetEdges()) {
                var next = edge.To;
                if (visited[next] == null && next != this) {
                    visited[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        return null;
    }

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
    public int GetDistanceToNodeByEdges(MazeNode target) {
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
    public HashSet<MazeNode> GetReachableNodes() {
        var nodes = new HashSet<MazeNode> { this };
        var queue = new Queue<MazeNode>();
        queue.Enqueue(this);

        while (queue.Count > 0) {
            var current = queue.Dequeue();
            foreach (var edge in current.GetEdges()) {
                if (nodes.Add(edge.To)) {
                    queue.Enqueue(edge.To);
                }
            }
        }
        return nodes;
    }

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
    public PathResult? FindWeightedPath(MazeNode target, PathWeightMode mode = PathWeightMode.Both) {
        if (target == this) {
            return new PathResult([this], mode == PathWeightMode.EdgesOnly ? 0 : Weight);
        }

        var nodes = GetReachableNodes();
        var distances = nodes.ToDictionary(node => node, _ => float.MaxValue);
        var previous = new Dictionary<MazeNode, MazeNode>();
        var unvisited = new PriorityQueue<MazeNode, float>();

        distances[this] = mode == PathWeightMode.EdgesOnly ? 0 : Weight;
        unvisited.Enqueue(this, distances[this]);

        while (unvisited.Count > 0) {
            var current = unvisited.Dequeue();

            if (current == target) {
                var path = new List<MazeNode>();
                var node = current;

                while (previous.ContainsKey(node)) {
                    path.Add(node);
                    node = previous[node];
                }
                path.Add(this);
                path.Reverse();

                return new PathResult(path, distances[target]);
            }

            foreach (var edge in current.GetEdges()) {
                var neighbor = edge.To;
                var distance = distances[current];

                // Añadir coste según el modo
                switch (mode) {
                    case PathWeightMode.NodesOnly:
                        distance += neighbor.Weight;
                        break;
                    case PathWeightMode.EdgesOnly:
                        distance += edge.Weight;
                        break;
                    case PathWeightMode.Both:
                        distance += edge.Weight + neighbor.Weight;
                        break;
                }

                if (distance < distances[neighbor]) {
                    distances[neighbor] = distance;
                    previous[neighbor] = current;
                    unvisited.Enqueue(neighbor, distance);
                }
            }
        }

        return null;
    }
    
    public override string ToString() {
        return $"Id:{Id} {Position}";
    }
}

public class PathResult(List<MazeNode> path, float totalCost) {
    public List<MazeNode> Path { get; } = path;
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