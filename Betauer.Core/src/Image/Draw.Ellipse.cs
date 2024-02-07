using System;
using Betauer.Core.Easing;
using static Godot.Mathf;

namespace Betauer.Core.Image;

public static partial class Draw {
    public static void Ellipse(int cx, int cy, int rx, int ry, Action<int, int> onPixel) {
        double dx, dy, d1, d2, x, y;
        x = 0;
        y = ry;

        // Initial decision parameter of region 1
        d1 = (ry * ry) - (rx * rx * ry) + (0.25f * rx * rx);
        dx = 2 * ry * ry * x;
        dy = 2 * rx * rx * y;

        // For region 1
        while (dx < dy) {
            // Print points based on 4-way symmetry
            onPixel(RoundToInt(x + cx), RoundToInt(y + cy));
            onPixel(RoundToInt(-x + cx), RoundToInt(y + cy));
            onPixel(RoundToInt(x + cx), RoundToInt(-y + cy));
            onPixel(RoundToInt(-x + cx), RoundToInt(-y + cy));
            // Checking and updating value of
            // decision parameter based on algorithm
            if (d1 < 0) {
                x++;
                dx = dx + (2 * ry * ry);
                d1 = d1 + dx + (ry * ry);
            } else {
                x++;
                y--;
                dx = dx + (2 * ry * ry);
                dy = dy - (2 * rx * rx);
                d1 = d1 + dx - dy + (ry * ry);
            }
        }

        // Decision parameter of region 2
        d2 = ((ry * ry) * ((x + 0.5f) * (x + 0.5f)))
             + ((rx * rx) * ((y - 1) * (y - 1)))
             - (rx * rx * ry * ry);

        // Plotting points of region 2
        while (y >= 0) {
            // printing points based on 4-way symmetry
            onPixel(RoundToInt(x + cx), RoundToInt(y + cy));
            onPixel(RoundToInt(-x + cx), RoundToInt(y + cy));
            onPixel(RoundToInt(x + cx), RoundToInt(-y + cy));
            onPixel(RoundToInt(-x + cx), RoundToInt(-y + cy));

            // Checking and updating parameter
            // value based on algorithm
            if (d2 > 0) {
                y--;
                dy = dy - (2 * rx * rx);
                d2 = d2 + (rx * rx) - dy;
            } else {
                y--;
                x++;
                dx = dx + (2 * ry * ry);
                dy = dy - (2 * rx * rx);
                d2 = d2 + dx - dy + (rx * rx);
            }
        }
    }

    public static void EllipseRotated(int cx, int cy, int rx, int ry, float rotation, Action<int, int> onPixel) {
        var cosTheta = Cos(rotation);
        var sinTheta = Sin(rotation);

        // Calculate the approximate perimeter of the ellipse
        var perimeter = Pi * (3 * (rx + ry) - Sqrt((3 * rx + ry) * (rx + 3 * ry)));

        // Calculate the number of points we want to use to draw the ellipse
        var numPoints = (int)Math.Max(100, perimeter);

        for (var i = 0; i < numPoints; i++) {
            var theta = 2 * Pi * i / numPoints;

            // Calculate the point on the ellipse before rotation
            var x = rx * Cos(theta);
            var y = ry * Sin(theta);

            // Rotate the point
            var rotatedX = cx + RoundToInt(x * cosTheta - y * sinTheta);
            var rotatedY = cy + RoundToInt(x * sinTheta + y * cosTheta);

            onPixel(rotatedX, rotatedY);
        }
    }

    /// <summary>
    /// This method is accurate with the edges, but it draws some lines duplicated. This is ok for drawing the ellipse with a solid color
    /// </summary>
    /// <param name="cx"></param>
    /// <param name="cy"></param>
    /// <param name="rx"></param>
    /// <param name="ry"></param>
    /// <param name="onPixel"></param>
    public static void FillEllipse(int cx, int cy, int rx, int ry, Action<int, int> onPixel) {
        // A shorter version, but no accurate in edges
        // for(int y=-ry; y<=ry; y++) {
        //     for(int x = -rx; x <= rx; x++) {
        //         if(x*x*ry*ry+y*y*rx*rx <= ry*ry*rx*rx)
        //             onPixel(cx+x, cy+y);
        //          }
        //      }
        // }

        double dx, dy, d1, d2, x, y;
        x = 0;
        y = ry;

        // Initial decision parameter of region 1
        d1 = (ry * ry) - (rx * rx * ry) + (0.25f * rx * rx);
        dx = 2 * ry * ry * x;
        dy = 2 * rx * rx * y;

        // For region 1
        while (dx < dy) {
            // Print points based on 4-way symmetry
            Line(RoundToInt(x + cx), RoundToInt(y + cy),
                RoundToInt(-x + cx), RoundToInt(y + cy), 1, onPixel);
            Line(RoundToInt(x + cx), RoundToInt(-y + cy),
                RoundToInt(-x + cx), RoundToInt(-y + cy), 1, onPixel);
            // Checking and updating value of
            // decision parameter based on algorithm
            if (d1 < 0) {
                x++;
                dx = dx + (2 * ry * ry);
                d1 = d1 + dx + (ry * ry);
            } else {
                x++;
                y--;
                dx = dx + (2 * ry * ry);
                dy = dy - (2 * rx * rx);
                d1 = d1 + dx - dy + (ry * ry);
            }
        }

        // Decision parameter of region 2
        d2 = ((ry * ry) * ((x + 0.5f) * (x + 0.5f)))
             + ((rx * rx) * ((y - 1) * (y - 1)))
             - (rx * rx * ry * ry);

        // Plotting points of region 2
        while (y >= 0) {
            // printing points based on 4-way symmetry
            Line(RoundToInt(x + cx), RoundToInt(y + cy),
                RoundToInt(-x + cx), RoundToInt(y + cy), 1, onPixel);
            Line(RoundToInt(x + cx), RoundToInt(-y + cy),
                RoundToInt(-x + cx), RoundToInt(-y + cy), 1, onPixel);

            // Checking and updating parameter
            // value based on algorithm
            if (d2 > 0) {
                y--;
                dy = dy - (2 * rx * rx);
                d2 = d2 + (rx * rx) - dy;
            } else {
                y--;
                x++;
                dx = dx + (2 * ry * ry);
                dy = dy - (2 * rx * rx);
                d2 = d2 + dx - dy + (rx * rx);
            }
        }
    }

    public static void FillEllipseRotated(int cx, int cy, int rx, int ry, float rotation, Action<int, int> onPixel) {
        var cos = Math.Cos(rotation);
        var sin = Math.Sin(rotation);
        var radiix = rx * rx;
        var radiiy = ry * ry;
        // Calculate the bounding box of the rotated ellipse
        var width = RoundToInt(rx * Math.Abs(cos) + ry * Math.Abs(sin));
        var height = RoundToInt(rx * Math.Abs(sin) + ry * Math.Abs(cos));

        // Adjust the loop to iterate over the bounding box
        for (var x = -width; x <= width; x++) {
            for (var y = -height; y <= height; y++) {
                var tdx = cos * x + sin * y;
                var tdy = sin * x - cos * y;
                if ((tdx * tdx) / radiix + (tdy * tdy) / radiiy <= 1f) {
                    onPixel(cx + x, cy + y);
                }
            }
        }
    }

    public static void GradientEllipse(int cx, int cy, int rx, int ry, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        // Loop the x axis from left to right
        for (var x = -rx; x <= rx; x++) {
            // Calculate the height of the ellipse at this x position
            var height = RoundToInt(Math.Sqrt((1 - (x * x) / (double)(rx * rx)) * ry * ry));
            // Draw a vertical line from -height to height
            for (var y = -height; y <= height; y++) {
                var pos = Functions.GetEllipse(rx, ry, x, y);
                onPixel(cx + x, cy + y, easing?.Get(pos) ?? pos);
            }
        }
    }

    public static void GradientEllipseRotated(int cx, int cy, int rx, int ry, float rotation, Action<int, int, float> onPixel, IInterpolation? easing = null) {
        var cos = Math.Cos(rotation);
        var sin = Math.Sin(rotation);
        var radiix = rx * rx;
        var radiiy = ry * ry;

        // Calculate the bounding box of the rotated ellipse
        var width = RoundToInt(rx * Math.Abs(cos) + ry * Math.Abs(sin));
        var height = RoundToInt(rx * Math.Abs(sin) + ry * Math.Abs(cos));

        // Adjust the loop to iterate over the bounding box
        for (var x = -width; x <= width; x++) {
            for (var y = -height; y <= height; y++) {
                var tdx = cos * x + sin * y;
                var tdy = sin * x - cos * y;
                var ellipseValue = (tdx * tdx) / radiix + (tdy * tdy) / radiiy;
                if (ellipseValue <= 1f) {
                    var pos = 1f - (float)Math.Sqrt(ellipseValue);
                    onPixel(cx + x, cy + y, easing?.Get(pos) ?? pos);
                }
            }
        }
    }
}