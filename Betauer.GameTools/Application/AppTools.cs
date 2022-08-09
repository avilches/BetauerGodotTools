using Godot;

namespace Betauer.Application {
    public static partial class AppTools {
        public static string GetUserFolder() => FeatureFlags.IsExported()
            ? OS.GetUserDataDir()
            : System.IO.Directory.GetCurrentDirectory();

        public static string GetUserFile(string file) =>
            System.IO.Path.Combine(GetUserFolder(), System.IO.Path.GetFileName(file));
        

        public static T GetProjectSetting<T>(string key, T defaultValue) {
            var value = ProjectSettings.GetSetting("display/window/size/width");
            if (value is T r) return r;
            return defaultValue;
        }
        
        public static int GetWindowSizeWidth() => GetProjectSetting("display/window/size/width", 1024);
        public static int GetWindowSizeHeight() => GetProjectSetting("display/window/size/height", 600);
        public static bool GetWindowFullscreen() => GetProjectSetting("display/window/size/fullscreen", false);
        public static bool GetWindowBorderless() => GetProjectSetting("display/window/size/borderless", false);
        public static bool GetWindowVsync() => GetProjectSetting("display/window/vsync/use_vsync", true);

    }
}