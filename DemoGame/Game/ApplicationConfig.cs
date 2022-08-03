using System.Collections.Generic;
using System.Linq;
using Betauer.Application;
using Betauer.Application.Screen;
using Godot;

namespace Veronenger.Game {
    public static class ApplicationConfig {
        public class UserSettings : IUserSettings {
            public bool PixelPerfect => true;
            public bool Fullscreen => true;
            public bool VSync => true;
            public bool Borderless => false;
            public Resolution WindowedResolution { get; } = Configuration.BaseResolution;
            public string SettingsPathFile => AppTools.GetUserFile("settings.ini");
        }


        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV3,
            SceneTree.StretchMode.Mode2d,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }
}