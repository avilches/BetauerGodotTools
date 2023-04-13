namespace Betauer.Application.Notifications;

public class WindowNotificationStatus {
    public bool IsMouseInsideScreen { get; internal set; } = false;
    public bool IsWindowFocused { get; internal set; } = true;
    public bool IsApplicationFocused { get; internal set; } = true;

    public WindowNotificationStatus(NotificationsHandler notificationsHandler) {
        notificationsHandler.OnWMMouseEnter += () => IsMouseInsideScreen = true;
        notificationsHandler.OnWMMouseExit += () => IsMouseInsideScreen = false;
        notificationsHandler.OnWMWindowFocusIn += () => IsWindowFocused = true;
        notificationsHandler.OnWMWindowFocusOut += () => IsWindowFocused = false;
        notificationsHandler.OnApplicationFocusIn += () => IsApplicationFocused = true;
        notificationsHandler.OnApplicationFocusOut += () => IsApplicationFocused = false;
    }
}