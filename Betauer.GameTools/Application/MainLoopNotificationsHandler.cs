using System;
using Godot;

namespace Betauer.Application {
    public class MainLoopNotificationsHandler {

        public event Action OnOsMemoryWarning;
        public event Action OnTranslationChanged;
        public event Action OnWmAbout;
        public event Action OnCrash;
        public event Action OnOsImeUpdate;
        public event Action OnApplicationResumed;
        public event Action OnApplicationPaused;
        public event Action OnApplicationFocusIn;
        public event Action OnApplicationFocusOut;
        public event Action OnTextServerChanged;
        public event Action OnPostinitialize;
        public event Action OnPredelete;

        public void Execute(int what) {
            switch (what) {
                case MainLoop.NotificationOsMemoryWarning:
                    OnOsMemoryWarning?.Invoke();
                    break;
                case MainLoop.NotificationTranslationChanged:
                    OnTranslationChanged?.Invoke();
                    break;
                case MainLoop.NotificationWmAbout:
                    OnWmAbout?.Invoke();
                    break;
                case MainLoop.NotificationCrash:
                    OnCrash?.Invoke();
                    break;
                case MainLoop.NotificationOsImeUpdate:
                    OnOsImeUpdate?.Invoke();
                    break;
                case MainLoop.NotificationApplicationResumed:
                    OnApplicationResumed?.Invoke();
                    break;
                case MainLoop.NotificationApplicationPaused:
                    OnApplicationPaused?.Invoke();
                    break;
                case MainLoop.NotificationApplicationFocusIn:
                    OnApplicationFocusIn?.Invoke();
                    break;
                case MainLoop.NotificationApplicationFocusOut:
                    OnApplicationFocusOut?.Invoke();
                    break;
                case MainLoop.NotificationTextServerChanged:
                    OnTextServerChanged?.Invoke();
                    break;
                case MainLoop.NotificationPostinitialize:
                    OnPostinitialize?.Invoke();
                    break;
                case MainLoop.NotificationPredelete:
                    OnPredelete?.Invoke();
                    break;
            }
        }  

    }
}