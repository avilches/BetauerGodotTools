using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public class Template {
    public byte DirectionFlags { get; set; } = 0;
    public HashSet<string> Tags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, object> Attributes { get; set; } = [];
    public Array2D<char> Body { get; set; }

    public void AddDirectionFlag(DirectionFlag flag) => DirectionFlags |= (byte)flag;
    public void RemoveDirectionFlag(DirectionFlag flag) => DirectionFlags &= (byte)~flag;
    public bool HasDirectionFlag(DirectionFlag flag) => (DirectionFlags & (byte)flag) != 0;
    public bool HasAllDirectionFlags(DirectionFlag flags) => (DirectionFlags & (byte)flags) == (byte)flags;
    public bool HasAnyDirectionFlag(DirectionFlag flags) => (DirectionFlags & (byte)flags) != 0;

    public void SetAttribute(string key, object value) => Attributes[key] = value;
    public object? GetAttribute(string key) => Attributes.GetValueOrDefault(key);
    public object GetAttributeOr(string key, object defaultValue) => Attributes.GetValueOrDefault(key, defaultValue);
    public T? GetAttributeAs<T>(string key) => Attributes.TryGetValue(key, out var value) && value is T typedValue ? typedValue : default;
    public T GetAttributeAsOrDefault<T>(string key, T defaultValue) => Attributes.TryGetValue(key, out var value) && value is T typedValue ? typedValue : defaultValue;
    public T GetAttributeAsOrNew<T>(string key) where T : new() => Attributes.TryGetValue(key, out var value) && value is T typedValue ? typedValue : new T();
    public T GetAttributeAsOr<T>(string key, Func<T> factory) => Attributes.TryGetValue(key, out var value) && value is T typedValue ? typedValue : factory();
    public bool RemoveAttribute(string key) => Attributes.Remove(key);
    public bool HasAttribute(string key) => Attributes.ContainsKey(key);
    public bool HasAttributeWithValue<T>(string key, T value) => Attributes.TryGetValue(key, out var existingValue) && existingValue is T && Equals(existingValue, value);
    public bool HasAttributeOfType<T>(string key) => Attributes.TryGetValue(key, out var existingValue) && existingValue is T;

    public void SetAttribute(DirectionFlag flag, object value) => Attributes["dir:"+DirectionFlagTools.DirectionFlagToString(flag)] = value;
    public object? GetAttribute(DirectionFlag flag) => Attributes.GetValueOrDefault("dir:"+DirectionFlagTools.DirectionFlagToString(flag));
    public object GetAttributeOr(DirectionFlag flag, object defaultValue) => Attributes.GetValueOrDefault("dir:"+DirectionFlagTools.DirectionFlagToString(flag), defaultValue);
    public T? GetAttributeAs<T>(DirectionFlag flag) => Attributes.TryGetValue("dir:"+DirectionFlagTools.DirectionFlagToString(flag), out var value) && value is T typedValue ? typedValue : default;
    public T GetAttributeAsOrDefault<T>(DirectionFlag flag, T defaultValue) => Attributes.TryGetValue("dir:"+DirectionFlagTools.DirectionFlagToString(flag), out var value) && value is T typedValue ? typedValue : defaultValue;
    public T GetAttributeAsOrNew<T>(DirectionFlag flag) where T : new() => Attributes.TryGetValue("dir:"+DirectionFlagTools.DirectionFlagToString(flag), out var value) && value is T typedValue ? typedValue : new T();
    public T GetAttributeAsOr<T>(DirectionFlag flag, Func<T> factory) => Attributes.TryGetValue("dir:"+DirectionFlagTools.DirectionFlagToString(flag), out var value) && value is T typedValue ? typedValue : factory();
    public bool RemoveAttribute(DirectionFlag flag) => Attributes.Remove("dir:"+DirectionFlagTools.DirectionFlagToString(flag));
    public bool HasAttribute(DirectionFlag flag) => Attributes.ContainsKey("dir:"+DirectionFlagTools.DirectionFlagToString(flag));
    public bool HasAttributeWithValue<T>(DirectionFlag flag, T value) => Attributes.TryGetValue("dir:"+DirectionFlagTools.DirectionFlagToString(flag), out var existingValue) && existingValue is T && Equals(existingValue, value);
    public bool HasAttributeOfType<T>(DirectionFlag flag) => Attributes.TryGetValue("dir:"+DirectionFlagTools.DirectionFlagToString(flag), out var existingValue) && existingValue is T;

    public void AddTag(string tag) => Tags.Add(tag);
    public bool AddTags(params string[] tags) => tags.All(tag => Tags.Add(tag));
    public bool RemoveTag(string tag) => Tags.Remove(tag);
    public bool RemoveTags(params string[] tags) => tags.All(tag => Tags.Remove(tag));
    public bool HasTag(string tag) => Tags.Contains(tag);
    public bool HasExactTags(params string[] tags) => Tags.SetEquals(new HashSet<string>(tags));
    public bool HasAllTags(params string[] tags) => tags.All(tag => Tags.Contains(tag));
    public bool HasAnyTag(params string[] tags) => tags.Any(tag => Tags.Contains(tag));    public IEnumerable<string> MatchingTags(string[] tags) => tags.Where(tag => Tags.Contains(tag));


    public override string ToString() {
        var baseString = DirectionFlagTools.FlagsToString(DirectionFlags);
        if (Tags.Count == 0) return baseString;
        return baseString + "/" + string.Join("/", Tags) + " " + string.Join(" ", Attributes.Select(pair => pair.Key + "=" + pair.Value));
    }

    public Template Transform(Transformations.Type type) {
        var attributes = DirectionTransformations.TransformAttributes(Attributes, type);
        return new Template {
            DirectionFlags = DirectionTransformations.TransformFlags(DirectionFlags, type),
            Tags = [..Tags],
            Attributes = attributes,
            Body = new Array2D<char>(Body.Data.Transform(type))
        };
    }
}