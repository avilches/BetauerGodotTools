using System;
using Godot;

namespace Betauer.Core.Easing;

/// <summary>
/// Creates a virtual grid using the Width and Height to define size.
/// 
/// GetValue will return a value from 0f to 1f based on the (x,y) coords inside the rect, returning 1 in the center of the grid and it goes
/// to 0 gradually when the coords touch the border of the grid.
///
/// You can change the center of the grid using SetCenter or SetRelativeCenter.
/// </summary>
public class InterpolationGridRect2D : IInterpolationGrid2D {
    public int Width { get; set; }
    public int Height { get; set; }
    public int CenterX { get; set; }
    public int CenterY { get; set; }
    public IInterpolation? Easing { get; set; }

    public InterpolationGridRect2D(int width, int height, IInterpolation? easing = null) {
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
        CenterX = Mathf.RoundToInt(Math.Clamp(centerX, 0f, 1f) * Width);
        CenterY = Mathf.RoundToInt(Math.Clamp(centerY, 0f, 1f) * Height);
    }

    public void SetCenter(int x, int y) {
        CenterX = Math.Clamp(x, 0, Width - 1);
        CenterY = Math.Clamp(y, 0, Height - 1);
    }

    public float Get(int x, int y) {
        var value = Functions.GetRect2D(Width, Height, CenterX, CenterY, x, y);
        return Easing?.Get(value) ?? value;
    }

    public void Loop(Action<int, int, float> action) {
        for (var xx = 0; xx < Width; xx++) {
            for (var yy = 0; yy < Height; yy++) {
                action.Invoke(xx, yy, Get(xx, yy));
            }
        }
    }
}