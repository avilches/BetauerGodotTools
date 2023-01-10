using System;
using Godot;

namespace Betauer.Application.Notifications; 

public partial class NotificationsHandler : Node {

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

    public override void _EnterTree() {
        ProcessMode = ProcessModeEnum.Always;
    }

    public void AddTo(Viewport viewport) {
        GetParent()?.RemoveChild(this);
        viewport.AddChild(this);
    }

    public override void _Notification(long what) {
        switch (what) {
            case Node.NotificationWmMouseEnter: // 1002
                OnWmMouseEnter?.Invoke();
                break;
            case Node.NotificationWmMouseExit: // 1003
                OnWmMouseExit?.Invoke();
                break;
            case Node.NotificationWmWindowFocusIn: // 1004
                OnWmWindowFocusIn?.Invoke();
                break;
            case Node.NotificationWmWindowFocusOut: // 1005
                OnWmWindowFocusOut?.Invoke();
                break;
            case Node.NotificationWmCloseRequest: // 1006
                OnWmCloseRequest?.Invoke();
                break;
            case Node.NotificationWmGoBackRequest: // 1007
                OnWmGoBackRequest?.Invoke();
                break;
            case Node.NotificationWmSizeChanged: // 1008
                OnWmSizeChanged?.Invoke();
                break;
            case Node.NotificationWmDpiChange: // 1009
                OnWmDpiChange?.Invoke();
                break;
            case Node.NotificationVpMouseEnter: // 1010
                OnVpMouseEnter?.Invoke();
                break;
            case Node.NotificationVpMouseExit: // 1011
                OnVpMouseExit?.Invoke();
                break;
            case Node.NotificationOsMemoryWarning: // 2009
                OnOsMemoryWarning?.Invoke();
                break;
            case Node.NotificationTranslationChanged: // 2010
                OnTranslationChanged?.Invoke();
                break;
            case Node.NotificationWmAbout: // 2011
                OnWmAbout?.Invoke();
                break;
            case Node.NotificationCrash: // 2012
                OnCrash?.Invoke();
                break;
            case Node.NotificationOsImeUpdate: // 2013
                OnOsImeUpdate?.Invoke();
                break;
            case Node.NotificationApplicationResumed: // 2014
                OnApplicationResumed?.Invoke();
                break;
            case Node.NotificationApplicationPaused: // 2015
                OnApplicationPaused?.Invoke();
                break;
            case Node.NotificationApplicationFocusIn: // 2016
                OnApplicationFocusIn?.Invoke();
                break;
            case Node.NotificationApplicationFocusOut: // 2017
                OnApplicationFocusOut?.Invoke();
                break;
            case Node.NotificationTextServerChanged: // 2018
                OnTextServerChanged?.Invoke();
                break;
        }
    }  

}
