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
            public string SettingsPathFile => GetUserFolder("settings.ini");
        }

        public static bool IsExported() => OS.HasFeature("standalone");

        public static string GetUserFolder(string? file = null) {
            var folder = IsExported() ? OS.GetUserDataDir() : System.IO.Directory.GetCurrentDirectory();
            return file == null ? folder : System.IO.Path.Combine(folder, System.IO.Path.GetFileName(file));
        }

        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV3,
            SceneTree.StretchMode.Mode2d,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5));
    }
}