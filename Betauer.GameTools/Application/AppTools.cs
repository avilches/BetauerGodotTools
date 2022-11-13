using System;
using System.Threading.Tasks;
using Betauer.Core.Nodes;
using Betauer.Tools.Logging;
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

        
        public static void ConfigureExceptionHandlers(Func<SceneTree> sceneTree) {
            
            TaskScheduler.UnobservedTaskException += (o, args) => {
                // This event logs errors in non-awaited Task, which is a weird case
                var e = args.Exception;
                LoggerFactory.GetLogger(o?.GetType() ?? typeof(AppTools))
                    .Error("TaskScheduler.UnobservedTaskException {0}", e);
                if (Project.FeatureFlags.IsTerminateOnExceptionEnabled()) {
                    sceneTree().QuitSafely(1);
                }
            };

            /*
            // TODO Godot 4
            AppDomain.CurrentDomain.UnhandledException += (o, args) => {
                // This event logs errors in _Input/_Ready or any other method called from Godot (async or non-async)
                // but it only works if runtime/unhandled_exception_policy is "0" (terminate),
                // so the quit is not really needed
                // If unhandled_exception_policy is "1" (LogError), the error is not logged neither this event is called
                var e = args.ExceptionObject;
                LoggerFactory.GetLogger(o?.GetType() ?? typeof(AppTools))
                    .Error("AppDomain.CurrentDomain.UnhandledException {0}", e);
            };
            
            GD.UnhandledException += (o, args) => {
                var e = args.Exception;
                LoggerFactory.GetLogger(o?.GetType() ?? typeof(AppTools))
                    .Error("GD.UnhandledException {0}", e);
            };
            */
        }

    }
}