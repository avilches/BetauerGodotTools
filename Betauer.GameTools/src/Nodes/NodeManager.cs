using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Core.Nodes;
using Betauer.Core.Nodes.Events;
using Betauer.Core.Signal;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Nodes;

[Notifications]
[InputEvents]
public partial class NodeManager : Node {

    private static readonly NodeManager _mainInstance = new();
    private bool _addedToTree = false;
    
    public static NodeManager MainInstance {
        get {
            _mainInstance.TryAddToSceneTree();
            return _mainInstance;
        }
    }

    private void TryAddToSceneTree() {
        if (_addedToTree) return;
        var sceneTree = (SceneTree)Engine.GetMainLoop();
        if (sceneTree != null) {
            _addedToTree = true;
            sceneTree.Root.AddChildDeferred(_mainInstance);
        }
    }

    private static readonly Logger Logger = LoggerFactory.GetLogger<NodeManager>();
    private static readonly StringName CallDeferredInvocationMethodName = "CallDeferredInvocation";
    private static readonly StringName NextFrameInvocationMethodName = "NextFrameInvocation";
    private bool _callDeferredScheduled = false;
    private bool _nextFrameScheduled = false;
    private readonly Stack<Action> _deferredActions = new();
    private readonly Stack<Action> _nextFrameActions = new();
    private Viewport? _viewport;
    private SceneTree? _sceneTree;
    
    public List<(GodotObject, Action)> Watchers { get; } = new();

    public override partial void _Process(double delta);
    public override partial void _PhysicsProcess(double delta);
    public override partial void _Notification(int what);
    public override partial void _Input(InputEvent @event);
    public override partial void _UnhandledInput(InputEvent @event);
    public override partial void _UnhandledKeyInput(InputEvent @event);
    public override partial void _ShortcutInput(InputEvent @event);

    public bool IsMouseInsideScreen { get; private set; } = false;
    public bool IsWindowFocused { get; private set; } = true;
    public bool IsApplicationFocused { get; private set; } = true;
    
    public NodeManager() {
        ProcessMode = ProcessModeEnum.Always;
        OnProcess += PurgeWatchers;
        OnWMMouseEnter += () => IsMouseInsideScreen = true;
        OnWMMouseExit += () => IsMouseInsideScreen = false;
        
        OnWMWindowFocusIn += () => IsWindowFocused = true;
        OnWMWindowFocusOut += () => IsWindowFocused = false;
        
        OnApplicationFocusIn += () => IsApplicationFocused = true;
        OnApplicationFocusOut += () => IsApplicationFocused = false;
    }

    public override void _EnterTree() {
        _viewport = GetViewport();
        _sceneTree = GetTree();
        GetParent().ChildEnteredTree += EnsureLastChild;
        if (_nextFrameActions.Count > 0) {
            ConnectNextFrameSignal();
        }
    }

    public override void _ExitTree() {
        _viewport = null;
        _sceneTree = null;
        GetParent().ChildEnteredTree -= EnsureLastChild;
    }

    private void EnsureLastChild(Node _) {
        GetParent()?.MoveChildDeferred(this, -1);
    }

    private void ConnectNextFrameSignal() {
        _nextFrameScheduled = true;
        _sceneTree.Connect(SceneTree.SignalName.ProcessFrame, new Callable(this, NextFrameInvocationMethodName), (uint)ConnectFlags.OneShot);
    }

    public void CallDeferred(Action action) {
        if (!_callDeferredScheduled) {
            _callDeferredScheduled = true;
            CallDeferred(CallDeferredInvocationMethodName);
        }
        _deferredActions.Push(action);
    }

    private void CallDeferredInvocation() {
        _callDeferredScheduled = false;
        while (_deferredActions.Count > 0) {
            _deferredActions.Pop().Invoke();
        }
    }

    public void NextFrame(Action action) {
        if (!_nextFrameScheduled) {
            if (_sceneTree != null) {
                ConnectNextFrameSignal();
            }
        }
        _nextFrameActions.Push(action);
    }

    private void NextFrameInvocation() {
        _nextFrameScheduled = false;
        while (_nextFrameActions.Count > 0) {
            _nextFrameActions.Pop().Invoke();
        }
    }

    public void OnDestroy(GodotObject o, Action removeAction) {
        if (o == null) return;
        Watchers.Add((o, removeAction));
        SetProcess(true);
    }

    public void RemoveOnDestroy(GodotObject o, Action removeAction) {
        if (o == null) return;
        Watchers.Remove((o, removeAction));
    }

    private void PurgeWatchers(double d) {
        var destroyed = 0;
        Watchers.RemoveAll(tuple => {
            if (IsInstanceValid(tuple.Item1)) return false;
            tuple.Item2();
            destroyed++;
            return true;
        });
        if (destroyed > 0) {
            Logger.Debug("Removed {0} destroyed GodotObjects", destroyed);
        }
    }
    
    public void AddOnProcess(GodotObject watched, Action<double> action) {
        OnProcess += action;
        OnDestroy(watched, () => OnProcess -= action);
    }

    public void AddOnPhysicsProcess(GodotObject watched, Action<double> action) {
        OnPhysicsProcess += action;
        OnDestroy(watched, () => OnPhysicsProcess -= action);
    }

    public void AddOnInput(GodotObject watched, Action<InputEvent> action) {
        OnInput += action;
        OnDestroy(watched, () => OnInput -= action);
    }
    
    public void AddOnUnhandledInput(GodotObject watched, Action<InputEvent> action) {
        OnUnhandledInput += action;
        OnDestroy(watched, () => OnUnhandledInput -= action);
    }

    public void AddOnUnhandledKeyInput(GodotObject watched, Action<InputEvent> action) {
        OnUnhandledKeyInput += action;
        OnDestroy(watched, () => OnUnhandledKeyInput -= action);
    }

    public void AddOnShortcutInput(GodotObject watched, Action<InputEvent> action) {
        OnShortcutInput += action;
        OnDestroy(watched, () => OnShortcutInput -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the mouse enters the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public void AddOnWMMouseEnter(GodotObject watched, Action action) {
        OnWMMouseEnter += action;
        OnDestroy(watched, () => OnWMMouseEnter -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the mouse leaves the game window.</para>
    /// <para>Implemented on desktop and web platforms.</para>
    /// </summary>
    public void AddOnWMMouseExit(GodotObject watched, Action action) {
        OnWMMouseExit += action;
        OnDestroy(watched, () => OnWMMouseExit -= action);
    }

    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is focused. This may be a change of focus between two windows of the same engine instance, or from the OS desktop or a third-party application to a window of the game (in which case <see cref="F:Godot.NotificationApplicationFocusIn" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is focused.</para>
    /// </summary>
    public void AddOnWMWindowFocusIn(GodotObject watched, Action action) {
        OnWMWindowFocusIn += action;
        OnDestroy(watched, () => OnWMWindowFocusIn -= action);
    }

    /// <summary>
    /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is defocused. This may be a change of focus between two windows of the same engine instance, or from a window of the game to the OS desktop or a third-party application (in which case <see cref="F:Godot.NotificationApplicationFocusOut" /> is also emitted).</para>
    /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is defocused.</para>
    /// </summary>
    public void AddOnWMWindowFocusOut(GodotObject watched, Action action) {
        OnWMWindowFocusOut += action;
        OnDestroy(watched, () => OnWMWindowFocusOut -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a close request is sent (e.g. closing the window with a "Close" button or Alt + F4).</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public void AddOnWMCloseRequest(GodotObject watched, Action action) {
        OnWMCloseRequest += action;
        OnDestroy(watched, () => OnWMCloseRequest -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a go back request is sent (e.g. pressing the "Back" button on Android).</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public void AddOnWMGoBackRequest(GodotObject watched, Action action) {
        OnWMGoBackRequest += action;
        OnDestroy(watched, () => OnWMGoBackRequest -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the window is resized.</para>
    /// </summary>
    public void AddOnWMSizeChanged(GodotObject watched, Action action) {
        OnWMSizeChanged += action;
        OnDestroy(watched, () => OnWMSizeChanged -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the screen's DPI has been changed. Only implemented on macOS.</para>
    /// </summary>
    public void AddOnWMDpiChange(GodotObject watched, Action action) {
        OnWMDpiChange += action;
        OnDestroy(watched, () => OnWMDpiChange -= action);
    }

    /// <summary>
    /// <para>Event called when the mouse enters the viewport.</para>
    /// </summary>
    public void AddOnVpMouseEnter(GodotObject watched, Action action) {
        OnVpMouseEnter += action;
        OnDestroy(watched, () => OnVpMouseEnter -= action);
    }

    /// <summary>
    /// <para>Event called when the mouse leaves the viewport.</para>
    /// </summary>
    public void AddOnVpMouseExit(GodotObject watched, Action action) {
        OnVpMouseExit += action;
        OnDestroy(watched, () => OnVpMouseExit -= action);
    }
    
    /// <summary>
    /// <para>Event called from the OS when the application is exceeding its allocated memory.</para>
    /// <para>Specific to the iOS platform.</para>
    /// </summary>
    public void AddOnOsMemoryWarning(GodotObject watched, Action action) {
        OnOsMemoryWarning += action;
        OnDestroy(watched, () => OnOsMemoryWarning -= action);
    }

    /// <summary>
    /// <para>Event called when translations may have changed. Can be triggered by the user changing the locale. Can be used to respond to language changes, for example to change the UI strings on the fly. Useful when working with the built-in translation support, like <see cref="M:Godot.GodotObject.Tr(Godot.StringName,Godot.StringName)" />.</para>
    /// </summary>
    public void AddOnTranslationChanged(GodotObject watched, Action action) {
        OnTranslationChanged += action;
        OnDestroy(watched, () => OnTranslationChanged -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when a request for "About" information is sent.</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public void AddOnWMAbout(GodotObject watched, Action action) {
        OnWMAbout += action;
        OnDestroy(watched, () => OnWMAbout -= action);
    }

    /// <summary>
    /// <para>Event called from Godot's crash handler when the engine is about to crash.</para>
    /// <para>Implemented on desktop platforms if the crash handler is enabled.</para>
    /// </summary>
    public void AddOnCrash(GodotObject watched, Action action) {
        OnCrash += action;
        OnDestroy(watched, () => OnCrash -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when an update of the Input Method Engine occurs (e.g. change of IME cursor position or composition string).</para>
    /// <para>Specific to the macOS platform.</para>
    /// </summary>
    public void AddOnOsImeUpdate(GodotObject watched, Action action) {
        OnOsImeUpdate += action;
        OnDestroy(watched, () => OnOsImeUpdate -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is resumed.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public void AddOnApplicationResumed(GodotObject watched, Action action) {
        OnApplicationResumed += action;
        OnDestroy(watched, () => OnApplicationResumed -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is paused.</para>
    /// <para>Specific to the Android platform.</para>
    /// </summary>
    public void AddOnApplicationPaused(GodotObject watched, Action action) {
        OnApplicationPaused += action;
        OnDestroy(watched, () => OnApplicationPaused -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is focused, i.e. when changing the focus from the OS desktop or a thirdparty application to any open window of the Godot instance.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public void AddOnApplicationFocusIn(GodotObject watched, Action action) {
        OnApplicationFocusIn += action;
        OnDestroy(watched, () => OnApplicationFocusIn -= action);
    }

    /// <summary>
    /// <para>Event called from the OS when the application is defocused, i.e. when changing the focus from any open window of the Godot instance to the OS desktop or a thirdparty application.</para>
    /// <para>Implemented on desktop platforms.</para>
    /// </summary>
    public void AddOnApplicationFocusOut(GodotObject watched, Action action) {
        OnApplicationFocusOut += action;
        OnDestroy(watched, () => OnApplicationFocusOut -= action);
    }

    /// <summary>
    /// <para>Event called when text server is changed.</para>
    /// </summary>
    public void AddOnTextServerChanged(GodotObject watched, Action action) {
        OnTextServerChanged += action;
        OnDestroy(watched, () => OnTextServerChanged -= action);
    }

    public Task<InputEvent> AwaitInput(Func<InputEvent, bool> predicate, float timeout = 0) {
        TaskCompletionSource<InputEvent> promise = new();
        Action<InputEvent> handler = null!;
        handler = (e) => {
            if (promise.Task.IsCompleted || !predicate(e)) return;
            OnInput -= handler;
            _viewport!.SetInputAsHandled();
            promise.TrySetResult(e);
        };
        if (timeout > 0) {
            _sceneTree!.CreateTimer(timeout).OnTimeout(() => {
                OnInput -= handler;
                if (!promise.Task.IsCompleted) promise.TrySetResult(null);
            });
        }
        OnInput += handler;
        return promise.Task;
    }

    
}