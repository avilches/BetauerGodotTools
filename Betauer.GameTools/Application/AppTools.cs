using Godot;

namespace Betauer.Application {
    public static partial class AppTools {
        public static string GetUserFolder() => FeatureFlags.IsExported()
            ? OS.GetUserDataDir()
            : System.IO.Directory.GetCurrentDirectory();

        public static string GetUserFile(string file) =>
            System.IO.Path.Combine(GetUserFolder(), System.IO.Path.GetFileName(file));
    }
}