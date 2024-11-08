using System;
using Godot;

namespace Betauer.Core.DataMath.Geometry;

public static partial class Geometry {

    public static Rect2 ShrinkRect(Rect2 rect, float maxPadding, Random random) {
        return ShrinkRect(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, maxPadding, random);
    }
    
    /// <summary>
    /// Creates a smaller version of the rectangle by removing a random padding from every side.
    /// </summary>
    public static Rect2 ShrinkRect(float x, float y, float width, float height, float maxPadding, Random random) {
        var maxAllowedPadding = System.Math.Min(maxPadding, System.Math.Min(width / 2-1, height / 2-1));

        var paddingLeft = random.NextSingle() * maxAllowedPadding;
        var paddingTop = random.NextSingle() * maxAllowedPadding;
        var paddingRight = random.NextSingle() * maxAllowedPadding;
        var paddingBottom = random.NextSingle() * maxAllowedPadding;

        var newX = x + paddingLeft;
        var newY = y + paddingTop;
        var newWidth = width - (paddingLeft + paddingRight);
        var newHeight = height - (paddingTop + paddingBottom);

        return new Rect2(newX, newY, newWidth, newHeight);
    }

    public static Rect2 ShrinkRect(Rect2 rect, float by) {
        return ShrinkRect(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, by);
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
    public static Rect2 ShrinkRectToEnsureRatio(float x, float y, float width, float height, float ratio) {
        var landscape = width > height;
        var currentRatio = (float)System.Math.Max(width, height) / System.Math.Min(width, height);
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
    
    public static Rect2 ShrinkRectProportional(Rect2 rect, float factor) {
        return ShrinkRectProportional(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, factor);
    }
    
    public static Rect2 ShrinkRectProportional(float x, float y, float width, float height, float factor) {
        if (factor < 0 || factor > 1) {
            throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be between 0 and 1.");
        }
        var newWidth = width * factor;
        var newHeight = height * factor;
        var newX = x + (width - newWidth) / 2f;
        var newY = y + (height - newHeight) / 2f;
        return new Rect2(newX, newY, newWidth, newHeight);
    }
    
    public static Rect2 ShrinkRect(float x, float y, float width, float height, float by) {
        var maxPadding = System.Math.Min(by, System.Math.Min(width / 2 - 1, height / 2 - 1));
        var newX = x + maxPadding;
        var newY = y + maxPadding;
        var newWidth = width - maxPadding * 2;
        var newHeight = height - maxPadding * 2;
        return new Rect2(newX, newY, newWidth, newHeight);
    }

    public static void Main2() {
        // Console.WriteLine(ShrinkRect(0, 0, 10, 10, 0));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 10, 1));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 10, 2));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 10, 4));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 10, 5));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 10, 6));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 9, 0));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 9, 1));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 9, 2));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 9, 4));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 9, 5));
        // Console.WriteLine(ShrinkRect(0, 0, 10, 9, 6));
        
        // Reduce width
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 18, 9, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 17, 9, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 16, 9, 16f/9));

        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 18, 9, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 17, 9, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 16, 9, 9f/16));
        
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 18, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 17, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 16, 16f/9));
        
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 18, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 17, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 16, 9f/16));
        
        
        
        // Reduce height
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 17, 9, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 15, 9, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 14, 9, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 13, 9, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 10, 9, 16f/9));

        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 17, 9, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 15, 9, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 14, 9, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 13, 9, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 10, 9, 9f/16));

        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 17, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 15, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 14, 16f/9));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 13, 16f/9));

        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 17, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 15, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 14, 9f/16));
        Console.WriteLine(ShrinkRectToEnsureRatio(0, 0, 9, 13, 9f/16));
    }
}