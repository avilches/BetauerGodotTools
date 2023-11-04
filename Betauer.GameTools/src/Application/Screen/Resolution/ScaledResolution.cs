using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Screen.Resolution;

public class ScaledResolution : Resolution, IEquatable<ScaledResolution> {
    public static readonly IEqualityComparer<ScaledResolution> ComparerByBaseSize = new ScaledResolutionComparer();
    public readonly Vector2I Base;
    public readonly Vector2 Scale;

    public ScaledResolution(Vector2I @base, Vector2I size) : base(size) {
        Base = @base;
        Scale = new Vector2(size.X, size.Y) / @base;
    }

    public ScaledResolution(Vector2I @base, int x, int y) : this(@base, new Vector2I(x, y)) {
    }

    public bool HasSameAspectRatio() => Math.Abs(Scale.X - Scale.Y) < 0.00001f;
    public bool IsScaleXInteger() => IsInteger(Scale.X);
    public bool IsScaleYInteger() => IsInteger(Scale.Y);
    /*
    public bool IsPixelPerfectDownScale() => Scale.X is 0.5f or 0.25f or 0.125f or 0.0625f;

    // Please check IsPixelPerfectScale/IsPixelPerfectDownScale before to use these values
    public string GetPixelPerfectScale() =>
        Scale.X switch {
            >= 1f => ((int)Math.Floor(Scale.X)).ToString(),
            0.5f => "1/2",
            0.25f => "1/4",
            0.125f => "1/8",
            0.0625f => "1/16",
            _ => Scale.X.ToString()
        };
    */
    
    /**
     * Two ScaledResolutions are equal if the base and the size are equals
     * Scale is a computed value (size / base)
     * Aspect ratio (inherited from parent Resolution class) is a computed value (size.X/size.Y, width/height)
     */
    public static bool operator ==(ScaledResolution? left, ScaledResolution? right) {
        if (ReferenceEquals(left, right)) return true; // equals or both null
        if (right is null || left is null) return false; // one of them is null
        return left.Equals(right); // not the same reference
    }

    public static bool operator !=(ScaledResolution? left, ScaledResolution? right) {
        if (ReferenceEquals(left, right)) return false; // equals or both null
        if (right is null || left is null) return true; // one of them is null
        return !left.Equals(right); // not the same reference
    }

    public override bool Equals(object? obj) =>
        obj is ScaledResolution other && Equals(other);

    public bool Equals(ScaledResolution? obj) =>
        ReferenceEquals(this, obj) ||
        (obj is { } && Base.Equals(obj.Base) && Size.Equals(obj.Size));

    public override int GetHashCode() {
        unchecked {
            var hashCode = base.GetHashCode();
            hashCode = (hashCode * 397) ^ Base.GetHashCode();
            hashCode = (hashCode * 397) ^ Size.GetHashCode();
            return hashCode;
        }
    }

    private static bool IsInteger(float x) => Math.Abs(x - Math.Floor(x)) < 0.00001f;
}