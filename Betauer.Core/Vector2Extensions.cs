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

    public const float HalfQuarter = Mathf.Pi / 4; // 0.78539818 -> 45
    // public const float Quarter = Mathf.Pi / 2;     // 1.57079637 -> 90

    /// <summary>
    /// Returns true if both vector points to the same direction. If vectors are perpendicular, it will return false
    /// </summary>
    /// <param name="normal"></param>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static bool IsSameDirection(this Vector2 normal, Vector2 vec) => normal.Dot(vec) > 0;
    /// <summary>
    /// Returns true if both vector points to the opposite direction. If vectors are perpendicular, it will return false
    /// </summary>
    /// <param name="normal"></param>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static bool IsOppositeDirection(this Vector2 normal, Vector2 vec) => normal.Dot(-vec) > 0;
    public static bool IsPerpendicular(this Vector2 normal, Vector2 vec) => normal.Dot(vec) == 0;

    /// <summary>
    /// Returns true if normal is on the left, so vec is on the right.
    /// If vector are identical (or exactly the opposite), it wil return false (use Dot instead)
    /// </summary>
    /// <param name="normal"></param>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static bool IsLeft(this Vector2 normal, Vector2 vec) => normal.Cross(vec) > 0;
    /// <summary>
    /// Returns true if normal is on the right, so vec is on the left.
    /// If vector are identical (or exactly the opposite), it wil return false (use Dot instead)
    /// </summary>
    /// <param name="normal"></param>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static bool IsRight(this Vector2 normal, Vector2 vec) => normal.Cross(vec) < 0;


    public static bool IsSameDirectionAngle(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        Mathf.Acos(normal.Dot(vec)) < maxAngle;

    public static bool IsOppositeDirectionAngle(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        Mathf.Acos(-normal.Dot(vec)) < maxAngle;

    public static bool IsLeftAngle(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        Mathf.Acos(normal.Cross(vec)) < maxAngle;

    public static bool IsRightAngle(this Vector2 normal, Vector2 vec, float maxAngle = HalfQuarter) =>
        Mathf.Acos(-normal.Cross(vec)) < maxAngle;

    
    public static bool IsFloor(this Vector2 normal, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        Mathf.Acos(normal.Dot(upDirection)) <= maxSlope + 0.01f;

    public static bool IsCeiling(this Vector2 normal, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        Mathf.Acos(normal.Dot(-upDirection)) <= maxSlope + 0.01f;

    public static bool IsWall(this Vector2 normal, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        !normal.IsFloor(upDirection, maxSlope) && !normal.IsCeiling(upDirection, maxSlope);
}