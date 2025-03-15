using System;
using Betauer.Core.PCG.Graph;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents an edge in the maze graph, connecting two nodes
/// </summary>
public class MazeEdge(MazeNode from, MazeNode to, float weight = 0f) : IGraphEdge<MazeNode> {
    public MazeNode From { get; } = from ?? throw new ArgumentNullException(nameof(from));
    public MazeNode To { get; } = to ?? throw new ArgumentNullException(nameof(to));
    public float Weight { get; set; } = weight;
    public Vector2I Direction => To.Position - From.Position;
}