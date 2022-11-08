using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Nodes {
    public static partial class SceneTreeExtensions {
        private static bool _quitRequested = false;
        public static void QuitSafely(this SceneTree sceneTree, int exitCode = -1) {
            if (_quitRequested) return;
            _quitRequested = true;
            sceneTree.SetAutoAcceptQuit(false);
            sceneTree.Notification((int)Node.NotificationWmCloseRequest);
            LoggerFactory.SetAutoFlush(true);
            sceneTree.Quit(exitCode);
        }
    }
}