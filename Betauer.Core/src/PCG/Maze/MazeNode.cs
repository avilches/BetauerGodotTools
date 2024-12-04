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
    /// Encuentra el camino hacia otro nodo navegando por los parents hasta encontrar un ancestro común
    /// </summary>
    /// <returns>Lista de nodos que forman el camino, o null si no hay camino</returns>
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
    /// Calcula la distancia hacia otro nodo usando los parents
    /// </summary>
    /// <returns>La distancia en número de nodos:
    /// 0 = el target es si mismo
    /// -1 = no hay camino</returns>
    public int GetDistanceToNode(MazeNode target) {
        var path = GetPathToNode(target);
        return path != null ? path.Count - 1 : -1;
    }

    /// <summary>
    /// Encuentra el camino más corto hacia otro nodo usando los edges (BFS)
    /// </summary>
    /// <returns>Lista de nodos que forman el camino, o null si no hay camino</returns>
    public List<MazeNode>? GetPathToNodeByEdges(MazeNode target) {
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
    /// Calcula la distancia más corta hacia otro nodo usando los edges
    /// </summary>
    /// <returns>La distancia en número de nodos, o -1 si no hay camino</returns>
    public int GetDistanceToNodeByEdges(MazeNode target) {
        var path = GetPathToNodeByEdges(target);
        return path != null ? path.Count - 1 : -1;
    }

    /// <summary>
    /// Obtiene todos los nodos alcanzables desde este nodo
    /// </summary>
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

    public enum PathWeightMode {
        NodesOnly, // Solo considera los pesos de los nodos
        EdgesOnly, // Solo considera los pesos de los edges
        Both // Considera tanto los pesos de los nodos como de los edges
    }

    /// <summary>
    /// Encuentra el camino más corto hacia otro nodo considerando diferentes modos de peso
    /// </summary>
    private PathResult? GetShortestPath(MazeNode target, PathWeightMode mode) {
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

    public PathResult? GetShortestPathByNodeWeights(MazeNode target) {
        return GetShortestPath(target, PathWeightMode.NodesOnly);
    }

    public PathResult? GetShortestPathByEdges(MazeNode target) {
        return GetShortestPath(target, PathWeightMode.EdgesOnly);
    }

    public PathResult? GetShortestPathByWeights(MazeNode target) {
        return GetShortestPath(target, PathWeightMode.Both);
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