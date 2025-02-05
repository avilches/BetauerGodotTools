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

    public static DirectionFlags Rotate90(DirectionFlags flag) {
        return flag switch {
            DirectionFlags.Up => DirectionFlags.Right,
            DirectionFlags.Right => DirectionFlags.Down,
            DirectionFlags.Down => DirectionFlags.Left,
            DirectionFlags.Left => DirectionFlags.Up,
            DirectionFlags.UpRight => DirectionFlags.DownRight,
            DirectionFlags.DownRight => DirectionFlags.DownLeft,
            DirectionFlags.DownLeft => DirectionFlags.UpLeft,
            DirectionFlags.UpLeft => DirectionFlags.UpRight,
            _ => DirectionFlags.None
        };
    }

    public static DirectionFlags Rotate180(DirectionFlags flag) {
        return Rotate90(Rotate90(flag));
    }

    public static DirectionFlags RotateMinus90(DirectionFlags flag) {
        return Rotate90(Rotate90(Rotate90(flag)));
    }

    public static DirectionFlags FlipH(DirectionFlags flag) {
        return flag switch {
            DirectionFlags.Right => DirectionFlags.Left,
            DirectionFlags.Left => DirectionFlags.Right,
            DirectionFlags.UpRight => DirectionFlags.UpLeft,
            DirectionFlags.UpLeft => DirectionFlags.UpRight,
            DirectionFlags.DownRight => DirectionFlags.DownLeft,
            DirectionFlags.DownLeft => DirectionFlags.DownRight,
            // Up and Down remain the same
            DirectionFlags.Up => DirectionFlags.Up,
            DirectionFlags.Down => DirectionFlags.Down,
            _ => DirectionFlags.None
        };
    }

    public static DirectionFlags FlipV(DirectionFlags flag) {
        return flag switch {
            DirectionFlags.Up => DirectionFlags.Down,
            DirectionFlags.Down => DirectionFlags.Up,
            DirectionFlags.UpRight => DirectionFlags.DownRight,
            DirectionFlags.DownRight => DirectionFlags.UpRight,
            DirectionFlags.UpLeft => DirectionFlags.DownLeft,
            DirectionFlags.DownLeft => DirectionFlags.UpLeft,
            // Left and Right remain the same
            DirectionFlags.Left => DirectionFlags.Left,
            DirectionFlags.Right => DirectionFlags.Right,
            _ => DirectionFlags.None
        };
    }

    public static int Rotate90(int flags) {
        var newType = 0;
        foreach (DirectionFlags flag in Enum.GetValues(typeof(DirectionFlags))) {
            if (flag == DirectionFlags.None) continue;
            if ((flags & (int)flag) != 0) {
                newType |= (int)Rotate90(flag);
            }
        }
        return newType;
    }

    public static int FlipH(int flags) {
        var newType = 0;
        foreach (DirectionFlags flag in Enum.GetValues(typeof(DirectionFlags))) {
            if (flag == DirectionFlags.None) continue;
            if ((flags & (int)flag) != 0) {
                newType |= (int)FlipH(flag);
            }
        }
        return newType;
    }

    public static int FlipV(int flags) {
        var newType = 0;
        foreach (DirectionFlags flag in Enum.GetValues(typeof(DirectionFlags))) {
            if (flag == DirectionFlags.None) continue;
            if ((flags & (int)flag) != 0) {
                newType |= (int)FlipV(flag);
            }
        }
        return newType;
    }

    public static int MirrorLR(int flags) {
        var newType = flags;
        // Primero limpiamos el lado derecho
        newType &= ~((int)(DirectionFlags.Right | DirectionFlags.UpRight | DirectionFlags.DownRight));

        // Copiamos el lado izquierdo al derecho
        if ((flags & (int)DirectionFlags.Left) != 0) newType |= (int)DirectionFlags.Right;
        if ((flags & (int)DirectionFlags.UpLeft) != 0) newType |= (int)DirectionFlags.UpRight;
        if ((flags & (int)DirectionFlags.DownLeft) != 0) newType |= (int)DirectionFlags.DownRight;

        return newType;
    }

    public static int MirrorRL(int flags) {
        var newType = flags;
        // Primero limpiamos el lado izquierdo
        newType &= ~((int)(DirectionFlags.Left | DirectionFlags.UpLeft | DirectionFlags.DownLeft));

        // Copiamos el lado derecho al izquierdo
        if ((flags & (int)DirectionFlags.Right) != 0) newType |= (int)DirectionFlags.Left;
        if ((flags & (int)DirectionFlags.UpRight) != 0) newType |= (int)DirectionFlags.UpLeft;
        if ((flags & (int)DirectionFlags.DownRight) != 0) newType |= (int)DirectionFlags.DownLeft;

        return newType;
    }

    public static int MirrorTB(int flags) {
        var newType = flags;
        // Primero limpiamos el lado inferior
        newType &= ~((int)(DirectionFlags.Down | DirectionFlags.DownLeft | DirectionFlags.DownRight));

        // Copiamos el lado superior al inferior
        if ((flags & (int)DirectionFlags.Up) != 0) newType |= (int)DirectionFlags.Down;
        if ((flags & (int)DirectionFlags.UpLeft) != 0) newType |= (int)DirectionFlags.DownLeft;
        if ((flags & (int)DirectionFlags.UpRight) != 0) newType |= (int)DirectionFlags.DownRight;

        return newType;
    }

    public static int MirrorBT(int flags) {
        var newType = flags;
        // Primero limpiamos el lado superior
        newType &= ~((int)(DirectionFlags.Up | DirectionFlags.UpLeft | DirectionFlags.UpRight));

        // Copiamos el lado inferior al superior
        if ((flags & (int)DirectionFlags.Down) != 0) newType |= (int)DirectionFlags.Up;
        if ((flags & (int)DirectionFlags.DownLeft) != 0) newType |= (int)DirectionFlags.UpLeft;
        if ((flags & (int)DirectionFlags.DownRight) != 0) newType |= (int)DirectionFlags.UpRight;

        return newType;
    }
}