using Betauer.Application.Notifications;
using Betauer.DI.Attributes;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application;

public class DefaultConfiguration {
    private readonly SceneTree _sceneTree;
    private readonly WindowNotificationStatus _windowNotificationStatus;

    public DefaultConfiguration(SceneTree sceneTree) {
        _sceneTree = sceneTree;
        DefaultNotificationsHandler.Instance.OnWMCloseRequest += () => LoggerFactory.SetAutoFlush(true);
        _windowNotificationStatus = new WindowNotificationStatus(DefaultNotificationsHandler.Instance);
    }

    [Singleton] public WindowNotificationStatus WindowNotificationStatus => _windowNotificationStatus;
    [Singleton] public SceneTree SceneTree => _sceneTree;
    [Singleton] public NotificationsHandler NotificationsHandler => DefaultNotificationsHandler.Instance;
    [Singleton] public NodeHandler NodeHandler => DefaultNodeHandler.Instance;

}