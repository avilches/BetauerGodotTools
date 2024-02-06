using System;
using Betauer.Core.Collision;
using Betauer.Core.Easing;

namespace Betauer.Core.Image;

public static partial class Draw {
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

    public static void GradientRect(int x, int y, int width, int height, int centerX, int centerY, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        if (width <= 0 || height <= 0) return;
        if (width == 1 && y == 1) {
            onPixel(x, y, 1f);
            return;
        }
        for (var drawY = 0; drawY < height; drawY++) {
            for (var drawX = 0; drawX < width; drawX++) {
                var pos = Functions.GetRect2D(width, height, centerX, centerY, drawX, drawY);
                onPixel(drawX + x, drawY + y, easing?.Get(pos) ?? pos);
            }
        }
    }

    public static void GradientRect(int x, int y, int width, int height, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        if (width <= 0 || height <= 0) return;
        if (width == 1 && y == 1) {
            onPixel(x, y, 1f);
            return;
        }
        for (var drawY = 0; drawY < height; drawY++) {
            for (var drawX = 0; drawX < width; drawX++) {
                var pos = Functions.GetRect2D(width, height, drawX, drawY);
                onPixel(drawX + x, drawY + y, easing?.Get(pos) ?? pos);
            }
        }
    }
}