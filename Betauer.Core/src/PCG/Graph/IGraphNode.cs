using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Graph;

public interface ITreeNode<out TNode> {
    /// <summary>
    /// The parent node in a tree structure (if applicable)
    /// </summary>
    TNode? Parent { get; }
}

/// <summary>
/// Generic interface for a node in a graph
/// </summary>
public interface IGraphNode<TNode, out TEdge>
    where TNode : class, IGraphNode<TNode, TEdge>
    where TEdge : IGraphEdge<TNode> {
    /// <summary>
    /// The position of the node in 2D space
    /// </summary>
    Vector2I Position { get; }

    /// <summary>
    /// The weight of the node, used in pathfinding algorithms
    /// </summary>
    float Weight { get; }

    /// <summary>
    /// Gets all outgoing edges from this node
    /// </summary>
    IEnumerable<TEdge> GetOutEdges();
}

/// <summary>
/// Generic interface for an edge in a graph
/// </summary>
public interface IGraphEdge<out TNode> where TNode : class {
    /// <summary>
    /// The node this edge goes to
    /// </summary>
    TNode To { get; }

    /// <summary>
    /// The weight of the edge, used in pathfinding algorithms
    /// </summary>
    float Weight { get; }
    
}