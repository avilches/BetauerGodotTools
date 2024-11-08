using System;
using Betauer.Core.DataMath.Geometry;
using Betauer.Core.Easing;
using Godot;

namespace Betauer.Core.Image;

public static partial class Draw {
    
    public static void Rect(Rect2I rect, Action<int, int> onPixel) {
        Rect(rect.Position, rect.Size, onPixel);
    }

    public static void Rect(Vector2I position, Vector2I size, Action<int, int> onPixel) {
        Rect(position.X, position.Y, size.X, size.Y, onPixel);
    }

    public static void Rect(int x, int y, int width, int height, Action<int, int> onPixel) {
        if (width <= 0 || height <= 0) return;
        if (width == 1 && y == 1) {
            onPixel(x, y);
            return;
        }
        var x2 = x + width;
        var y2 = y + height;
        Line1Width(x, y, x2, y, onPixel);
        Line1Width(x, y, x, y2, onPixel);
        Line1Width(x2, y, x2, y2, onPixel);
        Line1Width(x, y2, x2, y2, onPixel);
    }

    public static void FillRect(Rect2I rect, Action<int, int> onPixel) {
        FillRect(rect.Position, rect.Size, onPixel);
    }

    public static void FillRect(Vector2I position, Vector2I size, Action<int, int> onPixel) {
        FillRect(position.X, position.Y, size.X, size.Y, onPixel);
    }

    public static void FillRect(int x, int y, int width, int height, Action<int, int> onPixel) {
        if (width <= 0 || height <= 0) return;
        if (width == 1 && y == 1) {
            onPixel(x, y);
            return;
        }
        var lineEnd = y + height;
        for (var drawY = y; drawY < lineEnd; drawY++) {
            var pixelEnd = x + width;
            for (var drawX = x; drawX < pixelEnd; drawX++) {
                onPixel(drawX, drawY);
            }
        }
    }

    public static void GradientRect(Rect2I rect, Vector2I center, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        GradientRect(rect.Position, rect.Size, center, onPixel, easing);
    }

    public static void GradientRect(Vector2I position, Vector2I size, Vector2I center, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        GradientRect(position.X, position.Y, size.X, size.Y, center.X, center.Y, onPixel, easing);
    }

    public static void GradientRect(int x, int y, int width, int height, int centerX, int centerY, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        if (width <= 0 || height <= 0) return;
        if (width == 1 && y == 1) {
            onPixel(x, y, 1f);
            return;
        }
        for (var drawY = 0; drawY < height; drawY++) {
            for (var drawX = 0; drawX < width; drawX++) {
                var pos = Lerps.LerpToRectCustomCenter(width, height, centerX, centerY, drawX, drawY);
                onPixel(drawX + x, drawY + y, easing?.Get(pos) ?? pos);
            }
        }
    }

    public static void GradientRect(Rect2I rect, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        GradientRect(rect.Position, rect.Size, onPixel, easing);
    }

    public static void GradientRect(Vector2I position, Vector2I size, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        GradientRect(position.X, position.Y, size.X, size.Y, onPixel, easing);
    }

    public static void GradientRect(int x, int y, int width, int height, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        if (width <= 0 || height <= 0) return;
        if (width == 1 && y == 1) {
            onPixel(x, y, 1f);
            return;
        }
        for (var drawY = 0; drawY < height; drawY++) {
            for (var drawX = 0; drawX < width; drawX++) {
                var pos = Lerps.LerpToRectCenter(width, height, drawX, drawY);
                onPixel(drawX + x, drawY + y, easing?.Get(pos) ?? pos);
            }
        }
    }
}