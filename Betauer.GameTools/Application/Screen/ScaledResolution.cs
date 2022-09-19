using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Screen {
    internal class ScaledResolutionComparer : IEqualityComparer<ScaledResolution> {
        internal ScaledResolutionComparer() {
        }

        public bool Equals(ScaledResolution x, ScaledResolution y) => x.Size.Equals(y.Size);
        public int GetHashCode(ScaledResolution scaledResolution) => scaledResolution.Size.GetHashCode();
    }

    public class ScaledResolution : Resolution, IEquatable<ScaledResolution> {
        public static readonly IEqualityComparer<ScaledResolution> ComparerByBaseSize = new ScaledResolutionComparer();
        public readonly Vector2 Base;
        public readonly Vector2 Scale;

        public ScaledResolution(Vector2 @base, Vector2 size) : base(size) {
            Base = @base;
            Scale = size / @base;
        }

        public ScaledResolution(Vector2 @base, int x, int y) : this(@base, new Vector2(x, y)) {
        }

        public bool HasSameAspectRatio() => Math.Abs(Scale.x - Scale.y) < 0.00001f;
        public bool IsPixelPerfectScale() => HasSameAspectRatio() && IsInteger(Scale.x);

        // Please check IsPixelPerfectScale before to use this value!!
        public int GetPixelPerfectScale() => (int)Math.Floor(Scale.x);

        /**
         * Two ScaledResolutions are equal if the base and the size are equals
         * Scale is a computed value (size / base)
         * Aspect ratio (inherited from parent Resolution class) is a computed value (size.x/size.y, width/height)
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
}