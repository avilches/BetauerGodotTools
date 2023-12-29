using System;
using Betauer.Core.Nodes;
using Godot;

namespace Betauer.Nodes;

public class NodeManager {
    private static readonly NodeManager _mainInstance = new();
    private static bool _isDefaultInstanceAdded = false;
    
    public static NodeManager MainInstance {
        get {
            if (!_isDefaultInstanceAdded) {
                var sceneTree = (SceneTree)Engine.GetMainLoop();
                if (sceneTree != null) {
                    _isDefaultInstanceAdded = true;
                    sceneTree.Root.AddChildDeferred(_mainInstance.Node);
                }
            }
            return _mainInstance;
        }
    }

    public bool IsMouseInsideScreen { get; private set; } = false;
    public bool IsWindowFocused { get; private set; } = true;
    public bool IsApplicationFocused { get; private set; } = true;

    public NotificationsNodeHandler Node { get; }

    public NodeManager() {
        Node = new NotificationsNodeHandler();
        Node.OnWMMouseEnter += () => IsMouseInsideScreen = true;
        Node.OnWMMouseExit += () => IsMouseInsideScreen = false;
        Node.OnWMWindowFocusIn += () => IsWindowFocused = true;
        Node.OnWMWindowFocusOut += () => IsWindowFocused = false;
        Node.OnApplicationFocusIn += () => IsApplicationFocused = true;
        Node.OnApplicationFocusOut += () => IsApplicationFocused = false;
    }

    public void OnDestroy(GodotObject node, Action removeAction) => Node.AddOnDestroy(node, removeAction);
    
    public void RemoveOnDestroy(GodotObject node, Action removeAction) => Node.RemoveOnDestroy(node, removeAction);

    public void OnProcess(IProcessHandler inputEvent) {
        Console.WriteLine("["+Engine.GetProcessFrames()+ "] NodeManager frame");
        Node.AddOnProcess(inputEvent);
    }

    public void CallDeferred(Action action) {
        Node.CallDeferred(action);
    }

    public void OnPhysicsProcess(IProcessHandler inputEvent) => Node.AddOnPhysicsProcess(inputEvent);

    public void OnInput(IInputEventHandler inputEvent) => Node.AddOnInput(inputEvent);

    public void OnUnhandledInput(IInputEventHandler inputEvent) => Node.AddOnUnhandledInput(inputEvent);

    public void OnShortcutInput(IInputEventHandler inputEvent) => Node.AddOnShortcutInput(inputEvent);

    public void OnUnhandledKeyInput(IInputEventHandler inputEvent) => Node.AddOnUnhandledKeyInput(inputEvent);

    public string GetStateAsString() => Node.GetStateAsString();
  
    /*
    /// <summary>
    /// <para>Event called when the object is initialized, before its script is attached. Used internally.</para>
    /// </summary>
    public void OnPostinitialize(Node? node, Action action) {
        Node.OnPostinitialize += action;
        OnDestroy(node, () => Node.OnPostinitialize -= action);
    }

    /// <summary>
    /// <para>Event called when the object is about to be deleted. Can act as the deconstructor of some programming languages.</para>
    /// </summary>
    public void OnPredelete(Node? node, Action action) {
        Node.OnPredelete += action;
        OnDestroy(node, () => Node.OnPredelete -= action);
    }

    /// <summary>
    /// <para>Event called when the node is paused.</para>
    /// </summary>
    public void OnPaused(Node? node, Action action) {
        Node.OnPaused += action;
        OnDestroy(node, () => Node.OnPaused -= action);
    }

    /// <summary>
    /// <para>Event called when the node is unpaused.</para>
    /// </summary>
    public void OnUnpaused(Node? node, Action action) {
        Node.OnUnpaused += action;
        OnDestroy(node, () => Node.OnUnpaused -= action);
    }

    /// <summary>
    /// <para>Event called every frame when the physics process flag is set (see <see cref="M:Godot.Node.SetPhysicsProcess(System.Boolean)" />).</para>
    /// </summary>
    public void OnParented(Node? node, Action action) {
        Node.OnParented += action;
        OnDestroy(node, () => Node.OnParented -= action);
    }

    /// <summary>
    /// <para>Event called when a node is unparented (parent removed it from the list of children).</para>
    /// </summary>
    public void OnUnparented(Node? node, Action action) {
        Node.OnUnparented += action;
        OnDestroy(node, () => Node.OnUnparented -= action);
    }

    /// <summary>
    /// <para>Event called by scene owner when its scene is instantiated.</para>
    /// </summary>
    public void OnSceneInstantiated(Node? node, Action action) {
        Node.OnSceneInstantiated += action;
        OnDestroy(node, () => Node.OnSceneInstantiated -= action);
    }

    /// <summary>
    /// <para>Event called when a drag operation begins. All nodes receive this notification, not only the dragged one.</para>
    /// <para>Can be triggered either by dragging a <see cref="T:Godot.Control" /> that provides drag data (see <see cref="M:Godot.Control._GetDragData(Godot.Vector2)" />) or using <see cref="M:Godot.Control.ForceDrag(Godot.Variant,Godot.Control)" />.</para>
    /// <para>Use <see cref="M:Godot.Viewport.GuiGetDragData" /> to get the dragged data.</para>
    /// </summary>
    public void OnDragBegin(Node? node, Action action) {
        Node.OnDragBegin += action;
        OnDestroy(node, () => Node.OnDragBegin -= action);
    }

    /// <summary>
    /// <para>Event called when a drag operation ends.</para>
    /// <para>Use <see cref="M:Godot.Viewport.GuiIsDragSuccessful" /> to check if the drag succeeded.</para>
    /// </summary>
    public void OnDragEnd(Node? node, Action action) {
        Node.OnDragEnd += action;
        OnDestroy(node, () => Node.OnDragEnd -= action);
    }

    /// <summary>
    /// <para>Event called when the node's name or one of its parents' name is changed. This notification is <i>not</i> received when the node is removed from the scene tree to be added to another parent later on.</para>
    /// </summary>
    public void OnPathRenamed(Node? node, Action action) {
        Node.OnPathRenamed += action;
        OnDestroy(node, () => Node.OnPathRenamed -= action);
    }

    /// <summary>
    /// <para>Event called when the list of children is changed. This happens when child nodes are added, moved or removed.</para>
    /// </summary>
    public void OnChildOrderChanged(Node? node, Action action) {
        Node.OnChildOrderChanged += action;
        OnDestroy(node, () => Node.OnChildOrderChanged -= action);
    }

    /// <summary>
    /// <para>Event called every frame when the internal process flag is set (see <see cref="M:Godot.Node.SetProcessInternal(System.Boolean)" />).</para>
    /// </summary>
    public void OnInternalProcess(Node? node, Action action) {
        Node.OnInternalProcess += action;
        OnDestroy(node, () => Node.OnInternalProcess -= action);
    }

    /// <summary>
    /// <para>Event called every frame when the internal physics process flag is set (see <see cref="M:Godot.Node.SetPhysicsProcessInternal(System.Boolean)" />).</para>
    /// </summary>
    public void OnInternalPhysicsProcess(Node? node, Action action) {
        Node.OnInternalPhysicsProcess += action;
        OnDestroy(node, () => Node.OnInternalPhysicsProcess -= action);
    }

    /// <summary>
    /// <para>Event called when the node is ready, just before <see cref="F:Godot.Node.NotificationReady" /> is received. Unlike the latter, it's sent every time the node enters tree, instead of only once.</para>
    /// </summary>
    public void OnPostEnterTree(Node? node, Action action) {
        Node.OnPostEnterTree += action;
        OnDestroy(node, () => Node.OnPostEnterTree -= action);
    }

    /// <summary>
    /// <para>Event called when the node is disabled. See <see cref="F:Godot.Node.ProcessModeEnum.Disabled" />.</para>
    /// </summary>
    public void OnDisabled(Node? node, Action action) {
        Node.OnDisabled += action;
        OnDestroy(node, () => Node.OnDisabled -= action);
    }

    /// <summary>
    /// <para>Event called when the node is enabled again after being disabled. See <see cref="F:Godot.Node.ProcessModeEnum.Disabled" />.</para>
    /// </summary>
    public void OnEnabled(Node? node, Action action) {
        Node.OnEnabled += action;
        OnDestroy(node, () => Node.OnEnabled -= action);
    }

    /// <summary>
    /// <para>Event called right before the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.</para>
    /// </summary>
    public void OnEditorPreSave(Node? node, Action action) {
        Node.OnEditorPreSave += action;
        OnDestroy(node, () => Node.OnEditorPreSave -= action);
    }

    /// <summary>
    /// <para>Event called right after the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.</para>
    /// </summary>
    public void OnEditorPostSave(Node? node, Action action) {
        Node.OnEditorPostSave += action;
        OnDestroy(node, () => Node.OnEditorPostSave -= action);
    }
*/
    /// <summary>
    /// <para>Event called from the OS when the mouse enters the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public void OnWMMouseEnter(Node? node, Action action) {
        Node.OnWMMouseEnter += action;
        OnDestroy(node, () => Node.OnWMMouseEnter -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the mouse leaves the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public void OnWMMouseExit(Node? node, Action action) {
        Node.OnWMMouseExit += action;
        OnDestroy(node, () => Node.OnWMMouseExit -= action);
    }

    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is focused. This may be a change of focus between two windows of the same engine instance, or from the OS desktop or a third-party application to a window of the game (in which case <see cref="F:Godot.Node.NotificationApplicationFocusIn" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is focused.</para>
    /// </summary>
    public void OnWMWindowFocusIn(Node? node, Action action) {
        Node.OnWMWindowFocusIn += action;
        OnDestroy(node, () => Node.OnWMWindowFocusIn -= action);
    }

    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is defocused. This may be a change of focus between two windows of the same engine instance, or from a window of the game to the OS desktop or a third-party application (in which case <see cref="F:Godot.Node.NotificationApplicationFocusOut" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is defocused.</para>
    /// </summary>
    public void OnWMWindowFocusOut(Node? node, Action action) {
        Node.OnWMWindowFocusOut += action;
        OnDestroy(node, () => Node.OnWMWindowFocusOut -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a close request is sent (e.g. closing the window with a "Close" button or Alt + F4).</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public void OnWMCloseRequest(Node? node, Action action) {
        Node.OnWMCloseRequest += action;
        OnDestroy(node, () => Node.OnWMCloseRequest -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a go back request is sent (e.g. pressing the "Back" button on Android).</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public void OnWMGoBackRequest(Node? node, Action action) {
        Node.OnWMGoBackRequest += action;
        OnDestroy(node, () => Node.OnWMGoBackRequest -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the window is resized.</para>
    /// </summary>
    public void OnWMSizeChanged(Node? node, Action action) {
        Node.OnWMSizeChanged += action;
        OnDestroy(node, () => Node.OnWMSizeChanged -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the screen's DPI has been changed. Only implemented on macOS.</para>
    /// </summary>
    public void OnWMDpiChange(Node? node, Action action) {
        Node.OnWMDpiChange += action;
        OnDestroy(node, () => Node.OnWMDpiChange -= action);
    }

    /*
    /// <summary>
    /// <para>Event called when the mouse enters the viewport.</para>
    /// </summary>
    public void OnVpMouseEnter(Node? node, Action action) {
        Node.OnVpMouseEnter += action;
        OnDestroy(node, () => Node.OnVpMouseEnter -= action);
    }

    /// <summary>
    /// <para>Event called when the mouse leaves the viewport.</para>
    /// </summary>
    public void OnVpMouseExit(Node? node, Action action) {
        Node.OnVpMouseExit += action;
        OnDestroy(node, () => Node.OnVpMouseExit -= action);
    }
    */
    
    /// <summary>
    /// <para>Event called from the OS when the application is exceeding its allocated memory.</para>
    /// <para>Specific to the iOS platform.</para>
    /// </summary>
    public void OnOsMemoryWarning(Node? node, Action action) {
        Node.OnOsMemoryWarning += action;
        OnDestroy(node, () => Node.OnOsMemoryWarning -= action);
    }

    /// <summary>
    /// <para>Event called when translations may have changed. Can be triggered by the user changing the locale. Can be used to respond to language changes, for example to change the UI strings on the fly. Useful when working with the built-in translation support, like <see cref="M:Godot.GodotObject.Tr(Godot.StringName,Godot.StringName)" />.</para>
    /// </summary>
    public void OnTranslationChanged(Node? node, Action action) {
        Node.OnTranslationChanged += action;
        OnDestroy(node, () => Node.OnTranslationChanged -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a request for "About" information is sent.</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public void OnWMAbout(Node? node, Action action) {
        Node.OnWMAbout += action;
        OnDestroy(node, () => Node.OnWMAbout -= action);
    }

    /// <summary>
    /// <para>Event called from Godot's crash handler when the engine is about to crash.</para>
    /// <para>Implemented on desktop platforms if the crash handler is enabled.</para>
    /// </summary>
    public void OnCrash(Node? node, Action action) {
        Node.OnCrash += action;
        OnDestroy(node, () => Node.OnCrash -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when an update of the Input Method Engine occurs (e.g. change of IME cursor position or composition string).</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public void OnOsImeUpdate(Node? node, Action action) {
        Node.OnOsImeUpdate += action;
        OnDestroy(node, () => Node.OnOsImeUpdate -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is resumed.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public void OnApplicationResumed(Node? node, Action action) {
        Node.OnApplicationResumed += action;
        OnDestroy(node, () => Node.OnApplicationResumed -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is paused.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public void OnApplicationPaused(Node? node, Action action) {
        Node.OnApplicationPaused += action;
        OnDestroy(node, () => Node.OnApplicationPaused -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is focused, i.e. when changing the focus from the OS desktop or a thirdparty application to any open window of the Godot instance.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public void OnApplicationFocusIn(Node? node, Action action) {
        Node.OnApplicationFocusIn += action;
        OnDestroy(node, () => Node.OnApplicationFocusIn -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is defocused, i.e. when changing the focus from any open window of the Godot instance to the OS desktop or a thirdparty application.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public void OnApplicationFocusOut(Node? node, Action action) {
        Node.OnApplicationFocusOut += action;
        OnDestroy(node, () => Node.OnApplicationFocusOut -= action);
    }

    /// <summary>
    /// <para>Event called when text server is changed.</para>
    /// </summary>
    public void OnTextServerChanged(Node? node, Action action) {
        Node.OnTextServerChanged += action;
        OnDestroy(node, () => Node.OnTextServerChanged -= action);
    }

    public void Reset() => Node.Reset();
}