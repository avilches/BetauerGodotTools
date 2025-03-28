using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public static class DirectionTransformations {
    public static DirectionFlag TransformDirectionFlag(DirectionFlag flag, Transformations.Type transformType) {
        return transformType switch {
            Transformations.Type.Rotate90 => Rotate90(flag),
            Transformations.Type.Rotate180 => Rotate180(flag),
            Transformations.Type.RotateMinus90 => RotateMinus90(flag),
            Transformations.Type.FlipH => FlipH(flag),
            Transformations.Type.FlipV => FlipV(flag),
            Transformations.Type.FlipDiagonal => FlipDiagonal(flag), // Rotate by primary diagonal (the line from the up,left corner -> down,right corner). That means the "up side" is moved to the "left side".
            Transformations.Type.FlipDiagonalSecondary => FlipDiagonalSecondary(flag),
            Transformations.Type.MirrorLR => MirrorLR(flag),
            Transformations.Type.MirrorRL => MirrorRL(flag),
            Transformations.Type.MirrorTB => MirrorTB(flag),
            Transformations.Type.MirrorBT => MirrorBT(flag),
            _ => throw new ArgumentException($"Invalid flag: {transformType}")
        };
    }

    public static DirectionFlag Rotate90(DirectionFlag flag) {
        return flag switch {
            DirectionFlag.Up => DirectionFlag.Right,
            DirectionFlag.Right => DirectionFlag.Down,
            DirectionFlag.Down => DirectionFlag.Left,
            DirectionFlag.Left => DirectionFlag.Up,
            DirectionFlag.UpRight => DirectionFlag.DownRight,
            DirectionFlag.DownRight => DirectionFlag.DownLeft,
            DirectionFlag.DownLeft => DirectionFlag.UpLeft,
            DirectionFlag.UpLeft => DirectionFlag.UpRight,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag Rotate180(DirectionFlag flag) {
        return flag switch {
            DirectionFlag.Up => DirectionFlag.Down,
            DirectionFlag.Right => DirectionFlag.Left,
            DirectionFlag.Down => DirectionFlag.Up,
            DirectionFlag.Left => DirectionFlag.Right,
            DirectionFlag.UpRight => DirectionFlag.DownLeft,
            DirectionFlag.DownRight => DirectionFlag.UpLeft,
            DirectionFlag.DownLeft => DirectionFlag.UpRight,
            DirectionFlag.UpLeft => DirectionFlag.DownRight,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag RotateMinus90(DirectionFlag flag) {
        return flag switch {
            DirectionFlag.Up => DirectionFlag.Left,
            DirectionFlag.Right => DirectionFlag.Up,
            DirectionFlag.Down => DirectionFlag.Right,
            DirectionFlag.Left => DirectionFlag.Down,
            DirectionFlag.UpRight => DirectionFlag.UpLeft,
            DirectionFlag.DownRight => DirectionFlag.UpRight,
            DirectionFlag.DownLeft => DirectionFlag.DownRight,
            DirectionFlag.UpLeft => DirectionFlag.DownLeft,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag FlipH(DirectionFlag flag) {
        return flag switch {
            DirectionFlag.Right => DirectionFlag.Left,
            DirectionFlag.Left => DirectionFlag.Right,
            DirectionFlag.UpRight => DirectionFlag.UpLeft,
            DirectionFlag.UpLeft => DirectionFlag.UpRight,
            DirectionFlag.DownRight => DirectionFlag.DownLeft,
            DirectionFlag.DownLeft => DirectionFlag.DownRight,
            // Up and Down remain the same
            DirectionFlag.Up => DirectionFlag.Up,
            DirectionFlag.Down => DirectionFlag.Down,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag FlipV(DirectionFlag flag) {
        return flag switch {
            DirectionFlag.Up => DirectionFlag.Down,
            DirectionFlag.Down => DirectionFlag.Up,
            DirectionFlag.UpRight => DirectionFlag.DownRight,
            DirectionFlag.DownRight => DirectionFlag.UpRight,
            DirectionFlag.UpLeft => DirectionFlag.DownLeft,
            DirectionFlag.DownLeft => DirectionFlag.UpLeft,
            // Left and Right remain the same
            DirectionFlag.Left => DirectionFlag.Left,
            DirectionFlag.Right => DirectionFlag.Right,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    /// <summary>
    /// Rotate by primary diagonal (the line from the up,left corner -> down,right corner). That means the "up side" is moved to the "left side".
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static DirectionFlag FlipDiagonal(DirectionFlag flag) {
        return flag switch {
            DirectionFlag.Up => DirectionFlag.Left,
            DirectionFlag.Left => DirectionFlag.Up,
            DirectionFlag.Down => DirectionFlag.Right,
            DirectionFlag.Right => DirectionFlag.Down,
            DirectionFlag.UpLeft => DirectionFlag.UpLeft,
            DirectionFlag.DownRight => DirectionFlag.DownRight,
            DirectionFlag.UpRight => DirectionFlag.DownLeft,
            DirectionFlag.DownLeft => DirectionFlag.UpRight,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag FlipDiagonalSecondary(DirectionFlag flag) {
        return flag switch {
            DirectionFlag.Up => DirectionFlag.Right,
            DirectionFlag.Right => DirectionFlag.Up,
            DirectionFlag.Down => DirectionFlag.Left,
            DirectionFlag.Left => DirectionFlag.Down,
            DirectionFlag.UpRight => DirectionFlag.UpRight,
            DirectionFlag.DownLeft => DirectionFlag.DownLeft,
            DirectionFlag.UpLeft => DirectionFlag.DownRight,
            DirectionFlag.DownRight => DirectionFlag.UpLeft,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag MirrorLR(DirectionFlag flag) {
        return flag switch {
            // Mirror Left to Right
            DirectionFlag.Left => DirectionFlag.Right,
            DirectionFlag.UpLeft => DirectionFlag.UpRight,
            DirectionFlag.DownLeft => DirectionFlag.DownRight,
            // Remove right
            DirectionFlag.Right => DirectionFlag.None,
            DirectionFlag.UpRight => DirectionFlag.None,
            DirectionFlag.DownRight => DirectionFlag.None,
            // El resto permanecen igual
            DirectionFlag.Up => DirectionFlag.Up,
            DirectionFlag.Down => DirectionFlag.Down,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag MirrorRL(DirectionFlag flag) {
        return flag switch {
            // Mirror Right to Left
            DirectionFlag.Right => DirectionFlag.Left,
            DirectionFlag.UpRight => DirectionFlag.UpLeft,
            DirectionFlag.DownRight => DirectionFlag.DownLeft,
            // Remove Left
            DirectionFlag.Left => DirectionFlag.None,
            DirectionFlag.UpLeft => DirectionFlag.None,
            DirectionFlag.DownLeft => DirectionFlag.None,
            // El resto permanecen igual
            DirectionFlag.Up => DirectionFlag.Up,
            DirectionFlag.Down => DirectionFlag.Down,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag MirrorTB(DirectionFlag flag) {
        return flag switch {
            // Mirror Top to Bottom
            DirectionFlag.Up => DirectionFlag.Down,
            DirectionFlag.UpLeft => DirectionFlag.DownLeft,
            DirectionFlag.UpRight => DirectionFlag.DownRight,
            // Remove Bottom
            DirectionFlag.Down => DirectionFlag.None,
            DirectionFlag.DownLeft => DirectionFlag.None,
            DirectionFlag.DownRight => DirectionFlag.None,
            // El resto permanecen igual
            DirectionFlag.Left => DirectionFlag.Left,
            DirectionFlag.Right => DirectionFlag.Right,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static DirectionFlag MirrorBT(DirectionFlag flag) {
        return flag switch {
            // Mirror Bottom to Top
            DirectionFlag.Down => DirectionFlag.Up,
            DirectionFlag.DownLeft => DirectionFlag.UpLeft,
            DirectionFlag.DownRight => DirectionFlag.UpRight,
            // Remove Top
            DirectionFlag.Up => DirectionFlag.None,
            DirectionFlag.UpLeft => DirectionFlag.None,
            DirectionFlag.UpRight => DirectionFlag.None,
            // El resto permanecen igual
            DirectionFlag.Left => DirectionFlag.Left,
            DirectionFlag.Right => DirectionFlag.Right,
            DirectionFlag.None => DirectionFlag.None,
            _ => throw new ArgumentException($"Invalid flag: {flag}")
        };
    }

    public static byte TransformFlags(byte flags, Transformations.Type transformType) {
        return transformType switch {
            Transformations.Type.Rotate90 => Rotate90(flags),
            Transformations.Type.Rotate180 => Rotate180(flags),
            Transformations.Type.RotateMinus90 => RotateMinus90(flags),
            Transformations.Type.FlipH => FlipH(flags),
            Transformations.Type.FlipV => FlipV(flags),
            Transformations.Type.FlipDiagonal => FlipDiagonal(flags), // Rotate by primary diagonal (the line from the up,left corner -> down,right corner). That means the "up side" is moved to the "left side".
            Transformations.Type.FlipDiagonalSecondary => FlipDiagonalSecondary(flags),
            Transformations.Type.MirrorLR => MirrorLR(flags),
            Transformations.Type.MirrorRL => MirrorRL(flags),
            Transformations.Type.MirrorTB => MirrorTB(flags),
            Transformations.Type.MirrorBT => MirrorBT(flags),
            _ => throw new ArgumentException($"Invalid transform: {transformType}")
        };
    }

    public static byte Rotate90(byte flags) {
        byte newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (byte)flag) != 0) {
                newType |= (byte)Rotate90(flag);
            }
        }
        return newType;
    }

    public static byte Rotate180(byte flags) {
        byte newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (byte)flag) != 0) {
                newType |= (byte)Rotate180(flag);
            }
        }
        return newType;
    }

    public static byte RotateMinus90(byte flags) {
        byte newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (byte)flag) != 0) {
                newType |= (byte)RotateMinus90(flag);
            }
        }
        return newType;
    }

    public static byte FlipH(byte flags) {
        byte newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (byte)flag) != 0) {
                newType |= (byte)FlipH(flag);
            }
        }
        return newType;
    }

    public static byte FlipV(byte flags) {
        byte newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (byte)flag) != 0) {
                newType |= (byte)FlipV(flag);
            }
        }
        return newType;
    }


    /// <summary>
    /// Rotate by primary diagonal (the line from the up,left corner -> down,right corner). That means the "up side" is moved to the "left side".
    /// </summary>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static byte FlipDiagonal(byte flags) {
        byte newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (byte)flag) != 0) {
                newType |= (byte)FlipDiagonal(flag);
            }
        }
        return newType;
    }

    public static byte FlipDiagonalSecondary(byte flags) {
        byte newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (byte)flag) != 0) {
                newType |= (byte)FlipDiagonalSecondary(flag);
            }
        }
        return newType;
    }

    public static byte MirrorLR(byte flags) {
        byte newType = flags;
        // Primero limpiamos el lado derecho
        newType &= (byte)~(DirectionFlag.Right | DirectionFlag.UpRight | DirectionFlag.DownRight);

        // Copiamos el lado izquierdo al derecho
        if ((flags & (byte)DirectionFlag.Left) != 0) newType |= (byte)DirectionFlag.Right;
        if ((flags & (byte)DirectionFlag.UpLeft) != 0) newType |= (byte)DirectionFlag.UpRight;
        if ((flags & (byte)DirectionFlag.DownLeft) != 0) newType |= (byte)DirectionFlag.DownRight;

        return newType;
    }

    public static byte MirrorRL(byte flags) {
        byte newType = flags;
        // Primero limpiamos el lado izquierdo
        newType &= ((byte)~(DirectionFlag.Left | DirectionFlag.UpLeft | DirectionFlag.DownLeft));

        // Copiamos el lado derecho al izquierdo
        if ((flags & (byte)DirectionFlag.Right) != 0) newType |= (byte)DirectionFlag.Left;
        if ((flags & (byte)DirectionFlag.UpRight) != 0) newType |= (byte)DirectionFlag.UpLeft;
        if ((flags & (byte)DirectionFlag.DownRight) != 0) newType |= (byte)DirectionFlag.DownLeft;

        return newType;
    }

    public static byte MirrorTB(byte flags) {
        byte newType = flags;
        // Primero limpiamos el lado inferior
        newType &= ((byte)~(DirectionFlag.Down | DirectionFlag.DownLeft | DirectionFlag.DownRight));

        // Copiamos el lado superior al inferior
        if ((flags & (byte)DirectionFlag.Up) != 0) newType |= (byte)DirectionFlag.Down;
        if ((flags & (byte)DirectionFlag.UpLeft) != 0) newType |= (byte)DirectionFlag.DownLeft;
        if ((flags & (byte)DirectionFlag.UpRight) != 0) newType |= (byte)DirectionFlag.DownRight;

        return newType;
    }

    public static byte MirrorBT(byte flags) {
        byte newType = flags;
        // Primero limpiamos el lado superior
        newType &= ((byte)~(DirectionFlag.Up | DirectionFlag.UpLeft | DirectionFlag.UpRight));

        // Copiamos el lado inferior al superior
        if ((flags & (byte)DirectionFlag.Down) != 0) newType |= (byte)DirectionFlag.Up;
        if ((flags & (byte)DirectionFlag.DownLeft) != 0) newType |= (byte)DirectionFlag.UpLeft;
        if ((flags & (byte)DirectionFlag.DownRight) != 0) newType |= (byte)DirectionFlag.UpRight;

        return newType;
    }

    public static Dictionary<string, object> TransformAttributes(IReadOnlyDictionary<string, object> ro, Transformations.Type type) {
        var attributes = new Dictionary<string, object>();
        if (type == Transformations.Type.MirrorLR) {
            MirrorLRAttributes(ro, attributes);
        } else if (type == Transformations.Type.MirrorRL) {
            MirrorRLAttributes(ro, attributes);
        } else if (type == Transformations.Type.MirrorTB) {
            MirrorTBAttributes(ro, attributes);
        } else if (type == Transformations.Type.MirrorBT) {
            MirrorBTAttributes(ro, attributes);
        } else {
            // It's a flip or a rotation, just transform the keys
            foreach (var (k, value) in ro) {
                if (k.StartsWith("dir:")) {
                    var parts = k.Split(':');
                    if (parts.Length >= 2) {
                        var flagStr = parts[1];
                        var flag = DirectionFlagTools.StringToDirectionFlag(flagStr);
                        if (flag == DirectionFlag.None) {
                            attributes[k] = value; // No es una key de dirección correcta, la dejamos igual
                        } else {
                            var transformedFlags = TransformDirectionFlag(flag, type);
                            var newKey = "dir:" + DirectionFlagTools.DirectionFlagToString(transformedFlags);
                            // Add the original key part if it exists
                            if (parts.Length > 2) {
                                newKey += ":" + string.Join(":", parts.Skip(2));
                            }
                            attributes[newKey] = value;
                        }
                    } else {
                        attributes[k] = value; // Invalid format, keep as is
                    }
                } else {
                    attributes[k] = value; // No es una key de dirección, la dejamos igual
                }
            }
        }
        return attributes;
    }

    private static void MirrorLRAttributes(IReadOnlyDictionary<string, object> ro, Dictionary<string, object> attributes) {
        var flagsToIgnore = new[] { DirectionFlag.Right, DirectionFlag.UpRight, DirectionFlag.DownRight };
        var flagsToTransform = new[] { DirectionFlag.Left, DirectionFlag.UpLeft, DirectionFlag.DownLeft };
        MirrorAttributes(ro, attributes, flagsToIgnore, flagsToTransform, MirrorLR);
    }

    private static void MirrorAttributes(
        IReadOnlyDictionary<string, object> ro,
        Dictionary<string, object> attributes,
        DirectionFlag[] flagsToIgnore,
        DirectionFlag[] flagsToTransform,
        Func<DirectionFlag, DirectionFlag> transformFunction) {
        foreach (var (k, value) in ro) {
            if (!k.StartsWith("dir:")) {
                attributes[k] = value;
                continue;
            }

            var parts = k.Split(':');
            if (parts.Length < 2) {
                attributes[k] = value; // Invalid format, keep as is
                continue;
            }

            var flagStr = parts[1];
            var flag = DirectionFlagTools.StringToDirectionFlag(flagStr);

            if (flagsToIgnore.Contains(flag)) {
                // Ignore this flag
                continue;
            }

            // Keep the original flag
            attributes[k] = value;

            // Transform if needed
            if (flagsToTransform.Contains(flag)) {
                var transformedFlag = transformFunction(flag);
                var newKey = "dir:" + DirectionFlagTools.DirectionFlagToString(transformedFlag);
                if (parts.Length > 2) {
                    newKey += ":" + string.Join(":", parts.Skip(2));
                }
                attributes[newKey] = value;
            }
        }
    }

    private static void MirrorRLAttributes(IReadOnlyDictionary<string, object> ro, Dictionary<string, object> attributes) {
        var flagsToIgnore = new[] { DirectionFlag.Left, DirectionFlag.UpLeft, DirectionFlag.DownLeft };
        var flagsToTransform = new[] { DirectionFlag.Right, DirectionFlag.UpRight, DirectionFlag.DownRight };
        MirrorAttributes(ro, attributes, flagsToIgnore, flagsToTransform, MirrorRL);
    }

    private static void MirrorTBAttributes(IReadOnlyDictionary<string, object> ro, Dictionary<string, object> attributes) {
        var flagsToIgnore = new[] { DirectionFlag.Down, DirectionFlag.DownLeft, DirectionFlag.DownRight };
        var flagsToTransform = new[] { DirectionFlag.Up, DirectionFlag.UpLeft, DirectionFlag.UpRight };
        MirrorAttributes(ro, attributes, flagsToIgnore, flagsToTransform, MirrorTB);
    }

    private static void MirrorBTAttributes(IReadOnlyDictionary<string, object> ro, Dictionary<string, object> attributes) {
        var flagsToIgnore = new[] { DirectionFlag.Up, DirectionFlag.UpLeft, DirectionFlag.UpRight };
        var flagsToTransform = new[] { DirectionFlag.Down, DirectionFlag.DownLeft, DirectionFlag.DownRight };
        MirrorAttributes(ro, attributes, flagsToIgnore, flagsToTransform, MirrorBT);
    }
}