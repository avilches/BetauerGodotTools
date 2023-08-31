using System;
using Betauer.Core.Collision;

namespace Betauer.Core.Image;

public static partial class Draw {
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

    public static void Rect(int x, int y, int width, int height, Action<int, int> onPixel) {
        if (width <= 0 || height <= 0) return;
        if (width == 1 && y == 1) {
            onPixel(x, y);
            return;
        }
        var x2 = x + width;
        var y2 = y + height;
        Line(x, y, x2, y, onPixel);
        Line(x, y, x, y2, onPixel);
        Line(x2, y, x2, y2, onPixel);
        Line(x, y2, x2, y2, onPixel);
    }
}