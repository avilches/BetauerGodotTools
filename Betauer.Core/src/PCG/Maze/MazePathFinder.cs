using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Used by the FindWeightedPath method to determine which weights to consider
/// </summary>
public enum PathWeightMode {
    None,
    NodesOnly,
    EdgesOnly,
    Both
}

public static class MazePathFinder {
    public delegate float HeuristicFunction(MazeNode node, MazeNode target);

    private static float ManhattanDistance(MazeNode node, MazeNode target) {
        var dx = Math.Abs(node.Position.X - target.Position.X);
        var dy = Math.Abs(node.Position.Y - target.Position.Y);
        return dx + dy;
    }

    /// <summary>
    /// Finds the most efficient path considering node and/or connection weights.
    /// Useful when different paths have varying costs or difficulties.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="target">Destination node</param>
    /// <param name="mode">Weight calculation mode: nodes only, edges only, or both</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <param name="heuristic">Optional heuristic function for A* algorithm</param>
    /// <returns>Result containing the path and its total cost, or null if no path exists</returns>
    public static PathResult FindShortestPath(MazeNode start, MazeNode target, PathWeightMode mode = PathWeightMode.Both, Func<MazeNode, bool>? canTraverse = null, HeuristicFunction? heuristic = null) {
        if (start == target) {
            return new PathResult([start], mode is PathWeightMode.None or PathWeightMode.EdgesOnly ? 0 : start.Weight);
        }
        if (canTraverse != null && (!canTraverse(start) || !canTraverse(target))) return new PathResult([], -1);

        heuristic ??= ManhattanDistance;

        var distances = new Dictionary<MazeNode, float>();
        var previous = new Dictionary<MazeNode, MazeNode>();
        var unvisited = new PriorityQueue<MazeNode, float>();

        distances[start] = mode is PathWeightMode.None or PathWeightMode.EdgesOnly ? 0 : start.Weight;
        var initialCost = distances[start] + (heuristic?.Invoke(start, target) ?? 0);
        unvisited.Enqueue(start, initialCost);

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

                var g_score = distances[current];

                switch (mode) {
                    case PathWeightMode.None:
                        g_score += 1; // Incrementa 1 por cada paso
                        break;
                    case PathWeightMode.NodesOnly:
                        g_score += neighbor.Weight;
                        break;
                    case PathWeightMode.EdgesOnly:
                        g_score += edge.Weight;
                        break;
                    case PathWeightMode.Both:
                        g_score += edge.Weight + neighbor.Weight;
                        break;
                }

                if (!distances.ContainsKey(neighbor) || g_score < distances[neighbor]) {
                    distances[neighbor] = g_score;
                    previous[neighbor] = current;
                    var f_score = g_score + (heuristic?.Invoke(neighbor, target) ?? 0);
                    unvisited.Enqueue(neighbor, f_score);
                }
            }
        }
        return new PathResult([], -1);
    }

    /// <summary>
    /// Finds the shortest path to another node using direct connections (edges).
    /// If a predicate is provided, only nodes that satisfy the predicate will be considered.
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="target">Target node</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>List of nodes forming the shortest path, or empty list if no path exists</returns>
    public static List<MazeNode> FindBfsPath(MazeNode start, MazeNode target, Func<MazeNode, bool>? canTraverse = null) {
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
    public static IReadOnlyList<MazeNode> GetPathToNode(MazeNode start, MazeNode target) {
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

        if (commonAncestor == null) return ImmutableList<MazeNode>.Empty;

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