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
    private static NodeManager? _mainInstance;
    private bool _addedToTree = false;
    
    /// <summary>
    /// You can call to MainInstance.Free(), and it will be recreated and added again to the scene tree.
    /// </summary>
    public static NodeManager MainInstance {
        get {
            if (_mainInstance == null || !IsInstanceValid(_mainInstance)) {
                _mainInstance = new NodeManager();
            }
            _mainInstance.TryAddToSceneTree();
            return _mainInstance;
        }
    }

    private void TryAddToSceneTree() {
        if (_addedToTree) return;
        var sceneTree = (SceneTree)Engine.GetMainLoop();
        if (sceneTree != null) {
            sceneTree.Root.AddChildDeferred(this);
            _addedToTree = true;
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
            if (_sceneTree != null) ConnectNextFrameSignal();
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