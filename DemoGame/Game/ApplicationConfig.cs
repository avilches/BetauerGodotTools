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
        public static string GetVersion() => (string)ProjectSettings.GetSetting("application/config/version");
        
        public static string GetUserFolder(string? file = null) {
            var folder = IsExported() ? OS.GetUserDataDir() : System.IO.Directory.GetCurrentDirectory();
            return file == null ? folder : System.IO.Path.Combine(folder, System.IO.Path.GetFileName(file));
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

    public static class FeatureFlags {
        public static readonly Dictionary<string, string> Description = new Dictionary<string, string> {
            { "Android", "Running on Android" },
            { "HTML5", "Running on HTML5" },
            { "JavaScript", "JavaScript singleton is available" },
            { "OSX", "Running on macOS" },
            { "iOS", "Running on iOS" },
            { "UWP", "Running on UWP" },
            { "Windows", "Running on Windows" },
            { "X11", "Running on X11 (Linux/BSD desktop)" },
            { "Server", "Running on the headless server platform" },
            { "debug", "Running on a debug build (including the editor)" },
            { "release", "Running on a release build" },
            { "editor", "Running on an editor build" },
            { "standalone", "Running on a non-editor build" },
            { "64", "Running on a 64-bit build (any architecture)" },
            { "32", "Running on a 32-bit build (any architecture)" },
            { "x86_64", "Running on a 64-bit x86 build" },
            { "x86", "Running on a 32-bit x86 build" },
            { "arm64", "Running on a 64-bit ARM build" },
            { "arm", "Running on a 32-bit ARM build" },
            { "mobile", "Host OS is a mobile platform" },
            { "pc", "Host OS is a PC platform (desktop/laptop)" },
            { "web", "Host OS is a Web browser" },
            { "etc", "Textures using ETC1 compression are supported" },
            { "etc2", "Textures using ETC2 compression are supported" },
            { "s3tc", "Textures using S3TC (DXT/BC) compression are supported" },
            { "pvrtc", "Textures using PVRTC compression are supported" },
        };

        public static ICollection<string> GetActiveList() {
            return GetActive().Keys;
        }

        public static Dictionary<string, string> GetActive() {
            return Description
                .Where(val => OS.HasFeature(val.Key))
                .ToDictionary(val => val.Key, val => val.Value);;
        }

    }
}