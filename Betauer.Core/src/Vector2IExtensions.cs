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
    /// Returns true if the end position is in the same line as the path direction
    /// </summary>
    /// <param name="start"></param>
    /// <param name="direction"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static bool SameDirection(this Vector2I start, Vector2I direction, Vector2I end) {
        var delta = end - start;

        if (direction.X != 0) {
            // Movimiento horizontal
            return delta.Y == 0 && Math.Sign(delta.X) == Math.Sign(direction.X);
        }
        // Movimiento vertical
        return delta.X == 0 && Math.Sign(delta.Y) == Math.Sign(direction.Y);
    }

    /// <summary>
    /// Returns true if the end position is in the same horizontal or vertical line as the start position
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static bool SameDirection(this Vector2I start, Vector2I end) {
        var delta = end - start;
        // Horizontal or vertical line
        return delta.X == 0 || delta.Y == 0;
    }

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
        if (!start.SameDirection(end)) {
            throw new ArgumentException("Points must be aligned horizontally or vertically");
        }
        var direction = start.DirectionTo(end);
        var distance =  Math.Max(Math.Abs(start.X - end.X), Math.Abs(start.Y - end.Y));
        var position = start;
        yield return position;
        for (var i = 0; i < distance; i++) {
            position += direction;
            yield return position;
        }
    }

    /// <summary>
    /// Calculates the direction vector from start to end.
    /// </summary>
    public static Vector2I DirectionTo(this Vector2I start, Vector2I end) {
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
        throw new ArgumentException("Points must be aligned horizontally or vertically");
    }


    /// <summary>
    /// Checks if the vector is a valid cardinal direction
    /// </summary>
    public static bool IsValidDirection(this Vector2I direction) {
        return Math.Abs(direction.X) + Math.Abs(direction.Y) == 1;
    }

    public static bool IsHorizontal(this Vector2I direction) {
        return direction.X != 0 && direction.Y == 0;
    }

    public static bool IsVertical(this Vector2I direction) {
        return direction.X == 0 && direction.Y != 0;
    }

    public static bool IsPerpendicular(this Vector2I direction, Vector2I other) {
        return direction.X * other.X + direction.Y * other.Y == 0;
    }

    public static bool IsParallel(this Vector2I direction, Vector2I other) {
        return direction.X * other.Y == direction.Y * other.X;
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