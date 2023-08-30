using Godot;

namespace Betauer.Core.Image;

public static partial class FastImageExtensions {
    public static void Fill(this FastImage fast, Color color, bool blend = true) {
        fast.FillRect(0, 0, fast.Width, fast.Height, color, blend);
    }

    public static void DrawLine(this FastImage fast, int x, int y, int x2, int y2, Color color, bool blend = true) {
        Draw.Line(x, y, x2, y2, (x, y) => fast.SetPixel(x, y, color, blend));
    }

    public static void DrawRect(this FastImage fast, int x, int y, int width, int height, Color color, bool blend = true) {
        Draw.Rect(x, y, width, height, (x, y) => fast.SetPixel(x, y, color, blend));
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