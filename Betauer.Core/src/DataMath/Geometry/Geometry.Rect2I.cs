using System;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.DataMath.Geometry;

public static partial class Geometry {
    /// <summary>
    /// Creates a random rectangle with a specified aspect ratio and orientation.
    /// </summary>
    /// <param name="ratio">The aspect ratio of the rectangle (width/height).</param>
    /// <param name="horizontal">If true, the rectangle will be wider than it is tall; otherwise, it will be taller than it is wide.</param>
    /// <param name="minLong">The minimum length of the longer side of the rectangle.</param>
    /// <param name="maxLong">The maximum length of the longer side of the rectangle.</param>
    /// <param name="random">An instance of the Random class used to generate random values.</param>
    /// <returns>A Rect2I object representing the randomly generated rectangle.</returns>
    public static Rect2I CreateRandomRect2I(float ratio, bool horizontal, int minLong, int maxLong, Random random) {
        int width, height;
        if (horizontal) {
            width = random.Next(minLong, maxLong + 1);
            height = (int)(width / ratio);
        } else {
            height = random.Next(minLong, maxLong + 1);
            width = (int)(height / ratio);
        }
        return new Rect2I(new Vector2I(0, 0), new Vector2I(width, height));
    }

    /// <summary>
    /// Positions a rectangle randomly within the bounds of another rectangle.
    /// </summary>
    /// <param name="bounds">The bounding rectangle within which the rect will be positioned.</param>
    /// <param name="rect">The rectangle to be positioned randomly within the bounds.</param>
    /// <param name="random">An instance of the Random class used to generate random values.</param>
    /// <returns>A Rect2I object representing the randomly positioned rectangle.</returns>
    public static Rect2I PositionRect2IRandomly(Rect2I bounds, Rect2I rect, Random random) {
        var maxX = bounds.Size.X - rect.Size.X;
        var maxY = bounds.Size.Y - rect.Size.Y;
        var x = random.Next(bounds.Position.X, bounds.Position.X + maxX + 1);
        var y = random.Next(bounds.Position.Y, bounds.Position.Y + maxY + 1);
        return new Rect2I(new Vector2I(x, y), rect.Size);
    }

    /// <summary>
    /// Returns a Rect2I with the same center as the original one, but with a smaller size (if needed) to ensure the ratio is kept.
    /// It doesn't care about the orientation of the rectangle, it will always try to shrink the longest side to ensure the ratio.
    /// The ratio could be in the form of 16:9 or 9:16, it will always try to make the longest side to be the one with the ratio.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public static Rect2I ShrinkRect2IToEnsureRatio(int x, int y, int width, int height, float ratio) {
        var landscape = width > height;
        var currentRatio = (float)System.Math.Max(width, height) / System.Math.Min(width, height);
        if (ratio < 1) ratio = 1f / ratio;
        if (currentRatio < ratio) {
            // Console.WriteLine($"No need to shring {width}/{height} ({currentRatio:0.00})");
            return new Rect2I(x, y, width, height);
        }
        if (width > height) {
            if (!landscape) ratio = 1f / ratio;
            // El rectángulo es más ancho de lo necesario, reducir el ancho
            int newWidth = (int)(height * ratio);
            int deltaX = (width - newWidth) / 2;
            var r = new Rect2I(x + deltaX, y, newWidth, height);
            // var newRatio = (float)Math.Max(r.Size.X, r.Size.Y) / Math.Min(r.Size.X, r.Size.Y);
            // Console.WriteLine($"Shrinked {width}/{height} ({currentRatio:0.00}) to {r.Size.X}/{r.Size.Y} ({newRatio:0.00})");
            return r;
        } else {
            if (!landscape) ratio = 1f / ratio;
            // El rectángulo es más alto de lo necesario, reducir la altura
            int newHeight = (int)(width / ratio);
            int deltaY = (height - newHeight) / 2;
            var r = new Rect2I(x, y + deltaY, width, newHeight);
            // var newRatio = (float)Math.Max(r.Size.X, r.Size.Y) / Math.Min(r.Size.X, r.Size.Y);
            // Console.WriteLine($"Shrinked {width}/{height} ({currentRatio:0.00}) to {r.Size.X}/{r.Size.Y} ({newRatio:0.00})");
            return r;
        }
    }

    public static Rect2I ResizeRect2IByFactor(Rect2I rect, float factor) {
        return ResizeRect2IByFactor(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, factor);
    }

    /// <summary>
    /// Resizes the given rectangle proportionally by a specified factor.
    /// </summary>
    /// <param name="x">The x-coordinate of the rectangle.</param>
    /// <param name="y">The y-coordinate of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="factor">The factor by which to shrink the rectangle. Must be between 0 and 1.</param>
    /// <returns>A Rect2 object representing the shrunken rectangle.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the factor is 0.</exception>
    public static Rect2I ResizeRect2IByFactor(int x, int y, int width, int height, float factor) {
        if (factor <= 0) {
            throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be greater than 0.");
        }
        if (factor == 1f) {
            return new Rect2I(x, y, width, height);
        }
        var newWidth = Mathf.RoundToInt(width * factor);
        var newHeight = Mathf.RoundToInt(height * factor);
        var newX = Mathf.RoundToInt(x + (width - newWidth) / 2f);
        var newY = Mathf.RoundToInt(y + (height - newHeight) / 2f);
        return new Rect2I(newX, newY, newWidth, newHeight);
    }
}