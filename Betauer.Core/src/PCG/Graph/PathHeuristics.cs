namespace Betauer.Core.PCG.Graph;

/// <summary>
/// Common heuristic functions for grid-based pathfinding
/// </summary>
public static class PathHeuristics {
    /// <summary>
    /// Manhattan distance: |x1-x2| + |y1-y2|
    /// Best for grids with only orthogonal movement
    /// </summary>
    public static float Manhattan<TNode, TEdge>(TNode node, TNode target)
        where TNode : class, IGraphNode<TNode, TEdge>
        where TEdge : IGraphEdge<TNode> {
        return node.Position.ManhattanDistanceTo(target.Position);
    }

    /// <summary>
    /// Squared Euclidean distance: (x1-x2)² + (y1-y2)²
    /// Best for grids with any-angle movement
    /// Note: Returns squared distance for efficiency, since we only need
    /// to compare distances and sqrt is not necessary for A*
    /// </summary>
    public static float Euclidean<TNode, TEdge>(TNode node, TNode target)
        where TNode : class, IGraphNode<TNode, TEdge>
        where TEdge : IGraphEdge<TNode> {
        return node.Position.DistanceTo(target.Position);
    }

    /// <summary>
    /// Chebyshev distance: max(|x1-x2|, |y1-y2|)
    /// Best for grids with orthogonal and diagonal movement
    /// </summary>
    public static float Chebyshev<TNode, TEdge>(TNode node, TNode target)
        where TNode : class, IGraphNode<TNode, TEdge>
        where TEdge : IGraphEdge<TNode> {
        return node.Position.ChebyshevDistanceTo(target.Position);
    }
}