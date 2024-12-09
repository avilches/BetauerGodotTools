using System;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Represents an edge in the maze graph, connecting two nodes
/// </summary>
public class MazeEdge(MazeNode from, MazeNode to, object metadata = default, float weight = 0f) {
    public MazeNode From { get; } = from ?? throw new ArgumentNullException(nameof(from));
    public MazeNode To { get; } = to ?? throw new ArgumentNullException(nameof(to));
    public float Weight { get; set; } = weight;

    public Vector2I Direction => To.Position - From.Position;

    public object Metadata { get; set; }
    public void SetMetadata<T>(T value) => Metadata = value;
    public T GetMetadataOrDefault<T>() => Metadata is T value ? value : default;
    public T GetMetadataOrNew<T>() where T : new() => Metadata is T value ? value : new T();
    public T GetMetadataOr<T>(T defaultValue) => Metadata is T value ? value : defaultValue;
    public T GetMetadataOr<T>(Func<T> factory) => Metadata is T value ? value : factory();
    public bool HasMetadata<T>() => Metadata is T;
    public bool HasAnyMetadata => Metadata != null;
    public void ClearMetadata() => Metadata = null;
}