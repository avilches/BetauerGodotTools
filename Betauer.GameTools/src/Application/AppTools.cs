using System;
using System.Threading.Tasks;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application; 

public static partial class AppTools {
    
    private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(AppTools));
    
    public static string GetUserFolder() => Project.FeatureFlags.IsExported()
        ? OS.GetUserDataDir()
        : System.IO.Directory.GetCurrentDirectory();

    public static string GetUserFile(string file) =>
        System.IO.Path.Combine(GetUserFolder(), System.IO.Path.GetFileName(file));
        

    public static T GetProjectSetting<[MustBeVariant] T>(string key, T defaultValue) {
        if (!ProjectSettings.HasSetting(key)) return defaultValue;
        var variant = ProjectSettings.GetSetting(key);
        return variant.As<T>();
    }

    public static void SetProjectSetting<T>(string key, T value) {
        var variant = Variant.From(value);
        ProjectSettings.SetSetting(key, variant);
    }

    public static string GetProjectName() => GetProjectSetting("application/config/name", "(unknown)");
    public static string GetProjectVersion() => GetProjectSetting("application/config/version", "");
        
    public static int GetWindowSizeWidth() => GetProjectSetting("display/window/size/width", 1024);
    public static int GetWindowSizeHeight() => GetProjectSetting("display/window/size/height", 600);
    public static bool GetWindowFullscreen() => GetProjectSetting("display/window/size/fullscreen", false);
    public static bool GetWindowBorderless() => GetProjectSetting("display/window/size/borderless", false);
    public static bool GetWindowVsync() => GetProjectSetting("display/window/vsync/use_vsync", true);


    public static void AddQuitGameOnException(int exitCode = 1) {
        AddExceptionHandler((o, exception) => ((SceneTree)Engine.GetMainLoop()).QuitSafely(exitCode));
    }

    public static void AddLogOnException() {
        Logging.UserExceptionReporter += e => LogException(null, "Godot.Logging.UserExceptionReporter", e.ToString());
        TaskScheduler.UnobservedTaskException += (o, args) => LogException(o, "TaskScheduler.UnobservedTaskException", args.Exception.ToString());
        AppDomain.CurrentDomain.UnhandledException += (o, args) => LogException(o, "UnhandledException", ((Exception)args.ExceptionObject).ToString());
    }

    public static void AddExceptionHandler(Action<object?, Exception>? exceptionHandler = null) {
        Logging.UserExceptionReporter += e => exceptionHandler?.Invoke(null, e);
        TaskScheduler.UnobservedTaskException += (o, args) => exceptionHandler?.Invoke(o, args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (o, args) => exceptionHandler?.Invoke(o, (Exception)args.ExceptionObject);
    }

    private static void LogException(object? caller, string from, string message) {
        try {
            var logger = caller?.GetType() != null ? LoggerFactory.GetLogger(caller.GetType()) : Logger;
            logger.Error("{0} | {1}", from, message);
        } catch (Exception e) {
            Console.WriteLine(e);
        }
    }
}