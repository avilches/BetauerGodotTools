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
    /// <returns>A Rect2 object representing the randomly generated rectangle.</returns>
    public static Rect2 CreateRandomRect2(float minRatio, float maxRatio, float minLong, float maxLong, RectanglePart rectanglePart, Random random) {
        float ratio = (float)(random.NextDouble() * (maxRatio - minRatio) + minRatio);
        float length = (float)(random.NextDouble() * (maxLong - minLong) + minLong);

        return CreateRect2(ratio, length, rectanglePart);
    }

    /// <summary>
    /// Creates a rectangle with a specified aspect ratio and length.
    /// </summary>
    /// <param name="ratio">The aspect ratio of the rectangle (width/height).</param>
    /// <param name="length">The length of the specified side of the rectangle.</param>
    /// <param name="rectanglePart">Specifies whether the length applies to Width, Height, or the Longer side.</param>
    /// <returns>A Rect2 object representing the generated rectangle.</returns>
    public static Rect2 CreateRect2(float ratio, float length, RectanglePart rectanglePart) {
        float width, height;

        // Modify limitDimension if it's set to Longer based on the ratio
        if (rectanglePart == RectanglePart.Longer) {
            rectanglePart = ratio >= 1 ? RectanglePart.Width : RectanglePart.Height;
        }

        if (rectanglePart == RectanglePart.Width) {
            width = length;
            height = width / ratio;
        } else { // LimitDimension.Height
            height = length;
            width = height * ratio;
        }

        return new Rect2(new Vector2(0, 0), new Vector2(width, height));
    }
    
    /// <summary>
    /// Positions a rectangle randomly within the bounds of another rectangle.
    /// </summary>
    /// <param name="bounds">The bounding rectangle within which the rect will be positioned.</param>
    /// <param name="rect">The rectangle to be positioned randomly within the bounds.</param>
    /// <param name="random">An instance of the Random class used to generate random values.</param>
    /// <returns>A Rect2I object representing the randomly positioned rectangle.</returns>
    public static Rect2 PositionRect2Randomly(Rect2 bounds, Rect2 rect, Random random) {
        var maxX = bounds.Size.X - rect.Size.X;
        var maxY = bounds.Size.Y - rect.Size.Y;
        var x = (float)(random.NextDouble() * (bounds.Position.X + maxX + 1));
        var y = (float)(random.NextDouble() * (bounds.Position.Y + maxY + 1));
        return new Rect2(new Vector2(x, y), rect.Size);
    }

    /// <summary>
    /// Returns a Rect2 with the same center as the original one, but with a smaller size (if needed) to ensure the ratio is kept.
    /// It doesn't care about the orientation of the rectangle, it will always try to shrink the longest side to ensure the ratio.
    /// The ratio could be in the form of 16:9 or 9:16, it will always try to make the longest side to be the one with the ratio.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public static Rect2 ShrinkRect2ToEnsureRatio(float x, float y, float width, float height, float ratio) {
        var landscape = width > height;
        var currentRatio = (float)Math.Max(width, height) / Math.Min(width, height);
        if (ratio < 1) ratio = 1f / ratio;
        if (currentRatio < ratio) {
            // Console.WriteLine($"No need to shring {width}/{height} ({currentRatio:0.00})");
            return new Rect2(x, y, width, height);
        }
        if (width > height) {
            if (!landscape) ratio = 1f / ratio;
            // El rectángulo es más ancho de lo necesario, reducir el ancho
            float newWidth = height * ratio;
            float deltaX = (width - newWidth) / 2;
            var r = new Rect2(x + deltaX, y, newWidth, height);
            // var newRatio = (float)Math.Max(r.Size.X, r.Size.Y) / Math.Min(r.Size.X, r.Size.Y);
            // Console.WriteLine($"Shrinked {width}/{height} ({currentRatio:0.00}) to {r.Size.X}/{r.Size.Y} ({newRatio:0.00})");
            return r;
        } else {
            if (!landscape) ratio = 1f / ratio;
            // El rectángulo es más alto de lo necesario, reducir la altura
            float newHeight = width / ratio;
            float deltaY = (height - newHeight) / 2;
            var r = new Rect2(x, y + deltaY, width, newHeight);
            // var newRatio = (float)Math.Max(r.Size.X, r.Size.Y) / Math.Min(r.Size.X, r.Size.Y);
            // Console.WriteLine($"Shrinked {width}/{height} ({currentRatio:0.00}) to {r.Size.X}/{r.Size.Y} ({newRatio:0.00})");
            return r;
        }
    }

    public static Rect2 ResizeRect2ByFactor(Rect2 rect, float factor) {
        return ResizeRect2ByFactor(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, factor);
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
    public static Rect2 ResizeRect2ByFactor(float x, float y, float width, float height, float factor) {
        if (factor <= 0) {
            throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be greater than 0.");
        }
        if (factor == 1f) {
            return new Rect2(x, y, width, height);
        }
        var newWidth = width * factor;
        var newHeight = height * factor;
        var newX = x + (width - newWidth) / 2f;
        var newY = y + (height - newHeight) / 2f;
        return new Rect2(newX, newY, newWidth, newHeight);
    }


}