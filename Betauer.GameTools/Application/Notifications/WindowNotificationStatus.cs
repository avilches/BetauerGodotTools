namespace Betauer.Application.Notifications;

public class WindowNotificationStatus {
    public bool IsMouseInsideScreen { get; internal set; } = false;
    public bool IsWindowFocused { get; internal set; } = true;
    public bool IsApplicationFocused { get; internal set; } = true;

    public WindowNotificationStatus(NotificationsHandler notificationsHandler) {
        notificationsHandler.OnWmMouseEnter += () => IsMouseInsideScreen = true;
        notificationsHandler.OnWmMouseExit += () => IsMouseInsideScreen = false;
        notificationsHandler.OnWmWindowFocusIn += () => IsWindowFocused = true;
        notificationsHandler.OnWmWindowFocusOut += () => IsWindowFocused = false;
        notificationsHandler.OnApplicationFocusIn += () => IsApplicationFocused = true;
        notificationsHandler.OnApplicationFocusOut += () => IsApplicationFocused = false;
    }
}