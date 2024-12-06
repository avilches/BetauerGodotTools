using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Used by the FindWeightedPath method to determine which weights to consider
/// </summary>
public enum PathWeightMode {
    NodesOnly,
    EdgesOnly,
    Both
}


public class PathFinder<T> {
    /// <summary>
    /// Finds the most efficient path considering node and/or connection weights.
    /// Useful when different paths have varying costs or difficulties.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="target">Destination node</param>
    /// <param name="mode">Weight calculation mode: nodes only, edges only, or both</param>
    /// <returns>Result containing the path and its total cost, or null if no path exists</returns>
    public PathResult<T>? FindWeightedPath(MazeNode<T> start, MazeNode<T> target, PathWeightMode mode = PathWeightMode.Both) {
        if (start == target) {
            return new PathResult<T>([start], mode == PathWeightMode.EdgesOnly ? 0 : start.Weight);
        }

        var nodes = GetReachableNodes(start);
        var distances = nodes.ToDictionary(node => node, _ => float.MaxValue);
        var previous = new Dictionary<MazeNode<T>, MazeNode<T>>();
        var unvisited = new PriorityQueue<MazeNode<T>, float>();

        distances[start] = mode == PathWeightMode.EdgesOnly ? 0 : start.Weight;
        unvisited.Enqueue(start, distances[start]);

        while (unvisited.Count > 0) {
            var current = unvisited.Dequeue();

            if (current == target) {
                var path = new List<MazeNode<T>>();
                var node = current;

                while (previous.ContainsKey(node)) {
                    path.Add(node);
                    node = previous[node];
                }
                path.Add(start);
                path.Reverse();

                return new PathResult<T>(path, distances[target]);
            }

            foreach (var edge in current.GetOutEdges()) {
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

    /// <summary>
    /// Finds the shortest path to another node using direct connections (edges).
    /// Unlike GetPathToNode, this considers all available connections,
    /// not just the parent-child hierarchy.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="target">Target node</param>
    /// <returns>List of nodes forming the shortest path, or null if no path exists</returns>
    public List<MazeNode<T>>? FindShortestPath(MazeNode<T> start, MazeNode<T> target) {
        if (start == target) return [start];

        // Inicializamos el diccionario con todos los nodos alcanzables
        var visited = GetReachableNodes(start)
            .ToDictionary(node => node, MazeNode<T>? (_) => null);

        var queue = new Queue<MazeNode<T>>();
        queue.Enqueue(start);

        while (queue.Count > 0) {
            var current = queue.Dequeue();

            if (current == target) {
                // Reconstruir el camino
                var path = new List<MazeNode<T>>();
                var node = current;

                while (node != null) {
                    path.Add(node);
                    node = visited[node];
                }

                path.Reverse();
                return path;
            }

            // Explorar todos los nodos conectados por edges
            foreach (var edge in current.GetOutEdges()) {
                var next = edge.To;
                if (visited[next] == null && next != start) {
                    visited[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all nodes that can be reached from the start node using available connections.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <returns>Set of all reachable nodes, including the starting node</returns>
    public HashSet<MazeNode<T>> GetReachableNodes(MazeNode<T> start) {
        var nodes = new HashSet<MazeNode<T>> { start };
        var queue = new Queue<MazeNode<T>>();
        queue.Enqueue(start);

        while (queue.Count > 0) {
            var current = queue.Dequeue();
            foreach (var edge in current.GetOutEdges()) {
                if (nodes.Add(edge.To)) {
                    queue.Enqueue(edge.To);
                }
            }
        }
        return nodes;
    }

    /// <summary>
    /// Finds a path between two nodes using the parent-child hierarchy.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="target">Target node</param>
    /// <returns>List of nodes forming the path, or null if no path exists</returns>
    public List<MazeNode<T>>? GetPathToNode(MazeNode<T> start, MazeNode<T> target) {
        if (start == target) return [start];

        // Obtener el camino hasta la raíz para ambos nodos
        var startPath = start.GetPathToRoot();
        var targetPath = target.GetPathToRoot();

        // Encontrar el ancestro común
        MazeNode<T>? commonAncestor = null;
        int startIndex = 0, targetIndex = 0;

        // Buscar el ancestro común buscando en todos los nodos del path
        for (var i = 0; i < startPath.Count; i++) {
            var nodeInStartPath = startPath[i];
            for (var j = 0; j < targetPath.Count; j++) {
                if (nodeInStartPath == targetPath[j]) {
                    commonAncestor = nodeInStartPath;
                    startIndex = i;
                    targetIndex = j;
                    goto CommonAncestorFound; // Salir de ambos loops cuando encontremos el primer ancestro común
                }
            }
        }

        CommonAncestorFound:

        if (commonAncestor == null) return null;

        // Construir el camino: subir desde start hasta el ancestro común y bajar hasta target
        var path = new List<MazeNode<T>>();

        // Añadir el camino desde start hasta el ancestro común (en orden)
        for (var i = 0; i <= startIndex; i++) {
            path.Add(startPath[i]);
        }

        // Añadir el camino desde el ancestro común hasta target (en orden normal)
        for (var i = targetIndex - 1; i >= 0; i--) {
            path.Add(targetPath[i]);
        }
        return path;
    }
}