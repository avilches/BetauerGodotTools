using System;
using Godot;

namespace Betauer.Nodes;

public static class NodeManagerExtensions {
    public static void OnProcess(this GodotObject watched, Action<double> action) {
        NodeManager.MainInstance.OnProcess += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnProcess -= action);
    }

    public static void OnPhysicsProcess(this GodotObject watched, Action<double> action) {
        NodeManager.MainInstance.OnPhysicsProcess += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnPhysicsProcess -= action);
    }

    public static void OnInput(this GodotObject watched, Action<InputEvent> action) {
        NodeManager.MainInstance.OnInput += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnInput -= action);
    }
    
    public static void OnUnhandledInput(this GodotObject watched, Action<InputEvent> action) {
        NodeManager.MainInstance.OnUnhandledInput += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnUnhandledInput -= action);
    }

    public static void OnUnhandledKeyInput(this GodotObject watched, Action<InputEvent> action) {
        NodeManager.MainInstance.OnUnhandledKeyInput += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnUnhandledKeyInput -= action);
    }

    public static void OnShortcutInput(this GodotObject watched, Action<InputEvent> action) {
        NodeManager.MainInstance.OnShortcutInput += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnShortcutInput -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the mouse enters the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public static void OnWMMouseEnter(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMMouseEnter += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMMouseEnter -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the mouse leaves the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public static void OnWMMouseExit(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMMouseExit += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMMouseExit -= action);
    }

    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is focused. This may be a change of focus between two windows of the same engine instance, or from the OS desktop or a third-party application to a window of the game (in which case <see cref="F:Godot.NotificationApplicationFocusIn" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is focused.</para>
    /// </summary>
    public static void OnWMWindowFocusIn(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMWindowFocusIn += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMWindowFocusIn -= action);
    }

    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is defocused. This may be a change of focus between two windows of the same engine instance, or from a window of the game to the OS desktop or a third-party application (in which case <see cref="F:Godot.NotificationApplicationFocusOut" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is defocused.</para>
    /// </summary>
    public static void OnWMWindowFocusOut(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMWindowFocusOut += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMWindowFocusOut -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a close request is sent (e.g. closing the window with a "Close" button or Alt + F4).</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public static void OnWMCloseRequest(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMCloseRequest += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMCloseRequest -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a go back request is sent (e.g. pressing the "Back" button on Android).</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public static void OnWMGoBackRequest(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMGoBackRequest += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMGoBackRequest -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the window is resized.</para>
    /// </summary>
    public static void OnWMSizeChanged(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMSizeChanged += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMSizeChanged -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the screen's DPI has been changed. NodeManager.MainInstance.Only implemented on macOS.</para>
    /// </summary>
    public static void OnWMDpiChange(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMDpiChange += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMDpiChange -= action);
    }

    /// <summary>
    /// <para>Event called when the mouse enters the viewport.</para>
    /// </summary>
    public static void OnVpMouseEnter(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnVpMouseEnter += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnVpMouseEnter -= action);
    }

    /// <summary>
    /// <para>Event called when the mouse leaves the viewport.</para>
    /// </summary>
    public static void OnVpMouseExit(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnVpMouseExit += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnVpMouseExit -= action);
    }
    
    /// <summary>
    /// <para>Event called from the OS when the application is exceeding its allocated memory.</para>
    /// <para>Specific to the iOS platform.</para>
    /// </summary>
    public static void OnOsMemoryWarning(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnOsMemoryWarning += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnOsMemoryWarning -= action);
    }

    /// <summary>
    /// <para>Event called when translations may have changed. Can be triggered by the user changing the locale. Can be used to respond to language changes, for example to change the UI strings on the fly. Useful when working with the built-in translation support, like <see cref="M:Godot.GodotObject.Tr(Godot.StringName,Godot.StringName)" />.</para>
    /// </summary>
    public static void OnTranslationChanged(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnTranslationChanged += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnTranslationChanged -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a request for "About" information is sent.</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public static void OnWMAbout(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnWMAbout += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnWMAbout -= action);
    }

    /// <summary>
    /// <para>Event called from Godot's crash handler when the engine is about to crash.</para>
    /// <para>Implemented on desktop platforms if the crash handler is enabled.</para>
    /// </summary>
    public static void OnCrash(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnCrash += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnCrash -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when an update of the Input Method Engine occurs (e.g. change of IME cursor position or composition string).</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public static void OnOsImeUpdate(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnOsImeUpdate += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnOsImeUpdate -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is resumed.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public static void OnApplicationResumed(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnApplicationResumed += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnApplicationResumed -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is paused.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public static void OnApplicationPaused(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnApplicationPaused += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnApplicationPaused -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is focused, i.e. when changing the focus from the OS desktop or a thirdparty application to any open window of the Godot instance.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public static void OnApplicationFocusIn(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnApplicationFocusIn += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnApplicationFocusIn -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is defocused, i.e. when changing the focus from any open window of the Godot instance to the OS desktop or a thirdparty application.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public static void OnApplicationFocusOut(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnApplicationFocusOut += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnApplicationFocusOut -= action);
    }

    /// <summary>
    /// <para>Event called when text server is changed.</para>
    /// </summary>
    public static void OnTextServerChanged(this GodotObject watched, Action action) {
        NodeManager.MainInstance.OnTextServerChanged += action;
        NodeManager.MainInstance.OnDestroy(watched, () => NodeManager.MainInstance.OnTextServerChanged -= action);
    }

}
