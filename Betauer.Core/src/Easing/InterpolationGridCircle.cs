using System;
using Betauer.Core.Collision;

namespace Betauer.Core.Easing;

/// <summary>
/// Creates a virtual grid with a circle inside. The Width and Height defines the size of the grid and the center of the circle. If the Width and Height
/// are not set, the grid will be the bounding box of the circle by default. Changing the Radius will set the grid size of the bounding box of the circle.
/// 
/// GetValue will return a value from 0f to 1f based on the (x,y) coords inside the rect, returning 1 in the center of the circle located at (Width/2, Height/2),
/// which is the center of the grid too and it goes to 0 gradually when the coords touch the border of the circle. Outside of the circle the value will be 0.
/// </summary>
public class InterpolationCircleEllipse : IInterpolationGrid2D {
    private int _radius;

    public int Radius {
        get => _radius;
        set {
            _radius = value;
            Width = value * 2;
            Height = value * 2;
        }
    }
    
    public IInterpolation? Easing { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public InterpolationCircleEllipse(int radius, IInterpolation? easing = null) {
        _radius = radius;
        Easing = easing;
        Width = Radius * 2;
        Height = Radius * 2;
    }

    public InterpolationCircleEllipse(int radius, int width, int height, IInterpolation? easing = null) {
        Radius = radius;
        Easing = easing;
        Width = width;
        Height = height;
    }

    public float Get(int x, int y) {
        var rectX = x - Width / 2f;
        var rectY = y - Height / 2f;
        var value = Geometry.LerpCircle(Radius, rectX, rectY);
        return Easing?.Get(value) ?? value;
    }
    
    public void Loop(Action<int, int, float> action) {
        var width = Width;
        var height = Height;
        for (var xx = 0; xx < width; xx++) {
            for (var yy = 0; yy < height; yy++) {
                action.Invoke(xx, yy, Get(xx, yy));
            }
        }
    }
}