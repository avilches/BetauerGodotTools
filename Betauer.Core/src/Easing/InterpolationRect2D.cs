using System;

namespace Betauer.Core.Easing;

/// <summary>
/// Returns a value from 0f to 1f based on the (x,y) coords inside a rect where the center of the grid is 1 and it goes to 0 when the coords touch the border
/// </summary>
public class InterpolationRect2D  {
    public int Width { get; set; }
    public int Height { get; set; }
    public int CenterX { get; set; }
    public int CenterY { get; set; }
    public IInterpolation? Easing { get; set; }

    public InterpolationRect2D(int width, int height, IInterpolation? easing = null) {
        Width = width;
        Height = height;
        CenterX = Width / 2;
        CenterY = Height / 2;
        Easing = easing;
    }

    /// <summary>
    /// Center as a normalized value from 0 to 1, where (0.5, 0.5f) is the middle of the grid.
    /// </summary>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    public void SetCenter(float centerX, float centerY) {
        CenterX = (int)(Math.Clamp(centerX, 0f, 1f) * Width);
        CenterY = (int)(Math.Clamp(centerY, 0f, 1f) * Height);
    }

    public void SetCenterPosition(int x, int y) {
        CenterX = Math.Clamp(x, 0, Width - 1);
        CenterY = Math.Clamp(y, 0, Height - 1);
    }

    public float GetValue(int x, int y) {
        var value = Functions.GetRect2D(Width, Height, CenterX, CenterY, x, y);
        return Easing?.GetY(value) ?? value;
    }

    public void Loop(int x, int y, int width, int height, Action<float, int, int> action) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                action.Invoke(GetValue(xx, yy), xx, yy);
            }
        }
    }
}