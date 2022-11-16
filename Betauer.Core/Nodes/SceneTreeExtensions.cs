using System.Linq;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Core.Nodes {
    public static partial class SceneTreeExtensions {

        public static Node GetMainScene(this SceneTree sceneTree) {
            if (!ProjectSettings.HasSetting("application/run/main_scene")) return null;
            var mainScene = ProjectSettings.GetSetting("application/run/main_scene").AsString();
            return sceneTree.Root.GetChildren().FirstOrDefault(n => n.SceneFilePath == mainScene, null);
        }

        private static bool _quitRequested = false;

        public static void QuitSafely(this SceneTree sceneTree, int exitCode = -1) {
            if (_quitRequested) return;
            _quitRequested = true;
            sceneTree.AutoAcceptQuit = false;
            LoggerFactory.SetAutoFlush(true);
            sceneTree.Notification((int)Node.NotificationWmCloseRequest);
            sceneTree.Quit(exitCode);
        }
    }
}