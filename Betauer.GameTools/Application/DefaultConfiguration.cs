using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application;

public class DefaultConfiguration {
    private readonly SceneTree _sceneTree;

    public DefaultConfiguration(Node owner) {
        _sceneTree = owner.GetTree();
        DefaultNodeHandler.Configure(owner);
        DefaultNotificationsHandler.Configure(owner);
        DefaultNotificationsHandler.Instance.OnWmCloseRequest += () => LoggerFactory.SetAutoFlush(true);
    }

    [Service] public SceneTree SceneTree => _sceneTree;
    [Service] public NotificationsHandler NotificationsHandler => DefaultNotificationsHandler.Instance;
    [Service] public NodeHandler NodeHandler => DefaultNodeHandler.Instance;
    [Service] public DebugOverlayManager DebugOverlayManager => new();
    [Service] public InputActionsContainer InputActionsContainer => new();

    [PostInject]
    private void Configure() {
        InputActionsContainer.AddConsoleCommands();
    }
}