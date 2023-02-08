using Betauer.Application.Monitor;
using Betauer.Application.Notifications;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application;

public class DefaultConfiguration {
    private readonly SceneTree _sceneTree;
    private readonly WindowNotificationStatus _windowNotificationStatus;

    public DefaultConfiguration(SceneTree sceneTree) {
        _sceneTree = sceneTree;
        DefaultNotificationsHandler.Instance.OnWmCloseRequest += () => LoggerFactory.SetAutoFlush(true);
        _windowNotificationStatus = new WindowNotificationStatus(DefaultNotificationsHandler.Instance);
    }

    [Service] public WindowNotificationStatus WindowNotificationStatus => _windowNotificationStatus;
    [Service] public SceneTree SceneTree => _sceneTree;
    [Service] public NotificationsHandler NotificationsHandler => DefaultNotificationsHandler.Instance;
    [Service] public NodeHandler NodeHandler => DefaultNodeHandler.Instance;

}