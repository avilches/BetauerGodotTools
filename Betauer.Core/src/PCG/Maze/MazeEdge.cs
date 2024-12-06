using System;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents an edge in the maze graph, connecting two nodes
/// </summary>
public class MazeEdge<T>(MazeNode<T> from, MazeNode<T> to, T? metadata = default, float weight = 0f) {
    public MazeNode<T> From { get; } = from ?? throw new ArgumentNullException(nameof(from));
    public MazeNode<T> To { get; } = to ?? throw new ArgumentNullException(nameof(to));
    public T? Metadata { get; set; } = metadata;
    public float Weight { get; set; } = weight;
    
    public Vector2I Direction => To.Position - From.Position;
}