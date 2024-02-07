using System;
using Godot;

namespace Betauer.Core.Easing;

/// <summary>
/// Creates a virtual grid with a ellipse inside. The Width and Height defines the size of the grid and the center of the ellipse. If the Width and Height
/// are not set, the grid will be the bounding box of the ellipse by default. Changing the RadiusX and RadiusY will set the grid size of the bounding box of the ellipse.
/// 
/// GetValue will return a value from 0f to 1f based on the (x,y) coords inside the rect, returning 1 in the center of the ellipse located at (Width/2, Height/2),
/// which is the center of the grid too and it goes to 0 gradually when the coords touch the border of the ellipse. Outside of the ellipse the value will be 0.
/// </summary>
public class InterpolationGridEllipseRotated : IInterpolationGrid2D {
    private float _cos;
    private float _sin;
    private int _radiiX;   
    private int _radiiY;
    private int _radiusX;
    private int _radiusY;
    private float _rotation;

    public int Width { get; set; }
    public int Height { get; set; }
    
    public int RadiusX {
        get => _radiusX;
        set {
            _radiusX = value;
            Update();
        }
    }

    public int RadiusY {
        get => _radiusY;
        set {
            _radiusY = value;
            Update();
        }
    }

    public float Rotation {
        get => _rotation;
        set {
            _rotation = value;
            Update();
        }
    }

    public IInterpolation? Easing { get; set; }

    public InterpolationGridEllipseRotated(int radiusX, int radiusY, float rotation, IInterpolation? easing = null) {
        _radiusX = radiusX;
        _radiusY = radiusY;
        _rotation = rotation;
        Update();
        Easing = easing;
    }
    
    public InterpolationGridEllipseRotated(int radiusX, int radiusY, int width, int height, float rotation, IInterpolation? easing = null) {
        _radiusX = radiusX;
        _radiusY = radiusY;
        _rotation = rotation;
        Update();
        Width = width;
        Height = height;
        Easing = easing;
    }
    
    private void Update() {
        _cos = Mathf.Cos(Rotation);
        _sin = Mathf.Sin(Rotation);
        _radiiX = RadiusX * RadiusX;
        _radiiY = RadiusY * RadiusY;
        // Calculate the bounding box of the rotated ellipse
        Width = Mathf.RoundToInt(RadiusX * Math.Abs(_cos) + RadiusY * Math.Abs(_sin)) * 2;
        Height = Mathf.RoundToInt(RadiusX * Math.Abs(_sin) + RadiusY * Math.Abs(_cos)) * 2;

        // Calculate the bounding box of the rotated ellipse
        // var a = Math.Pow(RadiusX * _cos, 2) + Math.Pow(RadiusY * _sin, 2);
        // var b = 2 * (RadiusX * RadiusY * _sin * _cos);
        // var c = Math.Pow(RadiusX * _sin, 2) + Math.Pow(RadiusY * _cos, 2);
        // Width = Mathf.RoundToInt(2 * Math.Sqrt(a + b + c));
        // Height = Mathf.RoundToInt(2 * Math.Sqrt(a - b + c));
    }

    public float Get(int x, int y) {
        var rectX = x - (Width / 2f);
        var rectY = y - (Height / 2f);
        var rotatedX = _cos * rectX + _sin * rectY;
        var rotatedY = -_sin * rectX + _cos * rectY;
        // It works like Functions.GetEllipseRotated
        var value = Mathf.Clamp(1 - ((rotatedX * rotatedX) / _radiiX + (rotatedY * rotatedY) / _radiiY), 0f, 1f);
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