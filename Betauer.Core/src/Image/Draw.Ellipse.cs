using System;
using Betauer.Core.Easing;
using Godot;

namespace Betauer.Core.Image;

public static partial class Draw {
    public static void Ellipse(int cx, int cy, int rx, int ry, Action<int, int> onPixel) {
        double dx, dy, d1, d2, x, y;
        x = 0;
        y = ry;

        // Initial decision parameter of region 1
        d1 = (ry * ry) - (rx * rx * ry) +
             (0.25f * rx * rx);
        dx = 2 * ry * ry * x;
        dy = 2 * rx * rx * y;

        // For region 1
        while (dx < dy) {
            // Print points based on 4-way symmetry
            onPixel((int)Mathf.Round(x + cx), (int)Mathf.Round(y + cy));
            onPixel((int)Mathf.Round(-x + cx), (int)Mathf.Round(y + cy));
            onPixel((int)Mathf.Round(x + cx), (int)Mathf.Round(-y + cy));
            onPixel((int)Mathf.Round(-x + cx), (int)Mathf.Round(-y + cy));
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
            onPixel((int)Mathf.Round(x + cx), (int)Mathf.Round(y + cy));
            onPixel((int)Mathf.Round(-x + cx), (int)Mathf.Round(y + cy));
            onPixel((int)Mathf.Round(x + cx), (int)Mathf.Round(-y + cy));
            onPixel((int)Mathf.Round(-x + cx), (int)Mathf.Round(-y + cy));

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

    public static void GradientEllipse(int cx, int cy, int rx, int ry, Action<int, int, float> onPixel, IEasing? easing = null) {
        FillEllipseNoAccurate(cx, cy, rx, ry, (x, y) => {
            float dx = x - cx;
            float dy = y - cy;
            var pos = (dx * dx) / (rx * rx) + (dy * dy) / (ry * ry);
            onPixel(x, y, easing?.GetY(pos) ?? pos);
        });
    }

    /// <summary>
    /// This method is not accurate in the edges of the ellipse, where some points could be not drawn.
    /// But it ensures that only the pixels inside the ellipse are drawn, which is useful for filling the ellipse with a gradient (gradients need to be
    /// drawn only once per pixel. Accumulate more calls in the same pixel could change the color of the pixel if the pixel has a transparent color, creating
    /// artifacts in the gradient)
    /// </summary>
    /// <param name="cx"></param>
    /// <param name="cy"></param>
    /// <param name="rx"></param>
    /// <param name="ry"></param>
    /// <param name="onPixel"></param>
    public static void FillEllipseNoAccurate(int cx, int cy, int rx, int ry, Action<int, int> onPixel) {
        // Loop the x axis from left to right
        for (var x = -rx; x <= rx; x++) {
            // Calculate the height of the ellipse at this x position
            var height = (int)Math.Round(Math.Sqrt((1 - (x * x) / (double)(rx * rx)) * ry * ry));
            // Draw a vertical line from -height to height
            for (var y = -height; y <= height; y++) {
                onPixel(cx + x, cy + y);
            }
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
        double dx, dy, d1, d2, x, y;
        x = 0;
        y = ry;

        // Initial decision parameter of region 1
        d1 = (ry * ry) - (rx * rx * ry) +
             (0.25f * rx * rx);
        dx = 2 * ry * ry * x;
        dy = 2 * rx * rx * y;

        // For region 1
        while (dx < dy) {
            // Print points based on 4-way symmetry
            Line((int)Mathf.Round(x + cx), (int)Mathf.Round(y + cy),
                (int)Mathf.Round(-x + cx), (int)Mathf.Round(y + cy), 1, onPixel);
            Line((int)Mathf.Round(x + cx), (int)Mathf.Round(-y + cy),
                (int)Mathf.Round(-x + cx), (int)Mathf.Round(-y + cy), 1, onPixel);
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
            Line((int)Mathf.Round(x + cx), (int)Mathf.Round(y + cy),
                (int)Mathf.Round(-x + cx), (int)Mathf.Round(y + cy), 1, onPixel);
            Line((int)Mathf.Round(x + cx), (int)Mathf.Round(-y + cy),
                (int)Mathf.Round(-x + cx), (int)Mathf.Round(-y + cy), 1, onPixel);


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
}