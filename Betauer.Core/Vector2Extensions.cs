using Godot;

namespace Betauer.Core;

public static class Vector2Extensions {
    /// <summary>
    /// Rotate 180º 
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 Rotate180(this Vector2 vec) {
        return new Vector2(-vec.x, -vec.y);
    }

    /// <summary>
    /// Rotate 90º counter-clockwise
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 Rotate90Left(this Vector2 vec) {
        return new Vector2(vec.y, -vec.x); // same as vector2.Orthogonal();
    }

    /// <summary>
    /// Rotate 90º clockwise
    /// (or rotate 270º counter-clockwise)
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 Rotate90Right(this Vector2 vec) {
        return new Vector2(-vec.y, vec.x);
    }

    public const float HalfQuarter = Mathf.Pi / 4; // 0.785398185

    public static bool IsSameDirection(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        normal.Dot(vec) > maxAngle;

    public static bool IsOppositeDirection(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        normal.Dot(vec) < -maxAngle;

    public static bool IsLeft(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        normal.Cross(vec) > maxAngle;

    public static bool IsRight(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        normal.Cross(vec) < -maxAngle;

    public static bool IsFloor(this Vector2 normal, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        Mathf.Acos(normal.Dot(upDirection)) <= maxSlope + 0.01f;

    public static bool IsCeiling(this Vector2 normal, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        Mathf.Acos(normal.Dot(-upDirection)) <= maxSlope + 0.01f;

    public static bool IsWall(this Vector2 normal, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        !normal.IsFloor(upDirection, maxSlope) && !normal.IsCeiling(upDirection, maxSlope);
}