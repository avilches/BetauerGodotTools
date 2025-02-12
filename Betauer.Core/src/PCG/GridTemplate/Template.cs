using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTools;
using Godot;

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
        return $"Id: {AttributeParser.ParseResult.Print(Id)} DirectionFlags: \"{DirectionFlagTools.FlagsToString(DirectionFlags)}\" ({DirectionFlags}) Tags: {string.Join(",", Tags.OrderBy(s => s))} {Attributes.Count} Attributes: {string.Join(" ", Attributes.Select(pair => pair.Key + "=" + AttributeParser.ParseResult.Print(pair.Value)).OrderBy(t => t))}".Trim();
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

    private bool ValidateBorders(Func<Vector2I, bool> isBlocked) {
        var center = Body.Width / 2;

        bool ValidateSide(DirectionFlag flag, Vector2I startPos, bool horizontal) {
            if (!HasDirectionFlag(flag)) {
                // Si no hay salida, todo el lado debe estar bloqueado
                var length = horizontal ? Body.Width : Body.Height;
                for (var i = 0; i < length; i++) {
                    var pos = horizontal ? new Vector2I(i, startPos.Y) : new Vector2I(startPos.X, i);
                    if (!isBlocked(pos)) return false;
                }
            } else {
                // Si hay salida, validar que el tamaño es correcto (1, 3 o 5)
                var width = GetAttributeOrDefault(flag, 1);
                if (width != 1 && width != 3 && width != 5) return false;

                var halfWidth = width / 2;
                var exitStart = center - halfWidth;
                var exitEnd = center + halfWidth;
                var length = horizontal ? Body.Width : Body.Height;

                // Validar todo el lado
                for (var i = 0; i < length; i++) {
                    var pos = horizontal ? new Vector2I(i, startPos.Y) : new Vector2I(startPos.X, i);
                    var isExit = i >= exitStart && i <= exitEnd;

                    // Las celdas de salida deben ser transitables, el resto bloqueadas
                    if (isExit == isBlocked(pos)) return false;
                }
            }
            return true;
        }

        // Validar los cuatro lados
        if (!ValidateSide(DirectionFlag.Up, new Vector2I(0, 0), true)) return false;
        if (!ValidateSide(DirectionFlag.Down, new Vector2I(0, Body.Height - 1), true)) return false;
        if (!ValidateSide(DirectionFlag.Left, new Vector2I(0, 0), false)) return false;
        if (!ValidateSide(DirectionFlag.Right, new Vector2I(Body.Width - 1, 0), false)) return false;

        return true;
    }

    public bool IsValid(Func<Vector2I, bool> isBlocked) {
        // Primero validar que los bordes están correctamente formados
        if (!ValidateBorders(isBlocked)) return false;

        // Count direction flags (exits)
        var count = BitOperations.PopCount(DirectionFlags);
        if (count <= 1) return true; // 0 or 1 exit is always valid

        // Create graph where only floor tiles are walkable
        var graph = new Array2DGraph<char>(Body, null, isBlocked);

        // Get all exit positions (central position for each border)
        var center = Body.Width / 2; // Template is square and odd size

        // Dictionary to store all possible exits and their flags
        var exitPositions = new Dictionary<Vector2I, DirectionFlag> {
            { new Vector2I(center, 0), DirectionFlag.Up },
            { new Vector2I(Body.Width - 1, center), DirectionFlag.Right },
            { new Vector2I(center, Body.Height - 1), DirectionFlag.Down },
            { new Vector2I(0, center), DirectionFlag.Left }
        };

        // Get only the marked exits
        var markedExits = exitPositions
            .Where(kvp => HasDirectionFlag(kvp.Value))
            .Select(kvp => kvp.Key)
            .ToList();

        // Get all reachable positions from the first exit
        var reachableZone = graph.GetReachableZone(markedExits[0]);

        // First check: verify all marked exits are connected
        for (var i = 1; i < markedExits.Count; i++) {
            if (!reachableZone.Contains(markedExits[i])) return false; // Can't reach this marked exit
        }

        // Second check: verify there are no paths to unmarked exits
        var unmarkedExits = exitPositions
            .Where(kvp => !HasDirectionFlag(kvp.Value))
            .Select(kvp => kvp.Key);

        foreach (var unmarkedExit in unmarkedExits) {
            if (reachableZone.Contains(unmarkedExit)) return false; // Found a path to an unmarked exit!
        }

        return true;
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