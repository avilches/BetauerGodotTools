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
        public event Action OnSceneInstantiated;
        public event Action OnDragBegin;
        public event Action OnDragEnd;
        public event Action OnPathRenamed;
        public event Action OnInternalProcess;
        public event Action OnInternalPhysicsProcess;
        public event Action OnPostEnterTree;
        public event Action OnDisabled;
        public event Action OnEnabled;
        public event Action OnEditorPreSave;
        public event Action OnEditorPostSave;
        public event Action OnWmMouseEnter;
        public event Action OnWmMouseExit;
        public event Action OnWmWindowFocusIn;
        public event Action OnWmWindowFocusOut;
        public event Action OnWmCloseRequest;
        public event Action OnWmGoBackRequest;
        public event Action OnWmSizeChanged;
        public event Action OnWmDpiChange;
        public event Action OnVpMouseEnter;
        public event Action OnVpMouseExit;
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

        public void Execute(long what) {
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
                case Node.NotificationSceneInstantiated:
                    OnSceneInstantiated?.Invoke();
                    break;
                case Node.NotificationDragBegin:
                    OnDragBegin?.Invoke();
                    break;
                case Node.NotificationDragEnd:
                    OnDragEnd?.Invoke();
                    break;
                case Node.NotificationPathRenamed:
                    OnPathRenamed?.Invoke();
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
                case Node.NotificationDisabled:
                    OnDisabled?.Invoke();
                    break;
                case Node.NotificationEnabled:
                    OnEnabled?.Invoke();
                    break;
                case Node.NotificationEditorPreSave:
                    OnEditorPreSave?.Invoke();
                    break;
                case Node.NotificationEditorPostSave:
                    OnEditorPostSave?.Invoke();
                    break;
                case Node.NotificationWmMouseEnter:
                    OnWmMouseEnter?.Invoke();
                    break;
                case Node.NotificationWmMouseExit:
                    OnWmMouseExit?.Invoke();
                    break;
                case Node.NotificationWmWindowFocusIn:
                    OnWmWindowFocusIn?.Invoke();
                    break;
                case Node.NotificationWmWindowFocusOut:
                    OnWmWindowFocusOut?.Invoke();
                    break;
                case Node.NotificationWmCloseRequest:
                    OnWmCloseRequest?.Invoke();
                    break;
                case Node.NotificationWmGoBackRequest:
                    OnWmGoBackRequest?.Invoke();
                    break;
                case Node.NotificationWmSizeChanged:
                    OnWmSizeChanged?.Invoke();
                    break;
                case Node.NotificationWmDpiChange:
                    OnWmDpiChange?.Invoke();
                    break;
                case Node.NotificationVpMouseEnter:
                    OnVpMouseEnter?.Invoke();
                    break;
                case Node.NotificationVpMouseExit:
                    OnVpMouseExit?.Invoke();
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
                case Node.NotificationApplicationResumed:
                    OnApplicationResumed?.Invoke();
                    break;
                case Node.NotificationApplicationPaused:
                    OnApplicationPaused?.Invoke();
                    break;
                case Node.NotificationApplicationFocusIn:
                    OnApplicationFocusIn?.Invoke();
                    break;
                case Node.NotificationApplicationFocusOut:
                    OnApplicationFocusOut?.Invoke();
                    break;
                case Node.NotificationTextServerChanged:
                    OnTextServerChanged?.Invoke();
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