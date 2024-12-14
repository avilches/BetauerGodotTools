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
    private Dictionary<string, object>? _attributes;
    public void SetAttribute(string key, object value) => (_attributes ??= new Dictionary<string, object>())[key] = value;
    public object? GetAttribute(string key) => _attributes?.TryGetValue(key, out var value) == true ? value : default;
    public object GetAttributeOr(string key, object defaultValue) => _attributes?.TryGetValue(key, out var value) == true ? value : defaultValue;
    public T? GetAttributeAs<T>(string key) => _attributes?.TryGetValue(key, out var value) == true && value is T typedValue ? typedValue : default;
    public T GetAttributeAsOrDefault<T>(string key, T defaultValue) => _attributes?.TryGetValue(key, out var value) == true && value is T typedValue ? typedValue : defaultValue;
    public T GetAttributeAsOrNew<T>(string key) where T : new() => _attributes?.TryGetValue(key, out var value) == true && value is T typedValue ? typedValue : new T();
    public T GetAttributeAsOr<T>(string key, Func<T> factory) => _attributes?.TryGetValue(key, out var value) == true && value is T typedValue ? typedValue : factory();
    public bool RemoveAttribute(string key) {
        if (_attributes == null) return false;
        var deleted = _attributes.Remove(key);
        if (_attributes.Count == 0) _attributes = null;
        return deleted;
    }
    public bool HasAttribute(string key) => _attributes?.ContainsKey(key) == true;
    public bool HasAttributeWithValue(string key, object value) => _attributes?.TryGetValue(key, out var existingValue) == true && Equals(existingValue, value);
    public bool HasAttributeOfType<T>(string key) => _attributes?.TryGetValue(key, out var value) == true && value is T;
    public IReadOnlyDictionary<string, object>? GetAttributes() => _attributes;
    public void ClearAttributes() {
        _attributes?.Clear();
        _attributes = null;
    }
    public int AttributeCount => _attributes?.Count ?? 0;
    public bool HasAnyAttribute => _attributes != null && _attributes.Count > 0;
}