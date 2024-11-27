using Godot;

namespace Betauer.Core;

public static class Vector2Extensions {
    public static Vector2 Inverse(this Vector2 from) {
        return from * -1;
    }
    /// <summary>
    /// Rotate 180º 
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 Rotate180(this Vector2 vec) {
        return new Vector2(-vec.X, -vec.Y);
    }

    /// <summary>
    /// Rotate 90º counter-clockwise
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    public static Vector2 Rotate90Left(this Vector2 vec) {
        return new Vector2(vec.Y, -vec.X); // same as vector2.Orthogonal();
    }

    /// <summary>
    /// Rotate 90º clockwise
    /// (or rotate 270º counter-clockwise)
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector2 Rotate90Right(this Vector2 vector) {
        return new Vector2(-vector.Y, vector.X);
    }

    public const float Quarter = Mathf.Pi / 2;     // 1.57079637 -> 90 degrees
    public const float HalfQuarter = Mathf.Pi / 4; // 0.78539818 -> 45 degrees

    /// <summary>
    /// Returns true if both vectors points to the same direction. The means that the angle between them is less than 90º (a 180º overlap).
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsSameDirection(this Vector2 vector, Vector2 other) => 
        vector.Dot(other) > 0;

    /// <summary>
    /// Returns true if both vectors points to the same direction with a max angle. By default, the max angle is half quarter (45º).
    /// That means the overlap of both vectors could be up to 90º (45º on the left + 45º on the right).
    ///
    /// This method return the same as IsSameDirection() if the max angle is 90º (that will means 180º overlap)
    ///                          
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <param name="maxAngle"></param>
    /// <returns></returns>
    public static bool IsSameDirectionAngle(this Vector2 vector, Vector2 other, float maxAngle = HalfQuarter) =>
        Mathf.Acos(vector.Dot(other)) < maxAngle;


    /// <summary>
    /// Returns true if both vectors points int opposite directions.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsOppositeDirection(this Vector2 vector, Vector2 other) => 
        vector.Dot(-other) > 0;
    
    /// <summary>
    /// Returns true if both vectors are perpendicular (90º).
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsPerpendicular(this Vector2 vector, Vector2 other) => 
        vector.Dot(other) == 0;

    /// <summary>
    /// returns true if the other vector is to its left.
    /// If other vector is identical (or exactly the opposite), it wil return false too.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsLeft(this Vector2 vector, Vector2 other) => 
        vector.Cross(other) > 0;
    
    /// <summary>
    /// returns true if the other vector is to its right.
    /// If other vector is identical (or exactly the opposite), it wil return false too.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsRight(this Vector2 vector, Vector2 other) => 
        vector.Cross(other) < 0;

    /// <summary>
    /// returns true if the other vector is to its left with a max angle.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="other"></param>
    /// <param name="maxAngle"></param>
    /// <returns></returns>
    public static bool IsLeftAngle(this Vector2 vector, Vector2 other, float maxAngle = HalfQuarter) =>
        Mathf.Acos(-vector.Cross(other)) < maxAngle;

    public static bool IsRightAngle(this Vector2 vector, Vector2 other, float maxAngle = HalfQuarter) =>
        Mathf.Acos(vector.Cross(other)) < maxAngle;

    
    public static bool IsFloor(this Vector2 vector, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        Mathf.Acos(vector.Dot(upDirection)) <= maxSlope + 0.01f;

    public static bool IsCeiling(this Vector2 vector, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        Mathf.Acos(vector.Dot(-upDirection)) <= maxSlope + 0.01f;

    public static bool IsWall(this Vector2 vector, Vector2 upDirection, float maxSlope = HalfQuarter) =>
        !vector.IsFloor(upDirection, maxSlope) && !vector.IsCeiling(upDirection, maxSlope);
}