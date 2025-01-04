using System;
using System.Collections.Generic;
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

    // Metadata object
    public object Metadata { get; set; } = metadata;
    public void SetMetadata<T>(T value) => Metadata = value;
    public T GetMetadataOrDefault<T>() => Metadata is T value ? value : default;
    public T GetMetadataOrNew<T>() where T : new() => Metadata is T value ? value : new T();
    public T GetMetadataOr<T>(T defaultValue) => Metadata is T value ? value : defaultValue;
    public T GetMetadataOr<T>(Func<T> factory) => Metadata is T value ? value : factory();
    public bool HasMetadata<T>() => Metadata is T;
    public bool HasAnyMetadata => Metadata != null;
    public void ClearMetadata() => Metadata = null;

    // Attributes
    public void SetAttribute(string key, object value) => From.Graph.SetAttribute(this, key, value);
    public object? GetAttribute(string key) => From.Graph.GetAttribute(this, key);
    public object GetAttributeOr(string key, object defaultValue) => From.Graph.GetAttributeOr(this, key, defaultValue);
    public T? GetAttributeAs<T>(string key) => From.Graph.GetAttributeAs<T>(this, key);
    public T GetAttributeAsOrDefault<T>(string key, T defaultValue) => From.Graph.GetAttributeAsOrDefault(this, key, defaultValue);
    public T GetAttributeAsOrNew<T>(string key) where T : new() => From.Graph.GetAttributeAsOrNew<T>(this, key);
    public T GetAttributeAsOr<T>(string key, Func<T> factory) => From.Graph.GetAttributeAsOr(this, key, factory);
    public bool RemoveAttribute(string key) => From.Graph.RemoveAttribute(this, key);
    public bool HasAttribute(string key) => From.Graph.HasAttribute(this, key);
    public bool HasAttributeWithValue<T>(string key, T value) => From.Graph.HasAttributeWithValue(this, key, value);
    public bool HasAttributeOfType<T>(string key) => From.Graph.HasAttributeOfType<T>(this, key);
    public IEnumerable<KeyValuePair<string, object>> GetAttributes() => From.Graph.GetAttributes(this);
    public int AttributeCount => From.Graph.GetAttributeCount(this);
    public bool HasAnyAttribute => From.Graph.HasAnyAttribute(this);
    public void ClearAttributes() => From.Graph.ClearAttributes(this);
}