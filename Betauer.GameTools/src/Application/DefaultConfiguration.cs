using Betauer.Application.Notifications;
using Betauer.DI.Attributes;
using Betauer.Nodes;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application;

public class DefaultConfiguration {
    public DefaultConfiguration() {
        DefaultNotificationsHandler.Instance.OnWMCloseRequest += () => LoggerFactory.SetAutoFlush(true);
    }

    [Singleton] public WindowNotificationStatus WindowNotificationStatus => new WindowNotificationStatus(DefaultNotificationsHandler.Instance);
    [Singleton] public SceneTree SceneTree => (SceneTree)Engine.GetMainLoop();
    [Singleton] public NotificationsHandler NotificationsHandler => DefaultNotificationsHandler.Instance;
    [Singleton] public NodeHandler NodeHandler => DefaultNodeHandler.Instance;

}