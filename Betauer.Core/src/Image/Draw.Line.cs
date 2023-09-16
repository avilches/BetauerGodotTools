using System;
using Godot;

namespace Betauer.Core.Image;

public static partial class Draw {
    public static void Line(int x0, int y0, int x1, int y1, int width, Action<int, int> onPixel) {
        if (width == 0) return;
        if (width == 1) {
            Line(x0, y0, x1, y1, onPixel);
            return;
        }
        int dx = Math.Abs(x1-x0), sx = x0 < x1 ? 1 : -1; 
        int dy = Math.Abs(y1-y0), sy = y0 < y1 ? 1 : -1; 
        int err = dx-dy, e2, x2, y2;                          /* error value e_xy */
        var ed = dx+dy == 0 ? 1f : Mathf.Sqrt((float)dx*dx+(float)dy*dy);

        for (width = (width + 1) / 2;;) { /* pixel loop */
            onPixel(x0, y0);
            e2 = err; x2 = x0;
            if (2 * e2 >= -dx) { /* x step */
                for (e2 += dy, y2 = y0; e2 < ed * width && (y1 != y2 || dx > dy); e2 += dx)
                    onPixel(x0, y2 += sy);
                if (x0 == x1) break;
                e2 = err; err -= dy; x0 += sx; 
            }
            if (2 * e2 <= dy) { /* y step */
                for (e2 = dx - e2; e2 < ed * width && (x1 != x2 || dx < dy); e2 += dy)
                    onPixel(x2 += sx, y0);
                if (y0 == y1) break;
                err += dx; y0 += sy; 
            }
        }    
    }

    // width 1
    public static void Line(int x0, int y0, int x1, int y1, Action<int, int> onPixel) {
        if (x0 == x1 && y0 == y1) {
            onPixel(x0, y0);
            return;
        }
        if (x0 == x1) {
            if (y0 > y1) (y0, y1) = (y1, y0);
            for (var i = y0; i <= y1; i++) {
                onPixel(x0, i);
            }
            return;
        }
        if (y0 == y1) {
            if (x0 > x1) (x0, x1) = (x1, x0);
            for (var i = x0; i <= x1; i++) {
                onPixel(i, y0);
            }
            return;
        }

        var dx = Math.Abs(x1 - x0);
        var sx = x0 < x1 ? 1 : -1;
        var dy = -Math.Abs(y1 - y0);
        var sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2; /* error value e_xy */

        for (;;) { /* loop */
            onPixel(x0, y0);
            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) {
                err += dy;
                x0 += sx;
            } /* e_xy+e_x > 0 */
            if (e2 <= dx) {
                err += dx;
                y0 += sy;
            } /* e_xy+e_y < 0 */
        }
    }
    
    // aliasing with width 1
    // http://members.chello.at/~easyfilter/bresenham.html
    public static void LineAntialiasing(int x0, int y0, int x1, int y1, int width, Action<int, int, float> onPixel) {
        if (width == 0) return;
        if (width == 1) {
            LineAntialiasing(x0, y0, x1, y1, onPixel);
            return;
        }
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1; 
        int err = dx-dy, e2, x2, y2;                          /* error value e_xy */
        var ed = dx + dy == 0 ? 1f : Mathf.Sqrt((float)dx * dx + (float)dy * dy);

        for (width = (width + 1) / 2;;) { /* pixel loop */
            onPixel(x0, y0, 1 - Math.Max(0, Math.Abs(err - dx + dy) / ed - width + 1));
            e2 = err; x2 = x0;
            if (2 * e2 >= -dx) { /* x step */
                for (e2 += dy, y2 = y0; e2 < ed * width && (y1 != y2 || dx > dy); e2 += dx)
                    onPixel(x0, y2 += sy, 1 - Math.Max(0, Math.Abs(e2) / ed - width + 1));
                if (x0 == x1) break;
                e2 = err; err -= dy; x0 += sx; 
            }
            if (2 * e2 <= dy) { /* y step */
                for (e2 = dx - e2; e2 < ed * width && (x1 != x2 || dx < dy); e2 += dy)
                    onPixel(x2 += sx, y0, 1 - Math.Max(0, Math.Abs(e2) / ed - width + 1));
                if (y0 == y1) break;
                err += dx; y0 += sy; 
            }
        }    
    }
    
    // aliasing with width 1
    public static void LineAntialiasing(int x0, int y0, int x1, int y1, Action<int, int, float> onPixel) {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1; 
        int err = dx-dy, e2, x2;                       /* error value e_xy */
        var ed = dx + dy == 0 ? 1f : (float)Math.Sqrt((float)dx * dx + (float)dy * dy);

        for ( ; ; ){                                         /* pixel loop */
            onPixel(x0, y0, 1 - Math.Abs(err - dx + dy) / ed);
            e2 = err; x2 = x0;
            if (2 * e2 >= -dx) { /* x step */
                if (x0 == x1) break;
                if (e2 + dy < ed) onPixel(x0, y0 + sy, 1 - (e2 + dy) / ed);
                err -= dy; x0 += sx; 
            }
            if (2 * e2 <= dy) { /* y step */
                if (y0 == y1) break;
                if (dx - e2 < ed) onPixel(x2 + sx, y0, 1 - (dx - e2) / ed);
                err += dx; y0 += sy; 
            }
        }
    }
}