using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Betauer.Core.PCG.Graph;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeSolutionScoring {
    /// <summary>
    /// Location of keys in the maze, indexed by zone ID.
    /// - KeyLocations[0] = the node in the zone 0 with the key to open the zone 1.
    /// - KeyLocations[lastZone] = the "goal"
    /// </summary>
    public List<MazeNode> KeyLocations { get; }

    /// <summary>
    /// The complete path taken through the maze, including all nodes visited and revisited in order.
    /// For example: if a path goes A->B->C->B->D, SolutionPath will contain [A,B,C,B,D]
    /// </summary>
    public List<MazeNode> SolutionPath { get; }

    /// <summary>
    /// The shortest possible path to the final destination, ignoring zone restrictions and keys. It's the path the player would take if the player had all the
    /// keys to open all the zones since the beginning.
    /// Useful for comparing with SolutionPath to evaluate path efficiency.
    /// Example: if SolutionPath is [A,B,C,B,D] and GoalPath is [A,B,D], 
    /// it shows that revisiting B and visiting C were necessary due to zone constraints (the player needs to pick every key in order)
    /// </summary>
    public List<MazeNode> GoalPath { get; }

    /// <summary>
    /// Groups nodes by how many times they were visited during solution traversal.
    /// Key: number of visits, Value: list of nodes with that visit count.
    /// Example:
    /// - NodesByRevisit[0] = Nodes never visited (0 visits, not part of the solution path)
    /// - NodesByRevisit[1] = Nodes visited once
    /// - NodesByRevisit[2] = Nodes visited twice
    ///
    /// The [0] key is used for nodes that were never visited, so they are not part of the solution path.
    /// </summary>
    public Dictionary<int, List<MazeNode>> NodesByVisit { get; }

    /// <summary>
    /// Percentage of nodes that have each visit count, relative to total maze nodes.
    /// Key: number of visits, Value: percentage of total nodes with that visit count.
    /// Example for a maze with 10 nodes:
    /// - VisitDistribution[0] = 0.4 // 40% of nodes were never visited
    /// - VisitDistribution[1] = 0.5 // 50% of nodes were visited once
    /// - VisitDistribution[2] = 0.1 // 10% of nodes were visited twice
    /// </summary>
    public Dictionary<int, float> VisitDistribution { get; }

    /// <summary>
    /// Measures how evenly distributed the visits are across nodes using the Gini coefficient.
    /// - 0.0 means perfectly even distribution (all visited nodes have same visit count)
    /// - 1.0 means maximum concentration (one node has all the revisits)
    /// Example in a 4 node solution, where this array represents the number of visits per node:
    /// - [1,1,1,1] = 0.0 (all nodes visited once)
    /// - [1,1,2,2] ≈ 0.25 (balanced distribution)
    /// - [1,1,1,3] ≈ 0.5 (moderate concentration)
    /// - [1,1,1,10] ≈ 0.75 (high concentration)
    /// </summary>
    public float ConcentrationIndex { get; }

    /// <summary>
    /// Measures the efficiency of the solution path in terms of node reuse.
    /// Calculated as: (number of nodes in solution) / (total path length)
    /// 
    /// Examples for a path visiting 5 different nodes:
    /// - If path is [A,B,C,D,E]: Redundancy = 5/5 = 1.0 (perfect efficiency, no revisits)
    /// - If path is [A,B,C,B,D]: Redundancy = 4/5 = 0.8 (good efficiency, one revisit)
    /// - If path is [A,B,A,B,A]: Redundancy = 2/5 = 0.4 (poor efficiency, many revisits)
    /// 
    /// Range: 
    /// - 1.0: Each step in the path visits a new node (no revisits)
    /// - Near 0.0: Many revisits, solution heavily reuses the same nodes
    /// 
    /// Note: This metric alone doesn't indicate if revisits are concentrated in few nodes or evenly distributed.
    /// For example, these two paths have the same Redundancy = 0.4 but very different patterns:
    /// - [A,B,A,B,A]: Revisits distributed between A and B (lower ConcentrationIndex)
    /// - [A,A,A,A,B]: All revisits concentrated in A (higher ConcentrationIndex)
    /// Always check ConcentrationIndex to understand how the revisits are distributed.
    /// </summary>
    public float Redundancy { get; }

    /// <summary>
    /// Tracks how many times each node was traversed in the solution path
    /// </summary>
    private readonly Dictionary<MazeNode, int> _traversalCount = new();
    
    public int GetTraversalCount(MazeNode node) => _traversalCount.GetValueOrDefault(node, 0);

    public MazeSolutionScoring(IReadOnlyCollection<MazeNode> nodes, IEnumerable<MazeNode> keyLocations, MazeNode start) {
        KeyLocations = new List<MazeNode>(keyLocations);

        // The first path starts from the node root and goes to the first key in the zone 0 (best location in 0)
        // because the first key in the zone 0 opens the next zone in the zoneOrder
        var keys = new HashSet<int> { 0 /* The key 0 is always available */ };
        var stops = new List<MazeNode> { start };
        var paths = new List<IReadOnlyList<MazeNode>>();
        var solutionPath = new List<MazeNode>();

        foreach (var zoneId in Enumerable.Range(0, KeyLocations.Count)) {
            keys.Add(zoneId);
            var pathStart = stops.Last();
            var pathEnd = KeyLocations[zoneId];

            var path = pathStart.FindShortestPath(pathEnd, PathWeightMode.Both, node => keys.Contains(node.ZoneId));
            if (path.Count == 0) {
                // This is impossible because keys are visited in order, and the maze is created with the zones in order. But, if this happens,
                // it means the maze is not solvable with the current keys because the zones are not connected (e.g. zone 1 is not connected to zone 0).
                throw new InvalidOperationException($"Cannot reach zone {zoneId} (node id {pathEnd.Id}) from zone {pathStart.ZoneId} (node id {pathStart.Id}) : no valid path exists with current keys: {string.Join(", ", keys)}");
            }
            // Every path starts in the same node than the previous zone ends, so the first needs to be ignored to avoid duplicates
            var skip = zoneId == 0 ? 0 : 1;
            paths.Add(path);
            stops.Add(pathEnd);
            solutionPath.AddRange(path.Skip(skip));
            foreach (var node in path.Skip(skip)) {
                _traversalCount.TryAdd(node, 0);
                _traversalCount[node]++;
            }
        }

        SolutionPath = solutionPath;
        // Calcular non-linearity: suma de todas las visitas adicionales
        // (cada habitación que se visita más de una vez suma sus visitas extras)
        GoalPath = start.FindShortestPath(KeyLocations[^1], PathWeightMode.Both, node => true);

        NodesByVisit = nodes.GroupBy(node => _traversalCount.GetValueOrDefault(node, 0))
            .OrderBy(i => i.Key)
            .ToDictionary(
                grouping => grouping.Key,
                grouping => grouping.ToList()
            );

        VisitDistribution = NodesByVisit
            .ToDictionary(
                kv => kv.Key,
                kv => (float)kv.Value.Count / nodes.Count
            );

        var uniqueNodesInSolution = nodes.Where(node => _traversalCount.GetValueOrDefault(node, 0) > 0).ToList();

        Redundancy = (float)uniqueNodesInSolution.Count / SolutionPath.Count;

        // Índice de concentración usando coeficiente Gini
        // 0.0 = distribución perfectamente uniforme (todos los nodos tienen el mismo número de visitas)
        // 1.0 = concentración máxima (todas las visitas están en un solo nodo)
        var visits = uniqueNodesInSolution.Select(node => _traversalCount[node]).OrderBy(x => x).ToList();
        var n = visits.Count;
        if (n > 0) {
            var absoluteDifferencesSum = visits.SelectMany(x => visits.Select(y => Math.Abs(x - y))).Sum();
            ConcentrationIndex = absoluteDifferencesSum / (2f * n * n * (float)visits.Average());
        } else {
            ConcentrationIndex = 0f; // Si no hay nodos en la solución
        }
    }
}