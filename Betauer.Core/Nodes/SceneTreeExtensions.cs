using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Thread = System.Threading.Thread;

namespace Betauer.Nodes {
    public static partial class SceneTreeExtensions {
        public static void QuitSafely(this SceneTree sceneTree, int exitCode = -1) {
            sceneTree.SetAutoAcceptQuit(false);
            sceneTree.Notification(MainLoop.NotificationWmQuitRequest);
            sceneTree.Quit(exitCode);
        }
    }
}