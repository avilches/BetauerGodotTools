using Godot;

namespace Betauer.Core;

public static class Vector2IExtensions {
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
}