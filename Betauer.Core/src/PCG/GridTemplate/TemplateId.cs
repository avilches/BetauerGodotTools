using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.PCG.Maze;

namespace Betauer.Core.PCG.GridTemplate;

public class TemplateId {
    public readonly byte Type;
    public readonly HashSet<string> Flags;
    public readonly string? Transform; // Nueva propiedad

    internal TemplateId(byte type, IEnumerable<string> flags, string? transform = null) {
        Type = type;
        Flags = [..flags];
        Transform = transform;
    }

    public static TemplateId Parse(string idString) {
        var parts = idString.Split('/');
        if (parts.Length < 1) {
            throw new ArgumentException("Invalid template ID format. Expected 'directions/flag1/flag2...'");
        }

        var type = ParseDirections(parts[0]);
        var flags = parts.Length > 1 ? parts[1..] : [];
        return new TemplateId(type, flags);
    }

    // Nuevo método para parsear desde un ID con transformación
    public static (TemplateId baseId, string? transform) ParseFromString(string fromString) {
        var parts = fromString.Split(':');
        var baseIdString = parts[0];
        var transform = parts.Length > 1 ? parts[1] : null;

        // Validar transformación
        if (transform != null && !TemplateLoader.IsValidTransform(transform)) {
            throw new ArgumentException($"Invalid transform: {transform}");
        }

        return (Parse(baseIdString), transform);
    }

    public static string TypeToDirectionsString(byte type) {
        var directions = new List<string>();
        if ((type & (byte)DirectionFlags.North) != 0) directions.Add("N");
        if ((type & (byte)DirectionFlags.East) != 0) directions.Add("E");
        if ((type & (byte)DirectionFlags.South) != 0) directions.Add("S");
        if ((type & (byte)DirectionFlags.West) != 0) directions.Add("W");
        return string.Join("", directions);
    }

    private static byte ParseDirections(string directions) {
        byte type = 0;
        directions = directions.Trim().ToUpper();

        // Si es un número, lo parseamos directamente
        if (byte.TryParse(directions, out var numericValue)) {
            return numericValue;
        }

        // Si no, lo interpretamos como combinación de letras
        foreach (var c in directions) {
            type |= c switch {
                'N' => (byte)DirectionFlags.North, // 1
                'E' => (byte)DirectionFlags.East, // 4
                'S' => (byte)DirectionFlags.South, // 16
                'W' => (byte)DirectionFlags.West, // 64
                _ => throw new ArgumentException($"Invalid direction character: {c}")
            };
        }
        return type;
    }

    public static byte FromNode(MazeNode node) {
        byte directions = 0;

        if (node.Up != null) directions |= (byte)DirectionFlags.North;
        if (node.Right != null) directions |= (byte)DirectionFlags.East;
        if (node.Down != null) directions |= (byte)DirectionFlags.South;
        if (node.Left != null) directions |= (byte)DirectionFlags.West;

        return directions;
    }

    public override string ToString() {
        var baseString = TypeToDirectionsString(Type);
        if (Flags.Count == 0) return baseString;
        return baseString + "/" + string.Join("/", Flags);
    }

    public override bool Equals(object? obj) {
        if (obj is not TemplateId other) return false;
        return Type == other.Type && Flags.SetEquals(other.Flags);
    }

    public override int GetHashCode() {
        var hash = Type.GetHashCode();
        foreach (var flag in Flags.OrderBy(f => f)) {
            hash = HashCode.Combine(hash, flag.GetHashCode());
        }
        return hash;
    }
}