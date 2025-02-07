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
    /// Returns a string with the directions in the canonical order from a combination of flags.
    /// TypeToDirectionsString(DirectionFlags.Up) returns "D-U"
    /// </summary>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static string FlagsToString(byte flags) {
        var directions = new List<string>();

        // Solo usamos el mapa canónico para la salida
        foreach (var kvp in CanonicalDirectionMap) {
            if ((flags & (byte)kvp.Key) != 0) {
                directions.Add(kvp.Value);
            }
        }

        return string.Join("-", directions.OrderBy(d => d)); // Ordenamos para consistencia
    }

    /// <summary>
    /// Returns a string with the directions in the canonical order from a single flag
    /// TypeToDirectionsString(DirectionFlags.Up) returns "U"
    /// If the flag is not found, returns an empty string
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static string DirectionFlagToString(DirectionFlag flag) {
        // Solo usamos el mapa canónico para la salida
        foreach (var kvp in CanonicalDirectionMap) {
            if (((byte)flag & (byte)kvp.Key) != 0) {
                return kvp.Value;
            }
        }
        return "";
    }

    /// <summary>
    /// Returns the canonical representation of a key: "t" and "N" returns "U"
    /// If the direction is not valid, it will return the original string
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static string NormalizeFlag(string direction) {
        var flags = StringToDirectionFlag(direction);
        return flags == DirectionFlag.None ? direction : DirectionFlagToString(flags);
    }

    /// <summary>
    /// Returns the canonical representation of a key: "U-R" and "e-n" returns "R-U"
    /// If the string doesn't contain any valid direction, it will return the original string
    /// </summary>
    /// <param name="directions"></param>
    /// <returns></returns>
    public static string NormalizeFlags(string directions) {
        var flags = StringToFlags(directions);
        return flags == 0 ? directions : FlagsToString(flags);
    }


    /// <summary>
    /// Returns the flags from a string: "U-R" and "e-n" returns DirectionFlags.Up | DirectionFlags.Right
    /// It also works with 0-7 numbers, so "1-3" returns DirectionFlags.Up | DirectionFlags.Right
    /// </summary>
    /// <param name="directions"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static byte StringToFlags(string directions, string separator = "-") {
        byte flags = 0;
        directions = directions.Trim().ToUpper();

        // Si es un número, lo parseamos directamente
        if (byte.TryParse(directions, out var numericValue)) {
            return numericValue;
        }

        // Separamos por guiones y procesamos cada parte
        var parts = directions.Split(separator);

        foreach (var part in parts) {
            if (AliasMap.TryGetValue(part, out var partAliasValue)) {
                // Primero intentamos como alias
                flags |= (byte)partAliasValue;
            } else if (DirectionMap.TryGetValue(part, out var directionValue)) {
                // Luego buscamos en el mapa de direcciones
                flags |= (byte)directionValue;
            }
        }

        return flags;
    }

    /// <summary>
    /// Returns the flag from a string: "U" returns DirectionFlags.Up
    /// It also works with 0-7 numbers, so "1" returns DirectionFlags.Up
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static DirectionFlag StringToDirectionFlag(string direction) {
        direction = direction.Trim().ToUpper();

        // Si es un número, lo parseamos directamente
        if (byte.TryParse(direction, out var numericValue)) {
            return (DirectionFlag)numericValue;
        }

        if (AliasMap.TryGetValue(direction, out var partAliasValue)) {
            // Primero intentamos como alias
            return partAliasValue;
        }

        if (DirectionMap.TryGetValue(direction, out var directionValue)) {
            // Luego buscamos en el mapa de direcciones
            return directionValue;
        }
        return DirectionFlag.None;
    }

    public static byte GetDirectionFlags(MazeNode node) {
        byte directions = 0;
        if (node.Up != null) directions |= (byte)DirectionFlag.Up;
        if (node.UpRight != null) directions |= (byte)DirectionFlag.UpRight;
        if (node.Right != null) directions |= (byte)DirectionFlag.Right;
        if (node.DownRight != null) directions |= (byte)DirectionFlag.DownRight;
        if (node.Down != null) directions |= (byte)DirectionFlag.Down;
        if (node.DownLeft != null) directions |= (byte)DirectionFlag.DownLeft;
        if (node.Left != null) directions |= (byte)DirectionFlag.Left;
        if (node.UpLeft != null) directions |= (byte)DirectionFlag.UpLeft;
        return directions;
    }
}