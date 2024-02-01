using System;
using Betauer.Core.Collision;
using Betauer.Core.Easing;

namespace Betauer.Core.Image;

public static partial class Draw {
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

        onPixel(cx + x, cy);
        onPixel(cx - x, cy);
        onPixel(cx, cy + x);
        onPixel(cx, cy - x);

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

    public static void GradientCircle(int cx, int cy, int r, Action<int, int, float> onPixel, IEasing? easing = null) {
        var radii = r * r;
        FillCircle(cx, cy, r, (x, y) => {
            var distanceSquared = Geometry.DistanceSquared(cx, cy, x, y);
            var pos = distanceSquared / radii;
            onPixel(x, y, easing?.GetY(pos) ?? pos);
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