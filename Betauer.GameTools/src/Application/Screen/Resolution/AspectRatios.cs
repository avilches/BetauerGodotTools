using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Screen.Resolution; 

public static class AspectRatios {
    public static readonly List<AspectRatio> Cache = new List<AspectRatio>();
    public static readonly List<AspectRatio> Others = new List<AspectRatio>();
    public static readonly List<AspectRatio> Landscapes = new List<AspectRatio>();

    public static AspectRatio Ratio1_1 = Other(Get(1, 1)); // 1
    public static AspectRatio Ratio4_1 = Other(Get(4, 1)); // 4
    public static AspectRatio Ratio5_4 = Other(Get(5, 4)); // 1.125
    public static AspectRatio Ratio4_3 = Other(Get(4, 3)); // 1.3333333
    public static AspectRatio Ratio3_2 = Other(Get(3, 2)); // 1.5
    public static AspectRatio Ratio5_3 = Other(Get(5, 3)); // 1.6666666

    // 1.5 aprox -> 2560/1700 = 1,50588235 = 0,00588235 threshold
    public static AspectRatio Ratio16_10 = Landscape(Get(16, 10)); // 1.6

    // 1.7777 aprox -> 1366/768 = 1,77864583 = 0,00086813 threshold
    public static AspectRatio Ratio16_9 = Landscape(Get(16, 9));

    // 1.8888 aprox -> 4096/2160 = 1,8962963 = 0,0074 threshold
    public static AspectRatio Ratio17_9 = Other(Get(17, 9));

    // 2.3333 aprox -> 2560/1080 = 2,37037037 = 0,03703704 threshold
    public static AspectRatio Ratio21_9 = Landscape(Get(21, 9));

    // 2.4 aprox -> 3440/1440 = 2,38888889 = 0,011 threshold
    public static AspectRatio Ratio12_5 = Landscape(Get(12, 5));

    private static AspectRatio Other(AspectRatio ratio) {
        Others.Add(ratio);
        return ratio;
    }

    private static AspectRatio Landscape(AspectRatio ratio) {
        Landscapes.Add(ratio);
        return ratio;
    }
    public static AspectRatio Get(Resolution resolution) => Get(resolution.Size);
    public static AspectRatio Get(Vector2 resolution) => Get((int)resolution.X, (int)resolution.Y);
    public static AspectRatio Get(int width, int height) {
        var ratio = width / (float)height;
        var aspectRatio = Cache.Find(aspect => aspect.Matches(ratio));
        if (aspectRatio == null) {
            aspectRatio = new AspectRatio(width, height);
            Cache.Add(aspectRatio);
        }
        return aspectRatio;
    }
}