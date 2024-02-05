using System;
using Godot;

namespace Betauer.Core.Easing;

/// <summary>
/// Returns a value from 0f to 1f based on the (x,y) coords inside a rect where the center of the grid is 1 and it goes to 0 when the coords touch the border
/// </summary>
public class InterpolationCircle : IInterpolation2D {
    public float Radius { get; set; }
    public IInterpolation? Easing { get; set; }

    public InterpolationCircle(float radius, IInterpolation? easing = null) {
        Radius = radius;
        Easing = easing;
    }

    public float Get(float x, float y) {
        var value = Functions.GetCircle(Radius, x - Radius, y - Radius);
        return Easing?.Get(value) ?? value;
    }
}