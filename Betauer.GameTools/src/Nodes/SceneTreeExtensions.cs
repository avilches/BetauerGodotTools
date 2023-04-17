using System.Linq;
using Betauer.Core.Signal;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Nodes; 

public static partial class SceneTreeExtensions {

    public static SignalAwaiter Delay(this SceneTree sceneTree, float delay) => sceneTree.CreateTimer(delay).AwaitTimeout();
    
    public static T GetMainScene<T>(this SceneTree sceneTree) where T : Node {
        if (!ProjectSettings.HasSetting("application/run/main_scene")) return null;
        var mainScene = ProjectSettings.GetSetting("application/run/main_scene").AsString();
        return sceneTree.Root.GetChildren().FirstOrDefault(n => n.SceneFilePath == mainScene, null) as T;
    }

    private static bool _quitRequested = false;

    public static void QuitSafely(this SceneTree sceneTree, int exitCode = -1) {
        if (_quitRequested) return;
        _quitRequested = true;
        sceneTree.AutoAcceptQuit = false;
        LoggerFactory.SetAutoFlush(true);
        sceneTree.Notification((int)Node.NotificationWMCloseRequest);
        sceneTree.Quit(exitCode);
    }
}