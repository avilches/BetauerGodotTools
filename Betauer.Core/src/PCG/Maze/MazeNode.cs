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
    public object Metadata { get; set; }
    public float Weight { get; set; } = 0f;
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

    public MazeNode? Up => GetEdgeTo(Vector2I.Up)?.To;
    public MazeNode? Down => GetEdgeTo(Vector2I.Down)?.To;
    public MazeNode? Right => GetEdgeTo(Vector2I.Right)?.To;
    public MazeNode? Left => GetEdgeTo(Vector2I.Left)?.To;

    public object Metadata { get; set; }

    public float Weight { get; set; } = 0f;

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

    /// <summary>
    /// Gets the path from the current node to the root by following parent nodes.
    /// This is an implementation of a simple upward traversal using parent references.
    /// Time Complexity: O(h) where h is the height of the tree
    /// Space Complexity: O(h) to store the path
    /// </summary>
    /// <returns>A list of nodes representing the path from this node to the root, including both ends.</returns>
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
    /// Finds a path between two nodes by traversing up to find a common ancestor.
    /// This is an implementation of a Lowest Common Ancestor (LCA) algorithm.
    /// The algorithm:
    /// 1. Gets paths to root for both nodes
    /// 2. Finds the first common ancestor
    /// 3. Constructs the path by combining the upward path to ancestor and downward path to target
    /// Time Complexity: O(h1 * h2) where h1 and h2 are the heights of each node to root
    /// Space Complexity: O(h1 + h2) to store both paths
    /// </summary>
    /// <param name="target">The target node to find a path to</param>
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
    /// Uses the GetPathToNode method internally and counts the steps.
    /// Time Complexity: Same as GetPathToNode O(h1 * h2)
    /// Space Complexity: O(h1 + h2)
    /// </summary>
    /// <param name="target">The target node to calculate distance to</param>
    /// <returns>
    /// The distance in number of nodes:
    /// 0 = the target is itself
    /// -1 = no path exists
    /// n = number of nodes in between plus one
    /// </returns>
    public int GetDistanceToNode(MazeNode target) {
        var path = GetPathToNode(target);
        return path != null ? path.Count - 1 : -1;
    }

    /// <summary>
    /// Finds the shortest path to another node using edges (Breadth-First Search).
    /// BFS guarantees the shortest path in terms of number of edges when all edges have equal weight.
    /// The algorithm:
    /// 1. Uses a queue for BFS traversal
    /// 2. Maintains a visited dictionary to track the path
    /// 3. Reconstructs the path once target is found
    /// Time Complexity: O(V + E) where V is number of vertices and E is number of edges
    /// Space Complexity: O(V) for the visited dictionary and queue
    /// </summary>
    /// <param name="target">The target node to find a path to</param>
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
    /// Calculates the shortest distance to another node using edges.
    /// Uses FindShortestPath internally and returns the number of steps.
    /// Time Complexity: Same as FindShortestPath O(V + E)
    /// Space Complexity: O(V)
    /// </summary>
    /// <param name="target">The target node to calculate distance to</param>
    /// <returns>The distance in number of nodes, or -1 if no path exists</returns>
    public int GetDistanceToNodeByEdges(MazeNode target) {
        var path = FindShortestPath(target);
        return path != null ? path.Count - 1 : -1;
    }

    /// <summary>
    /// Gets all nodes that can be reached from this node using edges.
    /// Implements a Breadth-First Search to discover all connected nodes.
    /// Time Complexity: O(V + E) where V is number of vertices and E is number of edges
    /// Space Complexity: O(V) for the HashSet and queue
    /// </summary>
    /// <returns>A HashSet containing all reachable nodes, including the starting node</returns>
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
    /// Finds the shortest path considering node weights using Dijkstra's algorithm.
    /// Only considers node weights in path calculation.
    /// Time Complexity: O((V + E) * log V) where V is vertices and E is edges
    /// Space Complexity: O(V)
    /// </summary>
    /// <param name="target">The target node to find a path to</param>
    /// <returns>PathResult containing the path and total cost, or null if no path exists</returns>
    public PathResult? GetShortestPathByNodeWeights(MazeNode target) {
        return FindWeightedPath(target, PathWeightMode.NodesOnly);
    }

    /// <summary>
    /// Finds the shortest path considering edge weights using Dijkstra's algorithm.
    /// Only considers edge weights in path calculation.
    /// Time Complexity: O((V + E) * log V) where V is vertices and E is edges
    /// Space Complexity: O(V)
    /// </summary>
    /// <param name="target">The target node to find a path to</param>
    /// <returns>PathResult containing the path and total cost, or null if no path exists</returns>
    public PathResult? GetShortestPathByEdgeWeights(MazeNode target) {
        return FindWeightedPath(target, PathWeightMode.EdgesOnly);
    }

    /// <summary>
    /// Finds the shortest path considering both node and edge weights using Dijkstra's algorithm.
    /// Combines both node and edge weights for total path cost calculation.
    /// Time Complexity: O((V + E) * log V) where V is vertices and E is edges
    /// Space Complexity: O(V)
    /// </summary>
    /// <param name="target">The target node to find a path to</param>
    /// <returns>PathResult containing the path and total cost, or null if no path exists</returns>
    public PathResult? FindWeightedPath(MazeNode target) {
        return FindWeightedPath(target, PathWeightMode.Both);
    }
    

    /// <summary>
    /// Private helper method that implements Dijkstra's algorithm with different weight modes.
    /// Time Complexity: O((V + E) * log V) where V is vertices and E is edges
    /// Space Complexity: O(V)
    /// </summary>
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
}

public class PathResult(List<MazeNode> path, float totalCost) {
    public List<MazeNode> Path { get; } = path;
    public float TotalCost { get; } = totalCost;

    // Método auxiliar para calcular solo el coste de los edges
    public float GetEdgesCost() {
        float cost = 0;
        for (var i = 0; i < Path.Count - 1; i++) {
            var edge = Path[i].GetEdgeTo(Path[i + 1]);
            if (edge != null) cost += edge.Weight;
        }
        return cost;
    }

    // Método auxiliar para calcular solo el coste de los nodos
    public float GetNodesCost() {
        return Path.Sum(node => node.Weight);
    }
}