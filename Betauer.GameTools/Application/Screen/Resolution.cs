using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Application.Screen {
    public static class AspectRatios {
        public static List<AspectRatio> All = new List<AspectRatio>();
        public static List<AspectRatio> Landscapes = new List<AspectRatio>();

        public static AspectRatio Ratio1_1 = AddToAll(new AspectRatio(1, 1)); // 1
        public static AspectRatio Ratio4_1 = AddToAll(new AspectRatio(4, 1)); // 4
        public static AspectRatio Ratio5_4 = AddToAll(new AspectRatio(5, 4)); // 1.125
        public static AspectRatio Ratio4_3 = AddToAll(new AspectRatio(4, 3)); // 1.3333333
        public static AspectRatio Ratio3_2 = AddToAll(new AspectRatio(3, 2)); // 1.5
        public static AspectRatio Ratio5_3 = AddToAll(new AspectRatio(5, 3)); // 1.6666666

        // 1.5 aprox -> 2560/1700 = 1,50588235 = 0,00588235 threshold
        public static AspectRatio Ratio16_10 = AddToAll(Landscape(new AspectRatio(16, 10))); // 1.6

        // 1.7777 aprox -> 1366/768 = 1,77864583 = 0,00086813 threshold
        public static AspectRatio Ratio16_9 = AddToAll(Landscape(new AspectRatio(16, 9)));

        // 1.8888 aprox -> 4096/2160 = 1,8962963 = 0,0074 threshold
        public static AspectRatio Ratio17_9 = AddToAll(new AspectRatio(17, 9));

        // 2.3333 aprox -> 2560/1080 = 2,37037037 = 0,03703704 threshold
        public static AspectRatio Ratio21_9 = AddToAll(Landscape(new AspectRatio(21, 9)));
        // 2.4 aprox -> 3440/1440 = 2,38888889 = 0,011 threshold
        public static AspectRatio Ratio12_5 = AddToAll(Landscape(new AspectRatio(12, 5)));

        private static AspectRatio AddToAll(AspectRatio ratio) {
            All.Add(ratio);
            return ratio;
        }

        private static AspectRatio Landscape(AspectRatio ratio) {
            Landscapes.Add(ratio);
            return ratio;
        }

        public static AspectRatio? Get(Resolution resolution) => Get(resolution.Size);
        public static AspectRatio? Get(Vector2 resolution) => Get(resolution.x / resolution.y);
        public static AspectRatio? Get(int x, int y) => Get(x / (float)y);
        public static AspectRatio? Get(float ratio) => All.Find(aspect => aspect.Matches(ratio));
    }

    public static class Resolutions {
        private static readonly List<Resolution> _all = new List<Resolution>();

        // 16:9 https://en.wikipedia.org/wiki/16:9_aspect_ratio
        public static Resolution FULLHD_DIV6 = AddToAll(new Resolution(320, 180)); // 1920x1080 / 6
        public static Resolution FULLHD_DIV4 = AddToAll(new Resolution(480, 270)); // 1920x1080 / 4
        public static Resolution FULLHD_DIV2 = AddToAll(new Resolution(960, 540)); // 1920x1080 / 2
        public static Resolution FULLHD = AddToAll(new Resolution(1920, 1080));
        public static Resolution FULLHD_2K = AddToAll(new Resolution(2560, 1440)); // 1920x1080 * 1.33 aka "2K"
        public static Resolution FULLHD_4K = AddToAll(new Resolution(3840, 2160)); // 1920x1080 * 2 // aka 4K

        public static Resolution FULLHD_DIV1_875 = AddToAll(new Resolution(1024, 576)); // 1920x1080 / 1.875
        public static Resolution FULLHD_DIV1_5 = AddToAll(new Resolution(1280, 720)); // 1920x1080 / 1.5
        public static Resolution FULLHD_DIV1_2 = AddToAll(new Resolution(1600, 900)); // 1920x1080 / 1.2
        public static Resolution FULLHD_DIV3 = AddToAll(new Resolution(640, 360)); // 1920x1080 / 3

        // 16:10 https://en.wikipedia.org/wiki/16:10_aspect_ratio
        public static Resolution R1610SMALL1 = AddToAll(new Resolution(640, 400));
        public static Resolution R1610SMALL2 = AddToAll(new Resolution(960, 600));
        public static Resolution WXGA = AddToAll(new Resolution(1280, 800));
        public static Resolution WXGAplus = AddToAll(new Resolution(1440, 900));
        public static Resolution WSXGAplus = AddToAll(new Resolution(1680, 1050));
        public static Resolution WUXGA = AddToAll(new Resolution(1920, 1200));
        public static Resolution WQXGA = AddToAll(new Resolution(2560, 1600));
        public static Resolution WQUXGA = AddToAll(new Resolution(3840, 2400));

        // 21:9 https://en.wikipedia.org/wiki/21:9_aspect_ratio
        // 64:27	2.370
        public static Resolution UWDIE237_0 = AddToAll(new Resolution(2560, 1080));
        public static Resolution UWDIE237_1 = AddToAll(new Resolution(5120, 2160));
        public static Resolution UWDIE237_2 = AddToAll(new Resolution(7680, 3240));
        public static Resolution UWDIE237_3 = AddToAll(new Resolution(10240, 4320));

        // 43:18	2.38
        public static Resolution UWDI238_0 = AddToAll(new Resolution(3440, 1440));
        public static Resolution UWDI238_1 = AddToAll(new Resolution(5160, 2160));
        public static Resolution UWDI238_2 = AddToAll(new Resolution(6880, 2880));

        //	12:5	2.4
        public static Resolution UWDIE24_0 = AddToAll(new Resolution(1920, 800));
        public static Resolution UWDIE24_1 = AddToAll(new Resolution(2880, 1200));
        public static Resolution UWDIE24_2 = AddToAll(new Resolution(3840, 1600));
        public static Resolution UWDIE24_3 = AddToAll(new Resolution(4320, 1800));
        public static Resolution UWDIE24_4 = AddToAll(new Resolution(5760, 2400));
        public static Resolution UWDIE24_5 = AddToAll(new Resolution(7680, 3200));
        public static Resolution UWDIE24_6 = AddToAll(new Resolution(8640, 3600));

        private static Resolution AddToAll(Resolution resolution) {
            _all.Add(resolution);
            _all.Sort(Resolution.Comparison);
            return resolution;
        }

        public static List<Resolution> All(params AspectRatio[] aspectRatios) =>
            aspectRatios.Length switch {
                0 => new List<Resolution>(_all),
                1 => _all.FindAll(aspectRatios[0].Matches),
                _ => _all.FindAll(resolution => aspectRatios.Any(ratio => ratio.Matches(resolution)))
            };
    }

    public class AspectRatio {
        internal const float Tolerance = 0.05f;

        public readonly string Name;
        public readonly float Ratio;

        public AspectRatio(int width, int height, string? name = null) {
            Name = name ?? width + ":" + height;
            Ratio = width / (float)height;
        }

        public AspectRatio(Vector2 resolution, string? name = null) : this((int)resolution.x, (int)resolution.y, name) {
        }

        public bool Matches(Resolution resolution) => Matches(resolution.Size);
        public bool Matches(Vector2 resolution) => Matches(resolution.x / resolution.y);
        public bool Matches(int x, int y) => Matches(x / (float)y);
        public bool Matches(float ratio) => Math.Abs(ratio - Ratio) < Tolerance;
    }

    public class Resolution : IEquatable<Resolution> {
        public static readonly Comparison<Resolution> Comparison = (left, right) =>
            left.x * left.y > right.x * right.y ? 1 : -1;

        /**
         * Returns how many times can be multiplied the baseResolution without create a resolution bigger than maxSize
         */
        public static int CalculateMaxScale(Vector2 maxSize, Vector2 @base) {
            return (int)Mathf.Max(
                Mathf.Floor(Mathf.Min(maxSize.x / @base.x, maxSize.y / @base.y)), 1);
        }

        public readonly Vector2 Size;
        public readonly AspectRatio AspectRatio;

        public Resolution(Vector2 size) {
            Size = size;
            AspectRatio = AspectRatios.Get(this) ?? new AspectRatio(size);
        }

        public Resolution(int x, int y) : this(new Vector2(x, y)) {
        }

        public int x => (int)Size.x;
        public int y => (int)Size.y;

        public override string ToString() {
            return $"{AspectRatio.Name} {x}x{y}";
        }

        public bool Equals(Resolution other) => Size.Equals(other.Size);
        public override bool Equals(object obj) => obj is Resolution other && Equals(other);
        public static bool operator ==(Resolution left, Resolution right) => left.Equals(right);
        public static bool operator !=(Resolution left, Resolution right) => !left.Equals(right);
    }

    public class ScaledResolution : Resolution {
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

        private static bool IsInteger(float x) => Math.Abs(x - Math.Floor(x)) < 0.00001f;

        // Please check IsPixelPerfectScale before to use this value!!
        public int GetPixelPerfectScale() => (int)Math.Floor(Scale.x);
    }
}