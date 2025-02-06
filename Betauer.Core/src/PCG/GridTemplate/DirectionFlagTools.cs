using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.PCG.Maze;

namespace Betauer.Core.PCG.GridTemplate;

public static class DirectionFlagTools {

    // Mapa inverso para la representación canónica de cada DirectionFlags
    private static readonly Dictionary<DirectionFlag, string> CanonicalDirectionMap = new() {
        { DirectionFlag.Up, "U" },
        { DirectionFlag.Right, "R" },
        { DirectionFlag.Down, "D" },
        { DirectionFlag.Left, "L" },
        { DirectionFlag.UpRight, "UR" },
        { DirectionFlag.DownRight, "DR" },
        { DirectionFlag.DownLeft, "DL" },
        { DirectionFlag.UpLeft, "UL" }
    };

    // Dictionary para mapear strings a DirectionFlags
    private static readonly Dictionary<string, DirectionFlag> DirectionMap = new() {
        // Direcciones cardinales con todas sus variantes
        { "U", DirectionFlag.Up },    // Up
        { "N", DirectionFlag.Up },    // North
        { "T", DirectionFlag.Up },    // Top

        { "R", DirectionFlag.Right }, // Right
        { "E", DirectionFlag.Right }, // East

        { "D", DirectionFlag.Down },  // Down
        { "S", DirectionFlag.Down },  // South
        { "B", DirectionFlag.Down },  // Bottom

        { "L", DirectionFlag.Left },  // Left
        { "W", DirectionFlag.Left },  // West

        // Direcciones diagonales
        { "NE", DirectionFlag.UpRight },    // North-East
        { "SE", DirectionFlag.DownRight },  // South-East
        { "SW", DirectionFlag.DownLeft },   // South-West
        { "NW", DirectionFlag.UpLeft },     // North-West

        // Variantes de diagonales usando los alias
        { "TR", DirectionFlag.UpRight },    // Top-Right
        { "BR", DirectionFlag.DownRight },  // Bottom-Right
        { "BL", DirectionFlag.DownLeft },   // Bottom-Left
        { "TL", DirectionFlag.UpLeft },     // Top-Left

        { "UR", DirectionFlag.UpRight },    // Up-Right
        { "DR", DirectionFlag.DownRight },  // Down-Right
        { "DL", DirectionFlag.DownLeft },   // Down-Left
        { "UL", DirectionFlag.UpLeft },     // Up-Left
    };

    // Dictionary para alias de combinaciones
    private static readonly Dictionary<string, DirectionFlag> AliasMap = [];
    public static void RegisterAlias(string alias, DirectionFlag value) {
        AliasMap[alias.ToUpper()] = value;
    }

    /// <summary>
    /// Returns a string with the directions in the canonical order.
    /// TypeToDirectionsString(DirectionFlags.Up | DirectionFlags.Down) returns "D-U"
    /// </summary>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static string FlagsToString(int flags) {
        var directions = new List<string>();

        // Solo usamos el mapa canónico para la salida
        foreach (var kvp in CanonicalDirectionMap) {
            if ((flags & (int)kvp.Key) != 0) {
                directions.Add(kvp.Value);
            }
        }

        return string.Join("-", directions.OrderBy(d => d)); // Ordenamos para consistencia
    }

    /// <summary>
    /// Returns the flags from a string: "U-R" returns DirectionFlags.Up | DirectionFlags.Right
    /// </summary>
    /// <param name="directions"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int StringToFlags(string directions, string separator = "-") {
        var flags = 0;
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
        var parts = directions.Split(separator);

        foreach (var part in parts) {
            // Primero intentamos como alias
            if (AliasMap.TryGetValue(part, out var partAliasValue)) {
                flags |= (int)partAliasValue;
                continue;
            }

            // Luego buscamos en el mapa de direcciones
            if (DirectionMap.TryGetValue(part, out var directionValue)) {
                flags |= (int)directionValue;
                continue;
            }

            throw new ArgumentException($"Invalid direction or alias: {part}");
        }

        return flags;
    }

    public static int GetDirectionFlags(MazeNode node) {
        var directions = 0;
        if (node.Up != null) directions |= (int)DirectionFlag.Up;
        if (node.UpRight != null) directions |= (int)DirectionFlag.UpRight;
        if (node.Right != null) directions |= (int)DirectionFlag.Right;
        if (node.DownRight != null) directions |= (int)DirectionFlag.DownRight;
        if (node.Down != null) directions |= (int)DirectionFlag.Down;
        if (node.DownLeft != null) directions |= (int)DirectionFlag.DownLeft;
        if (node.Left != null) directions |= (int)DirectionFlag.Left;
        if (node.UpLeft != null) directions |= (int)DirectionFlag.UpLeft;
        return directions;
    }


}