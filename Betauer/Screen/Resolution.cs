using Godot;

namespace Betauer.Screen {
    public static class Resolution {
        // 16:9 https://en.wikipedia.org/wiki/16:9_aspect_ratio
        public static Vector2 FULLHD_DIV6 = new Vector2(320, 180); // 1920x1080 / 6
        public static Vector2 FULLHD_DIV4 = new Vector2(480, 270); // 1920x1080 / 4
        public static Vector2 FULLHD_DIV2 = new Vector2(960, 540); // 1920x1080 / 2
        public static Vector2 FULLHD = new Vector2(1920, 1080);
        public static Vector2 FULLHD_2K = new Vector2(2560, 1440); // 1920x1080 * 1.33 aka "2K"
        public static Vector2 FULLHD_4K = new Vector2(3840, 2160); // 1920x1080 * 2 // aka 4K

        public static Vector2 FULLHD_DIV1_875 = new Vector2(1024, 576); // 1920x1080 / 1.875
        public static Vector2 FULLHD_DIV1_5 = new Vector2(1280, 720); // 1920x1080 / 1.5
        public static Vector2 FULLHD_DIV1_2 = new Vector2(1600, 900); // 1920x1080 / 1.2
        public static Vector2 FULLHD_DIV3 = new Vector2(640, 360); // 1920x1080 / 3

        // 16:10 https://en.wikipedia.org/wiki/16:10_aspect_ratio
        public static Vector2 R1610SMALL1 = new Vector2(640, 400);
        public static Vector2 R1610SMALL2 = new Vector2(960, 600);
        public static Vector2 WXGA = new Vector2(1280, 800);
        public static Vector2 WXGAplus = new Vector2(1440, 900);
        public static Vector2 WSXGAplus = new Vector2(1680, 1050);
        public static Vector2 WUXGA = new Vector2(1920, 1200);
        public static Vector2 WQXGA = new Vector2(2560, 1600);
        public static Vector2 WQUXGA = new Vector2(3840, 2400);

        // 21:9 https://en.wikipedia.org/wiki/21:9_aspect_ratio
        // 64:27	2.370
        public static Vector2 UWDIE237_0 = new Vector2(2560, 1080);

        // public static Vector2 UWDIE237_1 = new Vector2(5120, 2160);
        // public static Vector2 UWDIE237_2 = new Vector2(7680, 3240);
        // public static Vector2 UWDIE237_3 = new Vector2(10240, 4320);
        // 43:18	2.38
        public static Vector2 UWDI238_0 = new Vector2(3440, 1440);

        // public static Vector2 UWDI238_1 = new Vector2(5160, 2160);
        // public static Vector2 UWDI238_2 = new Vector2(6880, 2880);
        //	12:5	2.4
        public static Vector2 UWDIE24_0 = new Vector2(1920, 800);
        public static Vector2 UWDIE24_1 = new Vector2(2880, 1200);
        public static Vector2 UWDIE24_2 = new Vector2(3840, 1600);

        public static Vector2 UWDIE24_3 = new Vector2(4320, 1800);
        // public static Vector2 UWDIE24_4 = new Vector2(5760, 2400);
        // public static Vector2 UWDIE24_5 = new Vector2(7680, 3200);
        // public static Vector2 UWDIE24_6 = new Vector2(8640, 3600);
    }

    public class ScreenConfiguration {
        public Vector2 BaseResolution { get; }

        // disabled: while the framebuffer will be resized to match the game window, nothing will be upscaled or downscaled (this includes GUIs).
        // 2d: the framebuffer is still resized, but GUIs can be upscaled or downscaled. This can result in blurry or pixelated fonts.
        // viewport: the framebuffer is resized, but computed at the original size of the project. The whole rendering will be pixelated. You generally do not want this, unless it's part of the game style.
        public SceneTree.StretchMode StretchMode { get; }
        public SceneTree.StretchAspect StretchAspect { get; }
        public int StretchShrink { get; }

        public ScreenConfiguration(Vector2 baseResolution, SceneTree.StretchMode stretchMode,
            SceneTree.StretchAspect stretchAspect, int stretchShrink = 1) {
            BaseResolution = baseResolution;
            StretchMode = stretchMode;
            StretchAspect = stretchAspect;
            StretchShrink = stretchShrink;
        }
    }


}