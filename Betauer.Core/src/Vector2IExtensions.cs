using System;
using System.Collections.Generic;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Betauer.Core;

public static class Vector2IExtensions {

    public static Vector2I Inverse(this Vector2I from) {
        return from * -1;
    }

    public static string ToDirectionString(this Vector2I dir) {
        string directionText;
        if (dir == Vector2I.Up)
            directionText = "Up";
        else if (dir == Vector2I.Right)
            directionText = "Right";
        else if (dir == Vector2I.Down)
            directionText = "Down";
        else if (dir == Vector2I.Left)
            directionText = "Left";
        else
            directionText = dir.ToString();
        return directionText;
    }

    /// <summary>
    /// Returns true if the end position is in the same horizontal or vertical line as the start position
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static bool IsOrthogonal(this Vector2I start, Vector2I end) {
        var delta = end - start;
        // Horizontal or vertical line
        return delta.X == 0 || delta.Y == 0;
    }

    /// <summary>
    /// Returns true if the end position is in the same line as the path direction from start
    /// Works with any direction (orthogonal or diagonal)
    /// </summary>
    /// <param name="start">Starting position</param>
    /// <param name="direction">Direction vector</param>
    /// <param name="end">End position to check</param>
    /// <returns>True if end is in the same direction from start</returns>
    public static bool IsSameDirection(this Vector2I start, Vector2I direction, Vector2I end) {
        var delta = end - start;

        // Handle zero direction
        if (direction == Vector2I.Zero)
            return delta == Vector2I.Zero;

        // Check if the vectors are collinear by comparing their slopes
        // For vectors to be collinear, cross product must be zero
        if (delta.X * direction.Y != delta.Y * direction.X)
            return false;

        // If they're collinear, check that they point in the same (not opposite) direction
        // If both X components are non-zero, compare their signs
        if (direction.X != 0)
            return Math.Sign(delta.X) == Math.Sign(direction.X);
        // Otherwise compare Y components
        else
            return Math.Sign(delta.Y) == Math.Sign(direction.Y);
    }

    /// <summary>
    /// Checks if two lines (each defined by a point and a direction) are on the same infinite line
    /// </summary>
    /// <param name="point1">Point on the first line</param>
    /// <param name="direction1">Direction of the first line</param>
    /// <param name="point2">Point on the second line</param>
    /// <param name="direction2">Direction of the second line</param>
    /// <returns>True if both lines are on the same infinite line, false otherwise</returns>
    public static bool IsSameLine(this Vector2I point1, Vector2I direction1, Vector2I point2, Vector2I direction2) {
        // First, check if the directions are parallel (or antiparallel)
        if (!direction1.IsParallel(direction2)) {
            return false;
        }

        // For horizontal and vertical lines, we can simply check if the constant coordinate is the same
        if (direction1.IsHorizontal()) {
            // For horizontal lines, the Y coordinate must be the same
            return point1.Y == point2.Y;
        }

        if (direction1.IsVertical()) {
            // For vertical lines, the X coordinate must be the same
            return point1.X == point2.X;
        }

        // For non-axis-aligned lines, we need a different approach
        // Calculate the cross product of the vector from point1 to point2 and direction1
        // If points are on the same line, this cross product should be zero
        var pointDiff = point2 - point1;
        return pointDiff.X * direction1.Y == pointDiff.Y * direction1.X;
    }

    /// <summary>
    /// Returns positions in a line starting from a point in a specified direction for a given length
    /// </summary>
    /// <param name="start">The starting position</param>
    /// <param name="direction">The direction vector</param>
    /// <param name="length">The number of steps to take in the direction</param>
    /// <returns>An enumerable of positions including the start position and all positions along the line</returns>
    public static IEnumerable<Vector2I> GetPositions(this Vector2I start, Vector2I direction, int length) {
        var position = start;
        yield return position;
        for (var i = 0; i < length; i++) {
            position += direction;
            yield return position;
        }
    }

    /// <summary>
    /// Returns all positions in a straight line between two Vector2I points
    /// </summary>
    /// <param name="start">The starting position</param>
    /// <param name="end">The ending position</param>
    /// <returns>An enumerable of all positions in the straight line from start to end</returns>
    public static IEnumerable<Vector2I> GetPositions(this Vector2I start, Vector2I end) {
        if (!start.IsOrthogonal(end)) {
            throw new ArgumentException($"Point {end} must be aligned horizontally or vertically with {start}");
        }

        var direction = start.GetOrthogonalDirectionTo(end);
        var distance = Math.Max(Math.Abs(start.X - end.X), Math.Abs(start.Y - end.Y));

        // Reuse the other GetPositions method to avoid code duplication
        return start.GetPositions(direction, distance);
    }

    /// <summary>
    /// Calculates the orthogonal unit direction vector from start to end.
    /// </summary>
    public static Vector2I GetOrthogonalDirectionTo(this Vector2I start, Vector2I end) {
        var (x, y) = end - start;

        // Determine direction (only one axis should have non-zero value)
        if (x != 0 && y == 0) {
            // Horizontal direction
            return new Vector2I(Math.Sign(x), 0);
        }
        if (x == 0 && y != 0) {
            // Vertical direction
            return new Vector2I(0, Math.Sign(y));
        }
        throw new ArgumentException($"Point {end} must be aligned horizontally or vertically with {start}");
    }


    /// <summary>
    /// Checks if the vector is a valid cardinal direction
    /// </summary>
    public static bool IsOrthogonalDirection(this Vector2I direction) {
        return Math.Abs(direction.X) + Math.Abs(direction.Y) == 1;
    }

    public static bool IsHorizontal(this Vector2I direction) {
        return direction.X != 0 && direction.Y == 0;
    }

    public static bool IsVertical(this Vector2I direction) {
        return direction.X == 0 && direction.Y != 0;
    }

    public static bool IsPerpendicular(this Vector2I one, Vector2I other) {
        return one.X * other.X + one.Y * other.Y == 0;
    }

    public static bool IsParallel(this Vector2I one, Vector2I other) {
        return one.X * other.Y == one.Y * other.X;
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

    public static Vector2I RightPos(this Vector2I from) {
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

    /// <summary>
    /// Euclidean distance: Sqrt of (x1-x2)² + (y1-y2)²
    /// </summary>
    public static float DistanceTo(this Vector2I from, Vector2I to) {
        var dx = from.X - to.X;
        var dy = from.Y - to.Y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Non Squared Euclidean distance: (x1-x2)² + (y1-y2)²
    /// Not real distante, only useful for comparing distances without the need of the square root
    /// </summary>
    public static int DistanceSquaredTo(this Vector2I from, Vector2I to) {
        var dx = from.X - to.X;
        var dy = from.Y - to.Y;
        return dx * dx + dy * dy;
    }

    /// <summary>
    /// Manhattan distance: |x1-x2| + |y1-y2|
    /// </summary>
    public static int ManhattanDistanceTo(this Vector2I from, Vector2I to) {
        return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
    }

    /// <summary>
    /// Chebyshev distance: max(|x1-x2|, |y1-y2|)
    /// </summary>
    public static int ChebyshevDistanceTo(this Vector2I from, Vector2I to) {
        return Math.Max(Math.Abs(from.X - to.X), Math.Abs(from.Y - to.Y));
    }
}