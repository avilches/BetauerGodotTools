using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Application {
    public static partial class AppTools {
        public static string GetUserFolder() => Project.FeatureFlags.IsExported()
            ? OS.GetUserDataDir()
            : System.IO.Directory.GetCurrentDirectory();

        public static string GetUserFile(string file) =>
            System.IO.Path.Combine(GetUserFolder(), System.IO.Path.GetFileName(file));
        

        public static T GetProjectSetting<T>(string key, T defaultValue) {
            var value = ProjectSettings.GetSetting(key);
            if (value is T r) return r;
            return defaultValue;
        }

        public static string GetProjectName() => GetProjectSetting("application/config/name", "(unknown)");
        public static string GetProjectVersion() => GetProjectSetting("application/config/version", "");
        
        public static int GetWindowSizeWidth() => GetProjectSetting("display/window/size/width", 1024);
        public static int GetWindowSizeHeight() => GetProjectSetting("display/window/size/height", 600);
        public static bool GetWindowFullscreen() => GetProjectSetting("display/window/size/fullscreen", false);
        public static bool GetWindowBorderless() => GetProjectSetting("display/window/size/borderless", false);
        public static bool GetWindowVsync() => GetProjectSetting("display/window/vsync/use_vsync", true);

        
        public static void ConfigureExceptionHandlers(SceneTree sceneTree) {
            TaskScheduler.UnobservedTaskException += (o, args) => {
                // This event logs errors in non-awaited Task. It needs
                var e = args.Exception;
                GD.PrintErr(
                    $"{StringTools.FastFormatDateTime(DateTime.Now)} [Error] TaskScheduler.UnobservedTaskException:\n{e}");
                if (Project.FeatureFlags.IsTerminateOnExceptionEnabled()) {
                    sceneTree.Notification(MainLoop.NotificationWmQuitRequest);
                }
            };
            AppDomain.CurrentDomain.UnhandledException += (o, args) => {
                // This event logs errors in _Input/_Ready or any other method called from Godot (async or non-async)
                // but it only works if runtime/unhandled_exception_policy is "0" (terminate),
                // so the quit is not really needed
                // If unhandled_exception_policy is "1" (LogError), the error is not logged neither this event is called
                var e = args.ExceptionObject;
                GD.PrintErr(
                    $"{StringTools.FastFormatDateTime(DateTime.Now)} [Error] AppDomain.CurrentDomain.UnhandledException:\n{e}");
            };
        }

    }
}