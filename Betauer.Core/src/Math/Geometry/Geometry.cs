using Godot;

namespace Betauer.Core.Math.Geometry;

public static partial class Geometry {

    public static float Distance(float x1, float y1, float x2, float y2) {
        return Mathf.Sqrt(DistanceSquared(x1, y1, x2, y2));
    }

    // Overloaded version using Vector2
    public static float Distance(Vector2 point1, Vector2 point2) {
        return Distance(point1.X, point1.Y, point2.X, point2.Y);
    }

    public static float DistanceSquared(float x1, float y1, float x2, float y2) {
        var dx = x1 - x2;
        var dy = y1 - y2;
        return dx * dx + dy * dy;
    }

    // Overloaded version using Vector2
    public static float DistanceSquared(Vector2 point1, Vector2 point2) {
        return DistanceSquared(point1.X, point1.Y, point2.X, point2.Y);
    }

    /// <summary>
    /// Returns the new start of a segment that starts at x0, y0 and ends at x1, y1 and is extended by extension
    /// </summary>
    public static Vector2 ExtendLineAtStart(float x0, float y0, float x1, float y1, float extension) {
        var length = Distance(x0, y0, x1, y1);
        var dx = (x1 - x0) / length;
        var dy = (y1 - y0) / length;

        return new Vector2(x0 - dx * extension, y0 - dy * extension);
    }

    // Overloaded version using Vector2
    public static Vector2 ExtendLineAtStart(Vector2 start, Vector2 end, float extension) {
        return ExtendLineAtStart(start.X, start.Y, end.X, end.Y, extension);
    }

    /// <summary>
    /// Returns the new end of a segment that starts at x0, y0 and ends at x1, y1 and is extended by extension
    /// </summary>
    public static Vector2 ExtendLineAtEnd(float x0, float y0, float x1, float y1, float extension) {
        var length = Distance(x0, y0, x1, y1);
        var dx = (x1 - x0) / length;
        var dy = (y1 - y0) / length;

        return new Vector2(x1 + dx * extension, y1 + dy * extension);
    }

    // Overloaded version using Vector2
    public static Vector2 ExtendLineAtEnd(Vector2 start, Vector2 end, float extension) {
        return ExtendLineAtEnd(start.X, start.Y, end.X, end.Y, extension);
    }

}