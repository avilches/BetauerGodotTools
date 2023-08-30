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

    public static void Circle(int cx, int cy, int r, Action<int, int> onPixel) {
        if (r <= 0) {
            onPixel(cx, cy);
            return;
        }
        if (r == 1) {
            onPixel(cx, cy - 1);
            onPixel(cx, cy + 1);
            onPixel(cx - 1, cy);
            onPixel(cx + 1, cy);
            return;
        }
        var x = r;
        var y = 0;

        onPixel(cx + x, cy + y);
        if (r > 0) {
            onPixel(cx - x, cy - y);
            onPixel(cx + y, cy + x);
            onPixel(cx - y, cy - x);
        }

        var error = 1 - r;
        while (x > y) {
            y++;
            if (error <= 0)
                error = error + 2 * y + 1; // fast.Draw up
            else {
                x--;
                error = error + 2 * y - 2 * x + 1;
            }
            if (x < y) break; // circle finished
            onPixel(cx + x, cy + y);
            onPixel(cx - x, cy + y);
            onPixel(cx + x, cy - y);
            onPixel(cx - x, cy - y);

            if (x != y) {
                onPixel(cx + y, cy + x);
                onPixel(cx - y, cy + x);
                onPixel(cx + y, cy - x);
                onPixel(cx - y, cy - x);
            }
        }
    }

    public static void GradientCircle(int cx, int cy, int r, Action<int, int, float> onPixel) {
        var radii = r * r;
        FillCircle(cx, cy, r, (x, y) => {
            var distanceSquared = Geometry.DistanceSquared(cx, cy, x, y);
            onPixel(x, y, distanceSquared / radii);
        });
    }


    public static void FillCircle(int cx, int cy, int r, Action<int, int> onPixel) {
        if (r <= 0) {
            onPixel(cx, cy);
            return;
        }
        if (r == 1) {
            onPixel(cx, cy - 1);
            onPixel(cx, cy + 1);
            onPixel(cx - 1, cy);
            onPixel(cx + 1, cy);
            onPixel(cx, cy);
            return;
        }

        Line(cx - r, cy, cx + r, cy, onPixel);
        Line(cx - r, cy, cx + r, cy, onPixel);
        Line(cx, cy + r, cx, cy + r, onPixel);
        Line(cx, cy - r, cx, cy - r, onPixel);

        var x = r;
        var y = 0;
        var error = 1 - r;
        while (x > y) {
            y++;
            if (error <= 0)
                error = error + 2 * y + 1; // fast.Draw up
            else {
                x--;
                error = error + 2 * y - 2 * x + 1;
            }
            if (x < y) break; // circle finished
            Line(cx - x, cy + y, cx + x, cy + y, onPixel);
            Line(cx - x, cy - y, cx + x, cy - y, onPixel);
            Line(cx - y, cy + x, cx + y, cy + x, onPixel);
            Line(cx - y, cy - x, cx + y, cy - x, onPixel);
        }
    }
}