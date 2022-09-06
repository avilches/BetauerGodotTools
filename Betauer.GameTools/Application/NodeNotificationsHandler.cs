using System;
using Godot;

namespace Betauer.Application {
    public class NodeNotificationsHandler {

        public event Action OnEnterTree;
        public event Action OnExitTree;
        public event Action OnMovedInParent;
        public event Action OnReady;
        public event Action OnPaused;
        public event Action OnUnpaused;
        public event Action OnPhysicsProcess;
        public event Action OnProcess;
        public event Action OnParented;
        public event Action OnUnparented;
        public event Action OnInstanced;
        public event Action OnDragBegin;
        public event Action OnDragEnd;
        public event Action OnPathChanged;
        public event Action OnInternalProcess;
        public event Action OnInternalPhysicsProcess;
        public event Action OnPostEnterTree;
        public event Action OnResetPhysicsInterpolation;
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
        public event Action OnPostinitialize;
        public event Action OnPredelete;

        public void Execute(int what) {
            switch (what) {
                case Node.NotificationEnterTree:
                    OnEnterTree?.Invoke();
                    break;
                case Node.NotificationExitTree:
                    OnExitTree?.Invoke();
                    break;
                case Node.NotificationMovedInParent:
                    OnMovedInParent?.Invoke();
                    break;
                case Node.NotificationReady:
                    OnReady?.Invoke();
                    break;
                case Node.NotificationPaused:
                    OnPaused?.Invoke();
                    break;
                case Node.NotificationUnpaused:
                    OnUnpaused?.Invoke();
                    break;
                case Node.NotificationPhysicsProcess:
                    OnPhysicsProcess?.Invoke();
                    break;
                case Node.NotificationProcess:
                    OnProcess?.Invoke();
                    break;
                case Node.NotificationParented:
                    OnParented?.Invoke();
                    break;
                case Node.NotificationUnparented:
                    OnUnparented?.Invoke();
                    break;
                case Node.NotificationInstanced:
                    OnInstanced?.Invoke();
                    break;
                case Node.NotificationDragBegin:
                    OnDragBegin?.Invoke();
                    break;
                case Node.NotificationDragEnd:
                    OnDragEnd?.Invoke();
                    break;
                case Node.NotificationPathChanged:
                    OnPathChanged?.Invoke();
                    break;
                case Node.NotificationInternalProcess:
                    OnInternalProcess?.Invoke();
                    break;
                case Node.NotificationInternalPhysicsProcess:
                    OnInternalPhysicsProcess?.Invoke();
                    break;
                case Node.NotificationPostEnterTree:
                    OnPostEnterTree?.Invoke();
                    break;
                case Node.NotificationResetPhysicsInterpolation:
                    OnResetPhysicsInterpolation?.Invoke();
                    break;
                case Node.NotificationWmMouseEnter:
                    OnWmMouseEnter?.Invoke();
                    break;
                case Node.NotificationWmMouseExit:
                    OnWmMouseExit?.Invoke();
                    break;
                case Node.NotificationWmFocusIn:
                    OnWmFocusIn?.Invoke();
                    break;
                case Node.NotificationWmFocusOut:
                    OnWmFocusOut?.Invoke();
                    break;
                case Node.NotificationWmQuitRequest:
                    OnWmQuitRequest?.Invoke();
                    break;
                case Node.NotificationWmGoBackRequest:
                    OnWmGoBackRequest?.Invoke();
                    break;
                case Node.NotificationWmUnfocusRequest:
                    OnWmUnfocusRequest?.Invoke();
                    break;
                case Node.NotificationOsMemoryWarning:
                    OnOsMemoryWarning?.Invoke();
                    break;
                case Node.NotificationTranslationChanged:
                    OnTranslationChanged?.Invoke();
                    break;
                case Node.NotificationWmAbout:
                    OnWmAbout?.Invoke();
                    break;
                case Node.NotificationCrash:
                    OnCrash?.Invoke();
                    break;
                case Node.NotificationOsImeUpdate:
                    OnOsImeUpdate?.Invoke();
                    break;
                case Node.NotificationAppResumed:
                    OnAppResumed?.Invoke();
                    break;
                case Node.NotificationAppPaused:
                    OnAppPaused?.Invoke();
                    break;
                case Node.NotificationPostinitialize:
                    OnPostinitialize?.Invoke();
                    break;
                case Node.NotificationPredelete:
                    OnPredelete?.Invoke();
                    break;
            }
        }  

    }
}