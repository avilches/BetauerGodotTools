using Betauer.Managers;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game {
    public static class ApplicationConfig {
        public class UserSettings : IUserSettings {
            public bool PixelPerfect => true;
            public bool Fullscreen => true;
            public bool VSync => true;
            public bool Borderless => false;
            public Resolution WindowedResolution { get; } = Configuration.BaseResolution;
        }

        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV3,
            SceneTree.StretchMode.Mode2d,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));

        public static readonly ScreenConfiguration AnimaDemoConfiguration = new ScreenConfiguration(
            Resolutions.FULLHD,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }
}