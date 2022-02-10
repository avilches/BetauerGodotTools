using Betauer.Screen;
using Godot;

namespace Veronenger.Game {
    public static class ApplicationConfig {
        public class UserScreenSettings {
            // Default UserScreenSetting when the app starts for the first time
            public const bool DefaultPixelPerfect = true;
            public const bool DefaultFullscreen = true;
            public const bool DefaultVSync = true;
            public const bool DefaultBorderless = false;
            public static readonly Vector2 DefaultWindowedResolution = Vector2.Zero;


            public const string Filename = "video.cfg";
            public const string VideoSection = "Video";
            public const string PixelPerfectProperty = "PixelPerfect";
            public const string FullscreenProperty = "Fullscreen";
            public const string VSyncProperty = "VSync";
            public const string BorderlessProperty = "Borderless";
            public const string WindowedResolutionProperty = "WindowedResolution";
        }

        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV3,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));

        public static readonly ScreenConfiguration Configuration2 = new ScreenConfiguration(
            Resolutions.FULLHD,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));

    }



}