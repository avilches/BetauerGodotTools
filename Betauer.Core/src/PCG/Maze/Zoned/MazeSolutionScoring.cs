using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeSolutionScoring {
    /// <summary>
    /// Location of keys in the maze, indexed by zone ID.
    /// For example, if KeyLocations[2] = NodeX, it means the key for zone 2 is located in NodeX.
    /// </summary>
    public Dictionary<int, MazeNode> KeyLocations { get; }

    /// <summary>
    /// The complete path taken through the maze, including all nodes visited and revisited in order.
    /// For example: if a path goes A->B->C->B->D, SolutionPath will contain [A,B,C,B,D]
    /// </summary>
    public IReadOnlyList<MazeNode> SolutionPath { get; }

    /// <summary>
    /// The order in which zones must be visited to collect keys.
    /// Example: [1,3,2] means:
    /// - First get key for zone 1 (in zone 0)
    /// - Then get key for zone 3 (in zone 1)
    /// - Finally get key for zone 2 (in zone 3)
    /// </summary>
    public IReadOnlyList<int> ZoneOrder { get; }

    /// <summary>
    /// The shortest possible path to the final destination, ignoring zone restrictions.
    /// Useful for comparing with SolutionPath to evaluate path efficiency.
    /// Example: if SolutionPath is [A,B,C,B,D] and GoalPath is [A,B,D], 
    /// it shows that revisiting B and visiting C were necessary due to zone constraints.
    /// </summary>
    public IReadOnlyList<MazeNode> GoalPath { get; }

    /// <summary>
    /// Groups nodes by how many times they were visited during solution traversal.
    /// Key: number of visits, Value: list of nodes with that visit count.
    /// Example:
    /// - NodesByRevisit[0] = [Node1, Node2] // Nodes never visited
    /// - NodesByRevisit[1] = [Node3, Node4] // Nodes visited once
    /// - NodesByRevisit[2] = [Node5] // Node visited twice
    ///
    /// The [0] key is used for nodes that were never visited, so they are not part of the solution path.
    /// </summary>
    public IReadOnlyDictionary<int, List<NodeScore>> NodesByVisit { get; }

    /// <summary>
    /// Percentage of nodes that have each visit count, relative to total maze nodes.
    /// Key: number of visits, Value: percentage of total nodes with that visit count.
    /// Example for a maze with 10 nodes:
    /// - VisitDistribution[0] = 0.4 // 40% of nodes were never visited
    /// - VisitDistribution[1] = 0.5 // 50% of nodes were visited once
    /// - VisitDistribution[2] = 0.1 // 10% of nodes were visited twice
    /// </summary>
    public IReadOnlyDictionary<int, float> VisitDistribution { get; }

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
    
    public MazeSolutionScoring(Dictionary<MazeNode, NodeScore> scores, Dictionary<int, MazeNode> keyLocations, List<int> zoneOrder, IReadOnlyList<MazeNode> goalPath, List<MazeNode> solutionPath) {
        KeyLocations = keyLocations;
        SolutionPath = solutionPath;
        ZoneOrder = zoneOrder;
        GoalPath = goalPath;

        NodesByVisit = scores.Values.GroupBy(i => i.SolutionTraversals).OrderBy(i => i.Key).ToDictionary(
            grouping => grouping.Key, // número de visitas
            grouping => grouping.ToList()
        );
        VisitDistribution = NodesByVisit
            .ToDictionary(
                kv => kv.Key, // número de visitas
                kv => (float)kv.Value.Count / scores.Count // porcentaje de nodos
            );

        var uniqueNodesInSolution = scores.Values.Where(score => score.SolutionTraversals > 0).ToList();
            
        Redundancy = (float)uniqueNodesInSolution.Count / SolutionPath.Count;

        // Índice de concentración usando coeficiente Gini
        // 0.0 = distribución perfectamente uniforme (todos los nodos tienen el mismo número de visitas)
        // 1.0 = concentración máxima (todas las visitas están en un solo nodo)
        var visits = uniqueNodesInSolution.Select(s => s.SolutionTraversals).OrderBy(x => x).ToList();
        var n = visits.Count;
        if (n > 0) {
            var absoluteDifferencesSum = visits.SelectMany(x => visits.Select(y => Math.Abs(x - y))).Sum();
            ConcentrationIndex = absoluteDifferencesSum / (2f * n * n * (float)visits.Average());
        } else {
            ConcentrationIndex = 0f; // Si no hay nodos en la solución
        }
    }


}