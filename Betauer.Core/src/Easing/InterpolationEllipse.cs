using System;
using Godot;

namespace Betauer.Core.Easing;

/// <summary>
/// Returns a value from 0f to 1f based on the (x,y) coords inside a rect where the center of the grid is 1 and it goes to 0 when the coords touch the border
/// </summary>
public class InterpolationEllipse : IInterpolation2D {
    public float RadiusX { get; set; }
    public float RadiusY { get; set; }
    public IInterpolation? Easing { get; set; }

    public InterpolationEllipse(float radiusX, float radiusY, IInterpolation? easing = null) {
        RadiusX = radiusX;
        RadiusY = radiusY;
        Easing = easing;
    }

    public float Get(float x, float y) {
        var value = Functions.GetEllipse(RadiusX, RadiusY, x - RadiusX, y - RadiusY);
        return Easing?.Get(value) ?? value;
    }
}