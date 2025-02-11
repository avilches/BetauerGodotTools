using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class Template {
    public byte DirectionFlags { get; set; } = 0;
    public HashSet<string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, object> Attributes { get; set; } = [];
    public Array2D<char> Body { get; set; }
    public string Id { get; set; }

    public Template() {
        Id = RuntimeHelpers.GetHashCode(this).ToString();
    }

    public void AddDirectionFlag(DirectionFlag flag) => DirectionFlags |= (byte)flag;
    public void RemoveDirectionFlag(DirectionFlag flag) => DirectionFlags &= (byte)~flag;
    public bool HasDirectionFlag(DirectionFlag flag) => (DirectionFlags & (byte)flag) != 0;
    public bool HasAllDirectionFlags(DirectionFlag flags) => (DirectionFlags & (byte)flags) == (byte)flags;
    public bool HasAnyDirectionFlag(DirectionFlag flags) => (DirectionFlags & (byte)flags) != 0;

    public void SetAttribute(string key, object value) => Attributes[key] = value;
    public object? GetAttribute(string key) => Attributes.GetValueOrDefault(key);
    public T? GetAttributeAs<T>(string key) => Attributes.TryGetValue(key, out var value) && value is T typedValue ? typedValue : default;
    public T GetAttributeOrDefault<T>(string key, T defaultValue) => Attributes.TryGetValue(key, out var value) && value is T typedValue ? typedValue : defaultValue;
    public T GetAttributeOrCreate<T>(string key, Func<T> factory) {
        if (Attributes.TryGetValue(key, out var value) && value is T typedValue) {
            return typedValue;
        }
        var result = factory();
        Attributes[key] = result;
        return result;
    }
    public bool RemoveAttribute(string key) => Attributes.Remove(key);
    public bool HasAttribute(string key) => Attributes.ContainsKey(key);
    public bool HasAttributeWithValue<T>(string key, T value) => Attributes.TryGetValue(key, out var existingValue) && existingValue is T && Equals(existingValue, value);
    public bool HasAttributeOfType<T>(string key) => Attributes.TryGetValue(key, out var existingValue) && existingValue is T;

    public void SetAttribute(DirectionFlag flag, object value) => SetAttribute(Key(flag), value);
    public object? GetAttribute(DirectionFlag flag) => GetAttribute(Key(flag));
    public T? GetAttributeAs<T>(DirectionFlag flag) => GetAttributeAs<T>(Key(flag));
    public T GetAttributeOrDefault<T>(DirectionFlag flag, T defaultValue) => GetAttributeOrDefault(Key(flag), defaultValue);
    public T GetAttributeOrCreate<T>(DirectionFlag flag, Func<T> factory) => GetAttributeOrCreate(Key(flag), factory);
    public bool RemoveAttribute(DirectionFlag flag) => RemoveAttribute(Key(flag));
    public bool HasAttribute(DirectionFlag flag) => HasAttribute(Key(flag));
    public bool HasAttributeWithValue<T>(DirectionFlag flag, T value) => HasAttributeWithValue(Key(flag), value);
    public bool HasAttributeOfType<T>(DirectionFlag flag) => HasAttributeOfType<T>(Key(flag));

    private static string Key(DirectionFlag flag) => "dir:" + DirectionFlagTools.DirectionFlagToString(flag);

    public void AddTag(string tag) => Tags.Add(tag);
    public bool AddTags(params string[] tags) => tags.All(tag => Tags.Add(tag));
    public bool RemoveTag(string tag) => Tags.Remove(tag);
    public bool RemoveTags(params string[] tags) => tags.All(tag => Tags.Remove(tag));
    public bool HasTag(string tag) => Tags.Contains(tag);
    public bool HasExactTags(params string[] tags) => Tags.SetEquals(new HashSet<string>(tags));
    public bool HasAllTags(params string[] tags) => tags.All(tag => Tags.Contains(tag));
    public bool HasAnyTag(params string[] tags) => tags.Any(tag => Tags.Contains(tag));
    public IEnumerable<string> MatchingTags(string[] tags) => tags.Where(tag => Tags.Contains(tag));

    public override string ToString() {
        return $"Id: {Id} DirectionFlags: {DirectionFlagTools.FlagsToString(DirectionFlags)} Tags:{string.Join(",", Tags.OrderBy(s => s))} {Attributes.Count} Attributes:\n{string.Join("\n", Attributes.Select(pair => pair.Key + "=" + pair.Value).OrderBy(t => t))}";
    }

    public Template Transform(Transformations.Type type) {
        return new Template {
            DirectionFlags = DirectionTransformations.TransformFlags(DirectionFlags, type),
            Tags = [..Tags],
            Attributes = DirectionTransformations.TransformAttributes(Attributes, type),
            Body = new Array2D<char>(Body.Data.Transform(type))
        };
    }

    /// <summary>
    /// Returns null if the transformation does not change the template body.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Template? TryTransform(Transformations.Type type) {
        var body = new Array2D<char>(Body.Data.Transform(type));
        if (BodyIsEquals(body, Body)) return null;
        return new Template {
            DirectionFlags = DirectionTransformations.TransformFlags(DirectionFlags, type),
            Tags = [..Tags],
            Attributes = DirectionTransformations.TransformAttributes(Attributes, type),
            Body = body,
            Id = $"{Id}-{type}"
        };
    }

    public static bool BodyIsEquals(Array2D<char> one, Array2D<char> other) {
        if (ReferenceEquals(one, other)) return true;

        // Check dimensions
        if (one.Width != other.Width || one.Height != other.Height) return false;

        // Compare cell by cell using the provided comparer
        for (var y = 0; y < one.Height; y++) {
            for (var x = 0; x < one.Width; x++) {
                if (one[y, x] != other[y, x]) return false;
            }
        }
        return true;
    }

}