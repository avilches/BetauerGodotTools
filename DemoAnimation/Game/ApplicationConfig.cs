using System.Collections.Generic;
using System.Linq;
using Betauer.Application;
using Betauer.Application.Screen;
using Godot;

namespace Veronenger.Game {
    public static class ApplicationConfig {
        public class UserSettings : IUserSettings {
            public bool PixelPerfect => true;
            public bool Fullscreen => false;
            public bool VSync => true;
            public bool Borderless => false;
            public Resolution WindowedResolution { get; } = Configuration.BaseResolution;
            public string SettingsPathFile => OS.GetUserDataDir() + "/settings.ini";
        }

        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV1_875,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }
}