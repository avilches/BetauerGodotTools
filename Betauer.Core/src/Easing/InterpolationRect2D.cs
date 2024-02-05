using System;
using Godot;

namespace Betauer.Core.Easing;

/// <summary>
/// Returns a value from 0f to 1f based on the (x,y) coords inside a rect where the center of the grid is 1 and it goes to 0 when the coords touch the border
/// </summary>
public class InterpolationRect2D : IInterpolation2D {
    public float Width { get; set; }
    public float Height { get; set; }
    public float CenterX { get; set; }
    public float CenterY { get; set; }
    public IInterpolation? Easing { get; set; }

    public InterpolationRect2D(float width, float height, IInterpolation? easing = null) {
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
    public void SetRelativeCenter(float centerX, float centerY) {
        CenterX = Math.Clamp(centerX, 0f, 1f) * Width;
        CenterY = Math.Clamp(centerY, 0f, 1f) * Height;
    }

    public void SetCenter(float x, float y) {
        CenterX = Math.Clamp(x, 0, Width);
        CenterY = Math.Clamp(y, 0, Height);
    }

    public float Get(float x, float y) {
        var value = Functions.GetRect2D(Width, Height, CenterX, CenterY, x, y);
        return Easing?.Get(value) ?? value;
    }

    public void Loop(float x, float y, float width, float height, Action<float, float, float> action) {
        for (var xx = x; xx < width - x; xx++) {
            for (var yy = y; yy < height - y; yy++) {
                action.Invoke(Get(xx, yy), xx, yy);
            }
        }
    }
}