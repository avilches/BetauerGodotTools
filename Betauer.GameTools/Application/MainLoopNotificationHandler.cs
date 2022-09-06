using System;
using Godot;

namespace Betauer.Application {
    public class MainLoopNotificationHandler {
        public event Action OnWmMouseEnter;
        public event Action OnWmMouseExit;
        public event Action OnWmFocusIn;
        public event Action OnWmFocusOut;
        public event Action OnWmQuitRequest;
        public event Action OnWmGoBackRequest;
        public event Action OnWmUnfocusRequest;
        public event Action OnOsMemoryWarning;
        public event Action OnTranslationChanged;
        public event Action OnWmAbout;
        public event Action OnCrash;
        public event Action OnOsImeUpdate;
        public event Action OnAppResumed;
        public event Action OnAppPaused;

        public void Execute(int what) {
            switch (what) {
                case MainLoop.NotificationWmMouseEnter:
                    OnWmMouseEnter?.Invoke();
                    break;
                case MainLoop.NotificationWmMouseExit:
                    OnWmMouseExit?.Invoke();
                    break;
                case MainLoop.NotificationWmFocusIn:
                    OnWmFocusIn?.Invoke();
                    break;
                case MainLoop.NotificationWmFocusOut:
                    OnWmFocusOut?.Invoke();
                    break;
                case MainLoop.NotificationWmQuitRequest:
                    OnWmQuitRequest?.Invoke();
                    break;
                case MainLoop.NotificationWmGoBackRequest:
                    OnWmGoBackRequest?.Invoke();
                    break;
                case MainLoop.NotificationWmUnfocusRequest:
                    OnWmUnfocusRequest?.Invoke();
                    break;
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
                case MainLoop.NotificationAppResumed:
                    OnAppResumed?.Invoke();
                    break;
                case MainLoop.NotificationAppPaused:
                    OnAppPaused?.Invoke();
                    break;
            }
        }
    }
}