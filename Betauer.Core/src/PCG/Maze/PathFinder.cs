using System;
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

public static class PathFinder {
    /// <summary>
    /// Finds the most efficient path considering node and/or connection weights.
    /// Useful when different paths have varying costs or difficulties.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="target">Destination node</param>
    /// <param name="mode">Weight calculation mode: nodes only, edges only, or both</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>Result containing the path and its total cost, or null if no path exists</returns>
    public static PathResult? FindWeightedPath(MazeNode start, MazeNode target, PathWeightMode mode = PathWeightMode.Both, Func<MazeNode, bool>? canTraverse = null) {
        if (start == target) {
            return new PathResult([start], mode == PathWeightMode.EdgesOnly ? 0 : start.Weight);
        }
        if (canTraverse != null && (!canTraverse(start) || !canTraverse(target))) return null;

        var nodes = GetReachableNodes(start, canTraverse);
        var distances = nodes.ToDictionary(node => node, _ => float.MaxValue);
        var previous = new Dictionary<MazeNode, MazeNode>();
        var unvisited = new PriorityQueue<MazeNode, float>();

        distances[start] = mode == PathWeightMode.EdgesOnly ? 0 : start.Weight;
        unvisited.Enqueue(start, distances[start]);

        while (unvisited.Count > 0) {
            var current = unvisited.Dequeue();

            if (current == target) {
                var path = new List<MazeNode>();
                var node = current;

                while (previous.ContainsKey(node)) {
                    path.Add(node);
                    node = previous[node];
                }
                path.Add(start);
                path.Reverse();

                return new PathResult(path, distances[target]);
            }

            foreach (var edge in current.GetOutEdges()) {
                var neighbor = edge.To;
                if (canTraverse != null && !canTraverse(neighbor)) continue;

                var distance = distances[current];

                // Añadir coste según el modo
                if (mode == PathWeightMode.NodesOnly)
                    distance += neighbor.Weight;
                else if (mode == PathWeightMode.EdgesOnly)
                    distance += edge.Weight;
                else if (mode == PathWeightMode.Both)
                    distance += edge.Weight + neighbor.Weight;

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
    /// If a predicate is provided, only nodes that satisfy the predicate will be considered.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="target">Target node</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>List of nodes forming the shortest path, or empty list if no path exists</returns>
    public static List<MazeNode> FindShortestPath(MazeNode start, MazeNode target, Func<MazeNode, bool>? canTraverse = null) {
        if (start == target) return [start];
        if (canTraverse != null && (!canTraverse(start) || !canTraverse(target))) return [];

        var previous = new Dictionary<MazeNode, MazeNode>();
        var queue = new Queue<MazeNode>();
        var visited = new HashSet<MazeNode>(); // Añadimos un HashSet para track de nodos visitados

        queue.Enqueue(start);
        visited.Add(start); // Marcamos el nodo inicial como visitado

        while (queue.Count > 0) {
            var current = queue.Dequeue();

            if (current == target) {
                var path = new List<MazeNode>();
                var node = current;

                // Reconstruir el camino desde el target hasta el start
                while (node != start) { // Cambiamos la condición del while
                    path.Add(node);
                    node = previous[node];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (var edge in current.GetOutEdges()) {
                var neighbor = edge.To;
                if (canTraverse != null && !canTraverse(neighbor)) continue;

                if (visited.Add(neighbor)) { // Usamos visited en vez de previous
                    previous[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        return [];
    }

    /// <summary>
    /// Gets all nodes that can be reached from the start node using available connections.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>Set of all reachable nodes, including the starting node</returns>
    public static HashSet<MazeNode> GetReachableNodes(MazeNode start, Func<MazeNode, bool>? canTraverse = null) {
        var nodes = new HashSet<MazeNode> { start };
        var queue = new Queue<MazeNode>();
        queue.Enqueue(start);

        while (queue.Count > 0) {
            var current = queue.Dequeue();
            foreach (var edge in current.GetOutEdges()) {
                var neighbor = edge.To;
                if (canTraverse != null && !canTraverse(neighbor)) continue;

                if (nodes.Add(neighbor)) {
                    queue.Enqueue(neighbor);
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
    public static List<MazeNode> GetPathToNode(MazeNode start, MazeNode target) {
        if (start == target) return [start];

        // Obtener el camino hasta la raíz para ambos nodos
        var startPath = start.FindTreePathToRoot();
        var targetPath = target.FindTreePathToRoot();

        // Encontrar el ancestro común
        MazeNode? commonAncestor = null;
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

        if (commonAncestor == null) return [];

        // Construir el camino: subir desde start hasta el ancestro común y bajar hasta target
        var path = new List<MazeNode>();

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