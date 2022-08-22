using System;
using Godot;

namespace Betauer.Application.Screen {
    public class AspectRatio {
        internal const float Tolerance = 0.05f;

        public readonly string Name;
        public readonly float Ratio;
        public readonly int Width;
        public readonly int Height;

        public AspectRatio(int width, int height) {
            Width = width;
            Height = height;
            Name = width + ":" + height;
            Ratio = width / (float)height;
        }

        private AspectRatio(Vector2 resolution) : this((int)resolution.x, (int)resolution.y) {
        }

        public bool Matches(AspectRatio aspectRatio) => Matches(aspectRatio.Ratio);
        public bool Matches(Resolution resolution) => Matches(resolution.Size);
        public bool Matches(Vector2 resolution) => Matches(resolution.x / resolution.y);
        public bool Matches(int x, int y) => Matches(x / (float)y);
        public bool Matches(float ratio) => Math.Abs(ratio - Ratio) < Tolerance;

        /**
         * Two AspectRatio are equal if their Width and Height are equal (Name is ignored)
         */
        public static bool operator ==(AspectRatio? left, AspectRatio? right) {
            if (ReferenceEquals(left, right)) return true; // equals or both null
            if (right is null || left is null) return false; // one of them is null
            return left.Equals(right); // not the same reference
        }

        public static bool operator !=(AspectRatio? left, AspectRatio? right) {
            if (ReferenceEquals(left, right)) return false; // equals or both null
            if (right is null || left is null) return true; // one of them is null
            return !left.Equals(right); // not the same reference
        }

        public override bool Equals(object? obj) => 
            obj is AspectRatio other && Equals(other);
        
        public bool Equals(AspectRatio? obj) => 
            ReferenceEquals(this, obj) ||
            (obj is { } && Ratio.Equals(obj.Ratio));
        
        public override int GetHashCode() => Ratio.GetHashCode();
    }
}