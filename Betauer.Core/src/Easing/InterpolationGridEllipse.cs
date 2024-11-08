using System;
using Betauer.Core.Math.Geometry;

namespace Betauer.Core.Easing;

/// <summary>
/// Creates a virtual grid with a ellipse inside. The Width and Height defines the size of the grid and the center of the ellipse. If the Width and Height
/// are not set, the grid will be the bounding box of the ellipse by default. Changing the RadiusX and RadiusY will set the grid size of the bounding box of the ellipse.
/// 
/// GetValue will return a value from 0f to 1f based on the (x,y) coords inside the rect, returning 1 in the center of the ellipse located at (Width/2, Height/2),
/// which is the center of the grid too and it goes to 0 gradually when the coords touch the border of the ellipse. Outside of the ellipse the value will be 0.
/// </summary>
public class InterpolationGridEllipse : IInterpolationGrid2D {
    private int _radiusX;
    private int _radiusY;

    public int RadiusX {
        get => _radiusX;
        set {
            _radiusX = value;
            Width = value * 2;
            Height = value * 2;
        }
    }

    public int RadiusY {
        get => _radiusY;
        set {
            _radiusY = value;
            Width = value * 2;
            Height = value * 2;
        }
    }

    public IInterpolation? Easing { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public InterpolationGridEllipse(int radiusX, int radiusY, IInterpolation? easing = null) {
        _radiusX = radiusX;
        _radiusY = radiusY;
        Easing = easing;
        Width = RadiusY * 2;
        Height = RadiusY * 2;
    }

    public InterpolationGridEllipse(int radiusX, int radiusY, int width, int height, IInterpolation? easing = null) {
        RadiusX = radiusX;
        RadiusY = radiusY;
        Easing = easing;
        Width = width;
        Height = height;
    }

    public float Get(int x, int y) {
        var rectX = x - Width / 2f;
        var rectY = y - Height / 2f;
        var value = Lerps.LerpToEllipseCenter(RadiusX, RadiusY, rectX, rectY);
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