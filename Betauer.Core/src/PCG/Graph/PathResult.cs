using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Graph;

/// <summary>
/// Result of a pathfinding operation
/// </summary>
public class PathResult<TNode, TEdge>(List<TNode> path, float totalCost)
    where TNode : class, IGraphNode<TNode, TEdge>
    where TEdge : IGraphEdge<TNode> {
    
    public List<TNode> Path { get; } = path;
    public float TotalCost { get; } = totalCost;

    /// <summary>
    /// Helper method to calculate only the edges cost
    /// </summary>
    /// <typeparam name="TEdge">The edge type</typeparam>
    /// <returns>The total cost of all edges in the path</returns>
    public float GetEdgesCost() {
        float cost = 0;
        for (var i = 0; i < Path.Count - 1; i++) {
            var edge = Path[i]
                .GetOutEdges()
                .FirstOrDefault(edge => edge.To == Path[i + 1]);
            if (edge != null) {
                cost += edge.Weight;
            }
        }
        return cost;
    }

    /// <summary>
    /// Helper method to calculate only the nodes cost
    /// </summary>
    /// <typeparam name="TEdge">The edge type (required for constraints)</typeparam>
    /// <returns>The total cost of all nodes in the path</returns>
    public float GetNodesCost() {
        return Path.Sum(node => node.Weight);
    }
}