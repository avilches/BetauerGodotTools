using System;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.GridTemplate;

public static class DirectionTransformations {
    public static int TransformDirections(int flags, Transformations.Type transformType) {
        return transformType switch {
            Transformations.Type.Rotate90 => Rotate90(flags),
            Transformations.Type.Rotate180 => Rotate90(Rotate90(flags)),
            Transformations.Type.RotateMinus90 => Rotate90(Rotate90(Rotate90(flags))),
            Transformations.Type.FlipH => FlipH(flags),
            Transformations.Type.FlipV => FlipV(flags),
            Transformations.Type.MirrorLR => MirrorLR(flags),
            Transformations.Type.MirrorRL => MirrorRL(flags),
            Transformations.Type.MirrorTB => MirrorTB(flags),
            Transformations.Type.MirrorBT => MirrorBT(flags),
            _ => throw new ArgumentException($"Invalid transform: {transformType}")
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
            _ => DirectionFlag.None
        };
    }

    public static DirectionFlag Rotate180(DirectionFlag flag) {
        return Rotate90(Rotate90(flag));
    }

    public static DirectionFlag RotateMinus90(DirectionFlag flag) {
        return Rotate90(Rotate90(Rotate90(flag)));
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
            _ => DirectionFlag.None
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
            _ => DirectionFlag.None
        };
    }

    public static int Rotate90(int flags) {
        var newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (int)flag) != 0) {
                newType |= (int)Rotate90(flag);
            }
        }
        return newType;
    }

    public static int FlipH(int flags) {
        var newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (int)flag) != 0) {
                newType |= (int)FlipH(flag);
            }
        }
        return newType;
    }

    public static int FlipV(int flags) {
        var newType = 0;
        foreach (DirectionFlag flag in Enum.GetValues(typeof(DirectionFlag))) {
            if (flag == DirectionFlag.None) continue;
            if ((flags & (int)flag) != 0) {
                newType |= (int)FlipV(flag);
            }
        }
        return newType;
    }

    public static int MirrorLR(int flags) {
        var newType = flags;
        // Primero limpiamos el lado derecho
        newType &= ~((int)(DirectionFlag.Right | DirectionFlag.UpRight | DirectionFlag.DownRight));

        // Copiamos el lado izquierdo al derecho
        if ((flags & (int)DirectionFlag.Left) != 0) newType |= (int)DirectionFlag.Right;
        if ((flags & (int)DirectionFlag.UpLeft) != 0) newType |= (int)DirectionFlag.UpRight;
        if ((flags & (int)DirectionFlag.DownLeft) != 0) newType |= (int)DirectionFlag.DownRight;

        return newType;
    }

    public static int MirrorRL(int flags) {
        var newType = flags;
        // Primero limpiamos el lado izquierdo
        newType &= ~((int)(DirectionFlag.Left | DirectionFlag.UpLeft | DirectionFlag.DownLeft));

        // Copiamos el lado derecho al izquierdo
        if ((flags & (int)DirectionFlag.Right) != 0) newType |= (int)DirectionFlag.Left;
        if ((flags & (int)DirectionFlag.UpRight) != 0) newType |= (int)DirectionFlag.UpLeft;
        if ((flags & (int)DirectionFlag.DownRight) != 0) newType |= (int)DirectionFlag.DownLeft;

        return newType;
    }

    public static int MirrorTB(int flags) {
        var newType = flags;
        // Primero limpiamos el lado inferior
        newType &= ~((int)(DirectionFlag.Down | DirectionFlag.DownLeft | DirectionFlag.DownRight));

        // Copiamos el lado superior al inferior
        if ((flags & (int)DirectionFlag.Up) != 0) newType |= (int)DirectionFlag.Down;
        if ((flags & (int)DirectionFlag.UpLeft) != 0) newType |= (int)DirectionFlag.DownLeft;
        if ((flags & (int)DirectionFlag.UpRight) != 0) newType |= (int)DirectionFlag.DownRight;

        return newType;
    }

    public static int MirrorBT(int flags) {
        var newType = flags;
        // Primero limpiamos el lado superior
        newType &= ~((int)(DirectionFlag.Up | DirectionFlag.UpLeft | DirectionFlag.UpRight));

        // Copiamos el lado inferior al superior
        if ((flags & (int)DirectionFlag.Down) != 0) newType |= (int)DirectionFlag.Up;
        if ((flags & (int)DirectionFlag.DownLeft) != 0) newType |= (int)DirectionFlag.UpLeft;
        if ((flags & (int)DirectionFlag.DownRight) != 0) newType |= (int)DirectionFlag.UpRight;

        return newType;
    }
}