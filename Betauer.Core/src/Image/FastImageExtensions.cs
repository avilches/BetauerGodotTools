using System;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public static class FastImageExtensions {

    public static void Fill(this FastImage fast, Color color) {
        fast.FillRect(0, 0, fast.Width, fast.Height, color);
    }

    public static void FillRect(this FastImage fast, Rect2I rect2I, Color color) {
        var position = rect2I.Position;
        var size = rect2I.Size;
        fast.FillRect(position.X, position.Y, size.X, size.Y, color);
    }

    public static void FillRect(this FastImage fast, int x, int y, int width, int height, Color color) {
        var lineEnd = y + height;
        for (var drawY = y; drawY < lineEnd; drawY++) {
            var pixelEnd = x + width;
            for (var drawX = x; drawX < pixelEnd; drawX++) {
                fast.SetPixel(drawX, drawY, color);
            }
        }
    }

    public static void DrawCircle(this FastImage fast, int cx, int cy, int r, Color color, bool fill = false) {
        if (fill) fast.DrawCircleFill(cx, cy, r, color);
        else fast.DrawCircleOutline(cx, cy, r, color);
    }

    private static void DrawCircleOutline(this FastImage fast, int centerX, int centerY, int r, Color color) {
        var x = r;
        var y = 0;

        fast.SetPixel(centerX + x, centerY + y, color);
        if (r > 0) {
            fast.SetPixel(centerX - x, centerY - y, color);
            fast.SetPixel(centerX + y, centerY + x, color);
            fast.SetPixel(centerX - y, centerY - x, color);
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
            fast.SetPixel(centerX + x, centerY + y, color);
            fast.SetPixel(centerX - x, centerY + y, color);
            fast.SetPixel(centerX + x, centerY - y, color);
            fast.SetPixel(centerX - x, centerY - y, color);

            if (x != y) {
                fast.SetPixel(centerX + y, centerY + x, color);
                fast.SetPixel(centerX - y, centerY + x, color);
                fast.SetPixel(centerX + y, centerY - x, color);
                fast.SetPixel(centerX - y, centerY - x, color);
            }
        }
    }

    public static void DrawCircleFill(this FastImage fast, int cx, int cy, int r, Color color) {
        var x = r;
        var y = 0;

        fast.DrawLine(cx - x, cy + y, cx + x, cy + y, color);
        fast.DrawLine(cx - x, cy - y, cx + x, cy - y, color);
        fast.DrawLine(cx - y, cy + x, cx + y, cy + x, color);
        fast.DrawLine(cx - y, cy - x, cx + y, cy - x, color);

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
            fast.DrawLine(cx - x, cy + y, cx + x, cy + y, color);
            fast.DrawLine(cx - x, cy - y, cx + x, cy - y, color);
            fast.DrawLine(cx - y, cy + x, cx + y, cy + x, color);
            fast.DrawLine(cx - y, cy - x, cx + y, cy - x, color);
        }
    }

    public static void DrawLine(this FastImage fast, int x, int y, int x2, int y2, Color color) {
        if (x == x2 && y == y2) {
            fast.SetPixel(x, y, color);
            return;
        }
        if (x == x2) {
            if (y > y2) (y, y2) = (y2, y);
            for (var i = y; i <= y2; i++) {
                fast.SetPixel(x, i, color);
            }
            return;
        }
        if (y == y2) {
            if (x > x2) (x, x2) = (x2, x);
            for (var i = x; i <= x2; i++) {
                fast.SetPixel(i, y, color);
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
            fast.SetPixel(x, y, color);
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


    public static void DrawRectangle(this FastImage fast, int x, int y, int width, int height, Color color, bool fill = false) {
        if (fill) fast.DrawRectangleFill(x, y, width, height, color);
        else fast.DrawRectangleOutline(x, y, width, height, color);
    }

    public static void DrawRectangleFill(this FastImage fast, int x, int y, int width, int height, Color color) {
        var x2 = x + width;
        var y2 = y + height;
        for (var drawY = y; drawY < y2; drawY++) {
            for (var drawX = x; drawX < x2; drawX++) {
                fast.SetPixel(drawX, drawY, color);
            }
        }
    }

    public static void DrawRectangleOutline(this FastImage fast, int x, int y, int width, int height, Color color) {
        var x2 = x + width;
        var y2 = y + height;
        fast.DrawLine(x, y, x2, y, color);
        fast.DrawLine(x, y, x, y2, color);
        fast.DrawLine(x2, y, x2, y2, color);
        fast.DrawLine(x, y2, x2, y2, color);
    }

}