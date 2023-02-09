using System;
using Godot;

namespace Betauer.Application.Notifications; 

public partial class NotificationsHandler : Node {

    public event Action OnWMMouseEnter;
    public event Action OnWMMouseExit;
    public event Action OnWMWindowFocusIn;
    public event Action OnWMWindowFocusOut;
    public event Action OnWMCloseRequest;
    public event Action OnWMGoBackRequest;
    public event Action OnWMSizeChanged;
    public event Action OnWMDpiChange;
    public event Action OnVpMouseEnter;
    public event Action OnVpMouseExit;
    public event Action OnOsMemoryWarning;
    public event Action OnTranslationChanged;
    public event Action OnWMAbout;
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

    public override void _Notification(int what) {
        switch ((long)what) {
            case Node.NotificationWMMouseEnter: // 1002
                OnWMMouseEnter?.Invoke();
                break;
            case Node.NotificationWMMouseExit: // 1003
                OnWMMouseExit?.Invoke();
                break;
            case Node.NotificationWMWindowFocusIn: // 1004
                OnWMWindowFocusIn?.Invoke();
                break;
            case Node.NotificationWMWindowFocusOut: // 1005
                OnWMWindowFocusOut?.Invoke();
                break;
            case Node.NotificationWMCloseRequest: // 1006
                OnWMCloseRequest?.Invoke();
                break;
            case Node.NotificationWMGoBackRequest: // 1007
                OnWMGoBackRequest?.Invoke();
                break;
            case Node.NotificationWMSizeChanged: // 1008
                OnWMSizeChanged?.Invoke();
                break;
            case Node.NotificationWMDpiChange: // 1009
                OnWMDpiChange?.Invoke();
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
            case Node.NotificationWMAbout: // 2011
                OnWMAbout?.Invoke();
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
