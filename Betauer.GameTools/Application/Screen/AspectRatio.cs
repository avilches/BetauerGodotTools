using System;
using Godot;

namespace Betauer.Application.Screen {
    public class AspectRatio : IEquatable<AspectRatio> {
        internal const float Tolerance = 0.05f;

        public readonly string Name;
        public readonly float Ratio;
        public readonly int Width;
        public readonly int Height;

        public AspectRatio(int width, int height, string? name = null) {
            Width = width;
            Height = height;
            Name = name ?? width + ":" + height;
            Ratio = width / (float)height;
        }

        public AspectRatio(Vector2 resolution, string? name = null) : this((int)resolution.x, (int)resolution.y, name) {
        }

        public bool Matches(AspectRatio aspectRatio) => Matches(aspectRatio.Ratio);
        public bool Matches(Resolution resolution) => Matches(resolution.Size);
        public bool Matches(Vector2 resolution) => Matches(resolution.x / resolution.y);
        public bool Matches(int x, int y) => Matches(x / (float)y);
        public bool Matches(float ratio) => Math.Abs(ratio - Ratio) < Tolerance;

        /**
         * Two AspectRatio are equal if their Width and Height are equal (Name is ignored)
         */
        public static bool operator ==(AspectRatio left, AspectRatio right) => left.Equals(right);
        public static bool operator !=(AspectRatio left, AspectRatio right) => !left.Equals(right);
        public override bool Equals(object obj) => obj is AspectRatio other && Equals(other);
        public bool Equals(AspectRatio other) => Width == other.Width && Height == other.Height;

        public override int GetHashCode() {
            unchecked {
                var hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Ratio.GetHashCode();
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                return hashCode;
            }
        }
    }
}