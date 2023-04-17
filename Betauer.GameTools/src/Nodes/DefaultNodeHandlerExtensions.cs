using System;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Betauer.Core.Time;
using Godot;

namespace Betauer.Nodes;

public static class DefaultNodeHandlerExtensions {
    public static ProcessNodeWrapper OnProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnProcess(handler);
        return handler;
    }

    public static ProcessNodeWrapper OnPhysicsProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnPhysicsProcess(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(_ => action(), pauseMode, node.Name));
        return DefaultNodeHandler.Instance.OnEveryProcess(every, handler);
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(_ => action(), pauseMode, node.Name));
        return DefaultNodeHandler.Instance.OnEveryPhysicsProcess(every, handler);
    }

    public static InputEventNodeWrapper OnInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnInput(handler);
        return handler;
    }

    public static InputEventNodeWrapper OnUnhandledInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnUnhandledInput(handler);
        return handler;
    }

    public static InputEventNodeWrapper OnShortcutInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnShortcutInput(handler);
        return handler;
    }

    public static InputEventNodeWrapper OnUnhandledKeyInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnUnhandledKeyInput(handler);
        return handler;
    }

    public static DrawNodeWrapper OnDraw(this Node node, Action<CanvasItem> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new DrawNodeWrapper(node, new DrawHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnDraw(handler);
        return handler;
    }

    public static void QueueDraw(this Node node, Action<CanvasItem> action) {
        DefaultNodeHandler.Instance.QueueDraw(action);
    }
}
    
public static class NodeHandlerExtensions {

    public static ProcessHandler OnProcess(this NodeHandler nodeHandler, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessHandler(action, pauseMode);
        nodeHandler.OnProcess(handler);
        return handler;
    }

    public static ProcessHandler OnPhysicsProcess(this NodeHandler nodeHandler, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessHandler(action, pauseMode);
        nodeHandler.OnPhysicsProcess(handler);
        return handler;
    }

    public static InputEventHandler OnInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventHandler(action, pauseMode);
        nodeHandler.OnInput(handler);
        return handler;
    }

    public static InputEventHandler OnUnhandledInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventHandler(action, pauseMode);
        nodeHandler.OnUnhandledInput(handler);
        return handler;
    }

    public static InputEventHandler OnShortcutInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventHandler(action, pauseMode);
        nodeHandler.OnShortcutInput(handler);
        return handler;
    }

    public static InputEventHandler OnUnhandledKeyInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventHandler(action, pauseMode);
        nodeHandler.OnUnhandledKeyInput(handler);
        return handler;
    }

    public static DrawHandler OnDraw(this NodeHandler nodeHandler, Action<CanvasItem> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new DrawHandler(action, pauseMode);
        nodeHandler.OnDraw(handler);
        return handler;
    }

    public static DrawHandler QueueDraw(this NodeHandler nodeHandler, Action<CanvasItem> action) {
        DrawHandler handler = null;
        handler = nodeHandler.OnDraw(canvas => {
            action(canvas);
            handler.Disable();
        }, Node.ProcessModeEnum.Always);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryProcess(this NodeHandler nodeHandler, float every, IProcessHandler processHandler, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessEveryWrapper(every, processHandler);
        nodeHandler.OnProcess(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this NodeHandler nodeHandler, float every, IProcessHandler processHandler, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessEveryWrapper(every, processHandler);
        nodeHandler.OnPhysicsProcess(handler);
        return handler;
    }

    public static Task<InputEvent> AwaitInput(this NodeHandler nodeHandler, Func<InputEvent, bool> predicate, bool setInputAsHandled = true, float timeout = 0) {
        TaskCompletionSource<InputEvent> promise = new();
        InputEventHandler handler = null;
        handler = new InputEventHandler(e => {
            if (handler.IsDestroyed || promise.Task.IsCompleted || !predicate(e)) return;
            if (setInputAsHandled) nodeHandler.GetViewport().SetInputAsHandled();
            promise.TrySetResult(e);
            handler.Destroy();
        }, Node.ProcessModeEnum.Always, "AwaitInput");
        if (timeout > 0) {
            nodeHandler.GetTree().CreateTimer(timeout).OnTimeout(() => {
                if (handler.IsDestroyed || promise.Task.IsCompleted) return;
                handler.Destroy();
                promise.TrySetResult(null);
            });
        }
        nodeHandler.OnInput(handler);
        return promise.Task;
    }
    
    public static Task<InputEvent> AwaitUnhandledInput(this NodeHandler nodeHandler, Func<InputEvent, bool> predicate, bool setInputAsHandled = true, float timeout = 0) {
        TaskCompletionSource<InputEvent> promise = new();
        InputEventHandler handler = null; 
        handler = new InputEventHandler(e => {
            if (handler.IsDestroyed || promise.Task.IsCompleted || !predicate(e)) return;
            if (setInputAsHandled) nodeHandler.GetViewport().SetInputAsHandled();
            promise.TrySetResult(e);
            handler.Destroy();
        }, Node.ProcessModeEnum.Always, "AwaitUnhandledInput");
        if (timeout > 0) {
            nodeHandler.GetTree().CreateTimer(timeout).OnTimeout(() => {
                if (handler.IsDestroyed || promise.Task.IsCompleted) return;
                handler.Destroy();
                promise.TrySetResult(null);
            });
        }
        nodeHandler.OnUnhandledInput(handler);
        return promise.Task;
    }
    
}