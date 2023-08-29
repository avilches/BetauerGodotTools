using System;
using Godot;

namespace Betauer.Core.Image;

/// <summary>
/// A class to read and write pixels from a Image or a Texture2D faster than using GetPixel
/// </summary>
public static partial class FastImageExtensions {

    public static void Fill(this FastImage fast, Color color, bool blend = true) {  
        fast.FillRect(0, 0, fast.Width, fast.Height, color, blend);
    }

    public static void FillRect(this FastImage fast, Rect2I rect2I, Color color, bool blend = true) { 
        var position = rect2I.Position;
        var size = rect2I.Size;
        fast.FillRect(position.X, position.Y, size.X, size.Y, color, blend);
    }

    public static void FillRect(this FastImage fast, int x, int y, int width, int height, Color color, bool blend = true) { 
        var lineEnd = y + height;
        for (var drawY = y; drawY < lineEnd; drawY++) {
            var pixelEnd = x + width;
            for (var drawX = x; drawX < pixelEnd; drawX++) {
                fast.SetPixel(drawX, drawY, color, blend);
            }
        }
    }

    public static void DrawCircle(this FastImage fast, int cx, int cy, int r, Color color, bool fill = false, bool blend = true) { 
        if (fill) fast.DrawCircleFill(cx, cy, r, color, blend);
        else fast.DrawCircleOutline(cx, cy, r, color, blend);
    }

    private static void DrawCircleOutline(this FastImage fast, int centerX, int centerY, int r, Color color, bool blend = true) { 
        var x = r;
        var y = 0;

        fast.SetPixel(centerX + x, centerY + y, color, blend);
        if (r > 0) {
            fast.SetPixel(centerX - x, centerY - y, color, blend);
            fast.SetPixel(centerX + y, centerY + x, color, blend);
            fast.SetPixel(centerX - y, centerY - x, color, blend);
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
            fast.SetPixel(centerX + x, centerY + y, color, blend);
            fast.SetPixel(centerX - x, centerY + y, color, blend);
            fast.SetPixel(centerX + x, centerY - y, color, blend);
            fast.SetPixel(centerX - x, centerY - y, color, blend);

            if (x != y) {
                fast.SetPixel(centerX + y, centerY + x, color, blend);
                fast.SetPixel(centerX - y, centerY + x, color, blend);
                fast.SetPixel(centerX + y, centerY - x, color, blend);
                fast.SetPixel(centerX - y, centerY - x, color, blend);
            }
        }
    }

    public static void DrawCircleFill(this FastImage fast, int cx, int cy, int r, Color color, bool blend = true) { 
        var x = r;
        var y = 0;

        fast.DrawLine(cx - x, cy + y, cx + x, cy + y, color, blend);
        fast.DrawLine(cx - x, cy - y, cx + x, cy - y, color, blend);
        fast.DrawLine(cx - y, cy + x, cx + y, cy + x, color, blend);
        fast.DrawLine(cx - y, cy - x, cx + y, cy - x, color, blend);

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
            fast.DrawLine(cx - x, cy + y, cx + x, cy + y, color, blend);
            fast.DrawLine(cx - x, cy - y, cx + x, cy - y, color, blend);
            fast.DrawLine(cx - y, cy + x, cx + y, cy + x, color, blend);
            fast.DrawLine(cx - y, cy - x, cx + y, cy - x, color, blend);
        }
    }

    public static void DrawLine(this FastImage fast, int x, int y, int x2, int y2, Color color, bool blend = true) { 
        if (x == x2 && y == y2) {
            fast.SetPixel(x, y, color, blend);
            return;
        }
        if (x == x2) {
            if (y > y2) (y, y2) = (y2, y);
            for (var i = y; i <= y2; i++) {
                fast.SetPixel(x, i, color, blend);
            }
            return;
        }
        if (y == y2) {
            if (x > x2) (x, x2) = (x2, x);
            for (var i = x; i <= x2; i++) {
                fast.SetPixel(i, y, color, blend);
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
            fast.SetPixel(x, y, color, blend);
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


    public static void DrawRectangle(this FastImage fast, int x, int y, int width, int height, Color color, bool fill = false, bool blend = true) { 
        if (fill) fast.FillRect(x, y, width, height, color, blend);
        else fast.DrawRectangleOutline(x, y, width, height, color, blend);
    }

    public static void DrawRectangleOutline(this FastImage fast, int x, int y, int width, int height, Color color, bool blend = true) { 
        var x2 = x + width;
        var y2 = y + height;
        fast.DrawLine(x, y, x2, y, color, blend);
        fast.DrawLine(x, y, x, y2, color, blend);
        fast.DrawLine(x2, y, x2, y2, color, blend);
        fast.DrawLine(x, y2, x2, y2, color, blend);
    }
    
}