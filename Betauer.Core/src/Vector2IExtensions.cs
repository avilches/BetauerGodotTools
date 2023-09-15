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
}