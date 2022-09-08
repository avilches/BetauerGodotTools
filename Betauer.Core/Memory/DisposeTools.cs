using Godot;

namespace Betauer.Memory {
    public static class DisposeTools {
        public static bool ShowMessageOnNewInstance = false;
        public static bool ShowWarningOnShutdownDispose = false;
        public static bool ShowMessageOnDispose = false;

        internal static void LogNewInstance(object o) {
#if DEBUG
            if (ShowMessageOnNewInstance) GD.Print($"New instance: {o.GetType().Name}@{o.GetHashCode():x8}");
#endif
        }

        internal static void LogDispose(bool disposing, object o) {
#if DEBUG
            if (disposing) {
                if (ShowMessageOnDispose) GD.Print($"Dispose(false): {o.GetType().Name}@{o.GetHashCode():x8}");
            } else {
                if (ShowWarningOnShutdownDispose) GD.PushWarning($"Shutdown Dispose(true): {o.GetType().Name}@{o.GetHashCode():x8}");
            }
#endif
        }
    }
}