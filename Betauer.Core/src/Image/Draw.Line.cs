using System;
using Betauer.Core.Easing;
using Godot;
using FastNoiseLite = Betauer.Core.DataMath.Data.FastNoiseLite;

namespace Betauer.Core.Image;

public static partial class Draw {
    public static void Line(Vector2I start, Vector2I end, int width, Action<int, int> onPixel) {
        Line(start.X, start.Y, end.X, end.Y, width, onPixel);
    }

    public static void Line(int x0, int y0, int x1, int y1, int width, Action<int, int> onPixel) {
        if (width == 0) return;
        if (width == 1) {
            Line1Width(x0, y0, x1, y1, onPixel);
            return;
        }
        if (x0 == x1) {
            // Vertical line
            if (y0 > y1) (y0, y1) = (y1, y0);
            for (var w = -width / 2; w <= width / 2; w++) {
                for (var i = y0; i <= y1; i++) {
                    onPixel(x0 + w, i);
                }
            }
        } else if (y0 == y1) {
            // Horizontal line
            if (x0 > x1) (x0, x1) = (x1, x0);
            for (var w = -width / 2; w <= width / 2; w++) {
                for (var i = x0; i <= x1; i++) {
                    onPixel(i, y0 + w);
                }
            }
        } else {
            // Diagonal
            int dx = Math.Abs(x1-x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1-y0), sy = y0 < y1 ? 1 : -1;
            int err = dx - dy, e2, x2, y2; /* error value e_xy */
            var ed = dx+dy == 0 ? 1f : Mathf.Sqrt((float)dx*dx + (float)dy*dy);

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
    }

    private static void Line1Width(int x0, int y0, int x1, int y1, Action<int, int> onPixel) {
        if (x0 == x1 && y0 == y1) {
            // one point case
            onPixel(x0, y0);
            return;
        }
        if (x0 == x1) {
            // horizontal line
            if (y0 > y1) (y0, y1) = (y1, y0);
            for (var i = y0; i <= y1; i++) {
                onPixel(x0, i);
            }
            return;
        }
        if (y0 == y1) {
            // vertical line
            if (x0 > x1) (x0, x1) = (x1, x0);
            for (var i = x0; i <= x1; i++) {
                onPixel(i, y0);
            }
            return;
        }

        // diagonal
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

    // http://members.chello.at/~easyfilter/bresenham.html
    public static void LineAntialiasing(Vector2I start, Vector2I end, int width, Action<int, int, float> onPixel) {
        LineAntialiasing(start.X, start.Y, end.X, end.Y, width, onPixel);
    }

    public static void LineAntialiasing(int x0, int y0, int x1, int y1, int width, Action<int, int, float> onPixel) {
        if (width == 0) return;
        if (width == 1) {
            LineAntialiasing1Width(x0, y0, x1, y1, onPixel);
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
    private static void LineAntialiasing1Width(int x0, int y0, int x1, int y1, Action<int, int, float> onPixel) {
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

    public static int Seed = 0;

    private static readonly IInterpolation Interpolation = Easing.Interpolation.Mirror(Easing.Interpolation.Bias(0.8f));

    public static void LineNoise(int x0, int y0, int x1, int y1, FastNoiseLite noiseGen, int scale, Action<int, int, float> onPixel, int? seed = null) {
        LineNoise(new Vector2I(x0, y0), new Vector2I(x1, y1), noiseGen, scale, onPixel, seed);
    }

    public static void LineNoise(Vector2I start, Vector2I end, FastNoiseLite noiseGen, int scale, Action<int, int, float> onPixel, int? seed = null) {
        var pixelsPerSegment = scale <= 10 ? 0.5f : 3f / scale;
        var lineLength = ((Vector2)start).DistanceTo(end);
        var segments = Mathf.Max(1, Mathf.RoundToInt(lineLength / pixelsPerSegment));
        var points = 1 + segments;
        var angle = Mathf.Atan2(end.Y - start.Y, end.X - start.X);
        var realSeed = seed ?? Seed;
        if (!seed.HasValue) {
            Seed = ++Seed % 999999;
        }
        var offsetX = JenkinsMix((uint)realSeed, (uint)points, (uint)(angle * 1000));
        var offsetY = JenkinsMix(offsetX, (uint)points, (uint)angle);

        for (var i = 0; i < points; i++) {
            var xInterval = pixelsPerSegment * Mathf.Cos(angle);
            var yInterval = pixelsPerSegment * Mathf.Sin(angle);
            var x = start.X + xInterval * i;
            var y = start.Y + yInterval * i;
            var t = (float)i / (points - 1);
            // noise is a value from -1 to 1
            var normalizedNoise = noiseGen.GetNoise(i * pixelsPerSegment + offsetX, i * pixelsPerSegment + offsetY);
            // fix starts at 0, goes to 1 and ends at 0 again. This ensures the line starts and ends at the same point
            var fix = Interpolation.Get(t);
            var offset = normalizedNoise * scale * fix;
            var xOffset = offset * Mathf.Cos(angle - Mathf.Pi / 2);
            var yOffset = offset * Mathf.Sin(angle - Mathf.Pi / 2);
            onPixel(Mathf.RoundToInt(x + xOffset), Mathf.RoundToInt(y + yOffset), 1f);
        }
    }

    private static uint JenkinsMix(uint a, uint b, uint c) {
        a -= b; a -= c; a ^= (c >> 13);
        b -= c; b -= a; b ^= (a << 8);
        c -= a; c -= b; c ^= (b >> 13);
        a -= b; a -= c; a ^= (c >> 12);
        b -= c; b -= a; b ^= (a << 16);
        c -= a; c -= b; c ^= (b >> 5);
        a -= b; a -= c; a ^= (c >> 3);
        b -= c; b -= a; b ^= (a << 10);
        c -= a; c -= b; c ^= (b >> 15);
        return c % 999999;
    }
    
}