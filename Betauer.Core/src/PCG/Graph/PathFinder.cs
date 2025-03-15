using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Betauer.Core.PCG.Maze;

namespace Betauer.Core.PCG.Graph;

/// <summary>
/// Used by the FindWeightedPath method to determine which weights to consider
/// </summary>
public enum PathWeightMode {
    None,
    NodesOnly,
    EdgesOnly,
    Both
}

public static class PathFinder {
    public delegate float HeuristicFunction<in TNode>(TNode node, TNode target) where TNode : class;

    /// <summary>
    /// Finds the most efficient path considering node and/or connection weights.
    /// </summary>
    /// <typeparam name="TNode">The node type</typeparam>
    /// <typeparam name="TEdge">The edge type</typeparam>
    /// <param name="start">Starting node</param>
    /// <param name="target">Destination node</param>
    /// <param name="mode">Weight calculation mode: nodes only, edges only, or both</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <param name="heuristic">Optional heuristic function for A* algorithm</param>
    /// <returns>Result containing the path and its total cost</returns>
    public static PathResult<TNode, TEdge> FindShortestPath<TNode, TEdge>(
        TNode start,
        TNode target,
        PathWeightMode mode = PathWeightMode.Both,
        Func<TNode, bool>? canTraverse = null,
        HeuristicFunction<TNode>? heuristic = null)
        where TNode : class, IGraphNode<TNode, TEdge>
        where TEdge : IGraphEdge<TNode> {
        if (start == target) {
            return new PathResult<TNode, TEdge>([start], mode is PathWeightMode.None or PathWeightMode.EdgesOnly ? 0 : start.Weight);
        }
        if (canTraverse != null && (!canTraverse(start) || !canTraverse(target))) return new PathResult<TNode, TEdge>([], -1);

        heuristic ??= PathHeuristics.Manhattan<TNode, TEdge>;

        var distances = new Dictionary<TNode, float>();
        var previous = new Dictionary<TNode, TNode>();
        var unvisited = new PriorityQueue<TNode, float>();

        distances[start] = mode is PathWeightMode.None or PathWeightMode.EdgesOnly ? 0 : start.Weight;
        var initialCost = distances[start] + (heuristic?.Invoke(start, target) ?? 0);
        unvisited.Enqueue(start, initialCost);

        while (unvisited.Count > 0) {
            var current = unvisited.Dequeue();

            if (current == target) {
                var path = new List<TNode>();
                var node = current;
                while (previous.ContainsKey(node)) {
                    path.Add(node);
                    node = previous[node];
                }
                path.Add(start);
                path.Reverse();
                return new PathResult<TNode, TEdge>(path, distances[target]);
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
        return new PathResult<TNode, TEdge>([], -1);
    }

    /// <summary>
    /// Finds the shortest path using breadth-first search.
    /// </summary>
    /// <typeparam name="TNode">The node type</typeparam>
    /// <typeparam name="TEdge">The edge type</typeparam>
    /// <param name="start">Starting node</param>
    /// <param name="target">Target node</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>List of nodes forming the shortest path, or empty list if no path exists</returns>
    public static List<TNode> FindBfsPath<TNode, TEdge>(
        TNode start,
        TNode target,
        Func<TNode, bool>? canTraverse = null)
        where TNode : class, IGraphNode<TNode, TEdge>
        where TEdge : IGraphEdge<TNode> {
        if (start == target) return [start];
        if (canTraverse != null && (!canTraverse(start) || !canTraverse(target))) return [];

        var previous = new Dictionary<TNode, TNode>();
        var queue = new Queue<TNode>();
        var visited = new HashSet<TNode>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0) {
            var current = queue.Dequeue();

            if (current == target) {
                var path = new List<TNode>();
                var node = current;

                while (node != start) {
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

                if (visited.Add(neighbor)) {
                    previous[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        return [];
    }

    /// <summary>
    /// Gets all nodes that can be reached from the start node.
    /// </summary>
    /// <typeparam name="TNode">The node type</typeparam>
    /// <typeparam name="TEdge">The edge type</typeparam>
    /// <param name="start">Starting node</param>
    /// <param name="canTraverse">Optional predicate that determines if a node can be traversed</param>
    /// <returns>Set of all reachable nodes, including the starting node</returns>
    public static HashSet<TNode> GetReachableNodes<TNode, TEdge>(
        TNode start,
        Func<TNode, bool>? canTraverse = null)
        where TNode : class, IGraphNode<TNode, TEdge>
        where TEdge : IGraphEdge<TNode> {
        var nodes = new HashSet<TNode> { start };
        var queue = new Queue<TNode>();
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

    public static List<TNode> FindTreePathToRoot<TNode>(TNode start) where TNode : ITreeNode<TNode> {
        var path = new List<TNode>();
        var current = start;
        while (current != null) {
            path.Add(current);
            current = current.Parent;
        }
        return path;
    }

    /// <summary>
    /// Finds a path between two nodes using the parent-child hierarchy.
    /// </summary>
    /// <typeparam name="TNode">The node type</typeparam>
    /// <typeparam name="TEdge">The edge type</typeparam>
    /// <param name="start">Starting node</param>
    /// <param name="target">Target node</param>
    /// <returns>List of nodes forming the path, or an empty list if no path exists</returns>
    public static IReadOnlyList<TNode> GetPathToNode<TNode, TEdge>(
        TNode start,
        TNode target)
        where TNode : class, ITreeNode<TNode> {
        if (start == target) return [start];

        // Obtener el camino hasta la raíz para ambos nodos
        var startPath = FindTreePathToRoot(start);
        var targetPath = FindTreePathToRoot(target);

        // Encontrar el ancestro común
        TNode? commonAncestor = null;
        int startIndex = 0, targetIndex = 0;

        // Buscar el ancestro común buscando en todos los nodos del path
        for (var i = 0; i < startPath.Count; i++) {
            var nodeInStartPath = startPath[i];
            for (var j = 0; j < targetPath.Count; j++) {
                if (nodeInStartPath.Equals(targetPath[j])) {
                    commonAncestor = nodeInStartPath;
                    startIndex = i;
                    targetIndex = j;
                    goto CommonAncestorFound; // Salir de ambos loops cuando encontremos el primer ancestro común
                }
            }
        }

        CommonAncestorFound:

        if (commonAncestor == null) return ImmutableList<TNode>.Empty;

        // Construir el camino: subir desde start hasta el ancestro común y bajar hasta target
        var path = new List<TNode>();

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