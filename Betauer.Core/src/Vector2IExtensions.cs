using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core;

public static class Vector2IExtensions {
    public static Vector2I Inverse(this Vector2I from) {
        return from * -1;
    }

    /// <summary>
    /// Rotate 180º
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2I Rotate180(this Vector2I vec) {
        return new Vector2I(-vec.X, -vec.Y);
    }

    /// <summary>
    /// Rotate 90º counter-clockwise
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2I Rotate90Left(this Vector2I vec) {
        return new Vector2I(vec.Y, -vec.X); // same as Vector2I.Orthogonal();
    }

    /// <summary>
    /// Rotate 90º clockwise
    /// (or rotate 270º counter-clockwise)
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2I Rotate90Right(this Vector2I vector) {
        return new Vector2I(-vector.Y, vector.X);
    }

    public static Vector2I UpLeftPos(this Vector2I from) {
        return new Vector2I(from.X - 1, from.Y - 1);
    }

    public static Vector2I UpRightPos(this Vector2I from) {
        return new Vector2I(from.X + 1, from.Y - 1);
    }

    public static Vector2I DownLeftPos(this Vector2I from) {
        return new Vector2I(from.X - 1, from.Y + 1);
    }

    public static Vector2I DownRightPos(this Vector2I from) {
        return new Vector2I(from.X + 1, from.Y + 1);
    }

    public static Vector2I LeftPos(this Vector2I from) {
        return new Vector2I(from.X - 1, from.Y);
    }

    public static Vector2I RightLeftPos(this Vector2I from) {
        return new Vector2I(from.X + 1, from.Y);
    }

    public static Vector2I UpPos(this Vector2I from) {
        return new Vector2I(from.X, from.Y - 1);
    }

    public static Vector2I DownPos(this Vector2I from) {
        return new Vector2I(from.X, from.Y + 1);
    }

    public static Vector2I Clockwise(this Vector2I dir) {
        return dir == Vector2I.Down ? Vector2I.Left :
            dir == Vector2I.Left ? Vector2I.Up :
            dir == Vector2I.Up ? Vector2I.Right : Vector2I.Down;
    }

    public static Vector2I CounterClockwise(this Vector2I dir) {
        return dir == Vector2I.Down ? Vector2I.Right :
            dir == Vector2I.Right ? Vector2I.Up :
            dir == Vector2I.Up ? Vector2I.Left : Vector2I.Down;
    }
    
    public static float DistanceTo(this Vector2I from, Vector2I to) {
        return Geometry.Distance(from, to);
    }
}