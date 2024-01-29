using System;
using Godot;

namespace Betauer.Core.Image;

public static partial class FastImageExtensions {
    public static void Fill(this FastImage fast, Color color, bool blend = true) {
        fast.FillRect(0, 0, fast.Width, fast.Height, color, blend);
    }

    public static void DrawLine(this FastImage fast, Vector2I start, Vector2I end, int width, Color color, bool blend = true) {
        DrawLine(fast, start.X, start.Y, end.X, end.Y, width, color, blend);
    }

    public static void DrawLine(this FastImage fast, int x1, int y1, int x2, int y2, int width, Color color, bool blend = true) {
        Draw.Line(x1, y1, x2, y2, width, (x, y) => fast.SetPixel(x, y, color, blend));
    }

    public static void DrawLineAntialiasing(this FastImage fast, Vector2I start, Vector2I end, int width, Color color) {
        DrawLineAntialiasing(fast, start.X, start.Y, end.X, end.Y, width, color);
    }

    public static void DrawLineAntialiasing(this FastImage fast, int x1, int y1, int x2, int y2, int width, Color color) {
        Draw.LineAntialiasing(x1, y1, x2, y2, width, (x, y, a) => {
            fast.SetPixel(x, y, a < color.A ? new Color(color, a) : color, true);
        });
    }

    public static void DrawRect(this FastImage fast, int x, int y, int width, int height, Color color, bool blend = true) {
        Draw.Rect(x, y, width, height, (x, y) => fast.SetPixel(x, y, color, blend));
    }

    public static void DrawRect(this FastImage fast, Rect2I rect2, Color color, bool blend) {
        Draw.Rect(rect2.Position.X, rect2.Position.Y, rect2.Size.X, rect2.Size.Y, (x, y) => fast.SetPixel(x, y, color, blend));
    }

    public static void FillRect(this FastImage fast, Rect2I rect2I, Color color, bool blend = true) {
        var position = rect2I.Position;
        var size = rect2I.Size;
        fast.FillRect(position.X, position.Y, size.X, size.Y, color, blend);
    }

    public static void FillRect(this FastImage fast, int x, int y, int width, int height, Color color, bool blend = true) {
        Draw.FillRect(x, y, width, height, (px, py) => fast.SetPixel(px, py, color, blend));
    }

    public static void DrawCircle(this FastImage fast, int cx, int cy, int r, Color color, bool blend = true) {
        Draw.Circle(cx, cy, r, (x, y) => fast.SetPixel(x, y, color, blend));
    }

    public static void FillCircle(this FastImage fast, int cx, int cy, int r, Color color, bool blend = true) {
        Draw.FillCircle(cx, cy, r, (x, y) => fast.SetPixel(x, y, color, blend));
    }
}