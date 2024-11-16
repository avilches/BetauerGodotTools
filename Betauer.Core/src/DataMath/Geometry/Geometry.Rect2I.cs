using System;
using Godot;

namespace Betauer.Core.DataMath.Geometry;

public static partial class Geometry {
    /// <summary>
    /// Creates a random rectangle with a specified aspect ratio range.
    /// </summary>
    /// <param name="minRatio">The minimum aspect ratio of the rectangle (width/height).</param>
    /// <param name="maxRatio">The maximum aspect ratio of the rectangle (width/height).</param>
    /// <param name="minLong">The minimum length of the specified side of the rectangle.</param>
    /// <param name="maxLong">The maximum length of the specified side of the rectangle.</param>
    /// <param name="rectanglePart">Specifies whether the limit applies to Width, Height, or the Longer side.</param>
    /// <param name="random">An instance of the Random class used to generate random values.</param>
    /// <returns>A Rect2I object representing the randomly generated rectangle.</returns>
    public static Rect2I CreateRandomRect2I(float minRatio, float maxRatio, int minLong, int maxLong, RectanglePart rectanglePart, Random random) {
        float ratio = (float)(random.NextDouble() * (maxRatio - minRatio) + minRatio);
        int length = random.Next(minLong, maxLong + 1);

        return CreateRect2I(ratio, length, rectanglePart);
    }

    /// <summary>
    /// Creates a rectangle with a specified aspect ratio and length.
    /// </summary>
    /// <param name="ratio">The aspect ratio of the rectangle (width/height).</param>
    /// <param name="length">The length of the specified side of the rectangle.</param>
    /// <param name="rectanglePart">Specifies whether the length applies to Width, Height, or the Longer side.</param>
    /// <returns>A Rect2I object representing the generated rectangle.</returns>
    public static Rect2I CreateRect2I(float ratio, int length, RectanglePart rectanglePart) {
        int width, height;

        // Modify limitDimension if it's set to Longer based on the ratio
        if (rectanglePart == RectanglePart.Longer) {
            rectanglePart = ratio >= 1 ? RectanglePart.Width : RectanglePart.Height;
        }

        if (rectanglePart == RectanglePart.Width) {
            width = length;
            height = Mathf.RoundToInt(width / ratio);
        } else { // LimitDimension.Height
            height = length;
            width = Mathf.RoundToInt(height * ratio);
        }

        return new Rect2I(new Vector2I(0, 0), new Vector2I(width, height));
    }

    /// <summary>
    /// Enum to specify which dimension should be limited.
    /// </summary>
    public enum RectanglePart {
        Width,
        Height,
        Longer
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
        var currentRatio = (float)Math.Max(width, height) / Math.Min(width, height); // 16/9
        if (ratio < 1) ratio = 1f / ratio;  // 0.6 -> 16/9
        if (currentRatio <= ratio) {
            // Console.WriteLine($"No need to shrink {width}/{height} ({currentRatio:0.00})");
            return new Rect2I(x, y, width, height);
        }
        if (landscape) {
            // El rectángulo es más ancho de lo necesario, reducir el ancho
            var newWidth = Mathf.RoundToInt(height * ratio);
            var deltaX = (width - newWidth) / 2;
            var r = new Rect2I(x + deltaX, y, newWidth, height);
            // var newRatio = (float)Math.Max(r.Size.X, r.Size.Y) / Math.Min(r.Size.X, r.Size.Y);
            // Console.WriteLine($"Shrinked {width}/{height} ({currentRatio:0.00}) to {r.Size.X}/{r.Size.Y} ({newRatio:0.00})");
            return r;
        } else {
            ratio = 1f / ratio;
            // El rectángulo es más alto de lo necesario, reducir la altura
            var newHeight = Mathf.RoundToInt(width / ratio);
            var deltaY = (height - newHeight) / 2;
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