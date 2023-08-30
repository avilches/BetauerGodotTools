using System;

namespace Betauer.Core.Image;

public class Draw {
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

    public static void Line(int x, int y, int x2, int y2, Action<int, int> onPixel) {
        if (x == x2 && y == y2) {
            onPixel(x, y);
            return;
        }
        if (x == x2) {
            if (y > y2) (y, y2) = (y2, y);
            for (var i = y; i <= y2; i++) {
                onPixel(x, i);
            }
            return;
        }
        if (y == y2) {
            if (x > x2) (x, x2) = (x2, x);
            for (var i = x; i <= x2; i++) {
                onPixel(i, y);
            }
            return;
        }

        var dx = x2 - x;
        var dy = y2 - y;
        var stepX = Math.Sign(dx);
        var stepY = Math.Sign(dy);
        dx = Math.Abs(dx);
        dy = Math.Abs(dy);
        var swap = false;
        if (dy > dx) {
            (dx, dy) = (dy, dx);
            swap = true;
        }
        var e = 2 * dy - dx;
        for (var i = 0; i < dx; i++) {
            onPixel(x, y);
            while (e >= 0) {
                if (swap) x += stepX;
                else y += stepY;
                e -= 2 * dx;
            }
            if (swap) y += stepY;
            else x += stepX;
            e += 2 * dy;
        }
    }
}