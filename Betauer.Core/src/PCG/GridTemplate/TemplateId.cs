using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.GridTemplate;

public class TemplateId {
    
    // Mapa inverso para la representación canónica de cada DirectionFlags
    private static readonly Dictionary<DirectionFlags, string> CanonicalDirectionMap = new() {
        { DirectionFlags.Up, "U" },
        { DirectionFlags.Right, "R" },
        { DirectionFlags.Down, "D" },
        { DirectionFlags.Left, "L" },
        { DirectionFlags.UpRight, "UR" },
        { DirectionFlags.DownRight, "DR" },
        { DirectionFlags.DownLeft, "DL" },
        { DirectionFlags.UpLeft, "UL" }
    };
    
    // Dictionary para mapear strings a DirectionFlags
    private static readonly Dictionary<string, DirectionFlags> DirectionMap = new() {
        // Direcciones cardinales con todas sus variantes
        { "U", DirectionFlags.Up },    // Up
        { "N", DirectionFlags.Up },    // North
        { "T", DirectionFlags.Up },    // Top
        
        { "R", DirectionFlags.Right }, // Right
        { "E", DirectionFlags.Right }, // East
        
        { "D", DirectionFlags.Down },  // Down
        { "S", DirectionFlags.Down },  // South
        { "B", DirectionFlags.Down },  // Bottom
        
        { "L", DirectionFlags.Left },  // Left
        { "W", DirectionFlags.Left },  // West
        
        // Direcciones diagonales
        { "NE", DirectionFlags.UpRight },    // North-East
        { "SE", DirectionFlags.DownRight },  // South-East
        { "SW", DirectionFlags.DownLeft },   // South-West
        { "NW", DirectionFlags.UpLeft },     // North-West
        
        // Variantes de diagonales usando los alias
        { "TR", DirectionFlags.UpRight },    // Top-Right
        { "BR", DirectionFlags.DownRight },  // Bottom-Right
        { "BL", DirectionFlags.DownLeft },   // Bottom-Left
        { "TL", DirectionFlags.UpLeft },     // Top-Left
        
        { "UR", DirectionFlags.UpRight },    // Up-Right
        { "DR", DirectionFlags.DownRight },  // Down-Right
        { "DL", DirectionFlags.DownLeft },   // Down-Left
        { "UL", DirectionFlags.UpLeft },     // Up-Left
    };
    
    // Dictionary para alias de combinaciones
    private static readonly Dictionary<string, DirectionFlags> AliasMap = [];


    public readonly int Type;
    public readonly HashSet<string> Flags;
    public readonly string? Transform;

    internal TemplateId(int type, IEnumerable<string> flags, string? transform = null) {
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
    public static (string originalId, TemplateId baseId, string? transform) ParseFromString(string fromString) {
        var parts = fromString.Split(':');
        var baseIdString = parts[0];
        var transform = parts.Length > 1 ? parts[1] : null;

        // Validar transformación
        if (transform != null && !TemplateLoader.IsValidTransform(transform)) {
            throw new ArgumentException($"Invalid transform: {transform}");
        }

        return (baseIdString, Parse(baseIdString), transform);
    }

    public static string TypeToDirectionsString(int type) {
        var directions = new List<string>();
        
        // Solo usamos el mapa canónico para la salida
        foreach (var kvp in CanonicalDirectionMap) {
            if ((type & (int)kvp.Key) != 0) {
                directions.Add(kvp.Value);
            }
        }
        
        return string.Join("-", directions.OrderBy(d => d)); // Ordenamos para consistencia
    }


    private static int ParseDirections(string directions) {
        int type = 0;
        directions = directions.Trim().ToUpper();

        // Si es un número, lo parseamos directamente
        if (int.TryParse(directions, out var numericValue)) {
            return numericValue;
        }

        // Si es un alias, lo devolvemos directamente
        if (AliasMap.TryGetValue(directions, out var aliasValue)) {
            return (int)aliasValue;
        }

        // Separamos por guiones y procesamos cada parte
        var parts = directions.Split('-');
        
        foreach (var part in parts) {
            // Primero intentamos como alias
            if (AliasMap.TryGetValue(part, out var partAliasValue)) {
                type |= (int)partAliasValue;
                continue;
            }
            
            // Luego buscamos en el mapa de direcciones
            if (DirectionMap.TryGetValue(part, out var directionValue)) {
                type |= (int)directionValue;
                continue;
            }
            
            throw new ArgumentException($"Invalid direction or alias: {part}");
        }
        
        return type;
    }
    
    // Método público para registrar nuevos alias (útil para testing o extensibilidad)
    public static void RegisterAlias(string alias, DirectionFlags value) {
        AliasMap[alias.ToUpper()] = value;
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