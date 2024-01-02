using System;
using System.Threading.Tasks;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Nodes;
                           /*
public static class NodeManagerExtensions {
    public static NodeManager GetNodeHandler(this Node node) {
        return node.FirstChildInParentOrNull<NodeManager>() ?? NodeManager.MainInstance;
    }
    
    public static void OnDestroy(this Node node, Action action) {
        node.GetNodeHandler().OnDestroy(node, action);
    }
        
    public static void OnDestroy(this GodotObject godotObject, Action action) {
        NodeManager.MainInstance.OnDestroy(godotObject, action);
    }
    
    public static ProcessNodeHandler OnProcess(this Node node, Action<double> action) {
        var handler = new ProcessNodeHandler(node, new ProcessHandler(action, node.Name));
        node.GetNodeHandler().OnProcess(handler);
        return handler;
    }

    public static ProcessNodeHandler OnPhysicsProcess(this Node node, Action<double> action) {
        var handler = new ProcessNodeHandler(node, new ProcessHandler(action, node.Name));
        node.GetNodeHandler().OnPhysicsProcess(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryProcess(this Node node, float every, Action action) {
        var handler = new ProcessNodeHandler(node, new ProcessHandler(_ => action(), node.Name));
        return node.GetNodeHandler().OnEveryProcess(every, handler);
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this Node node, float every, Action action) {
        var handler = new ProcessNodeHandler(node, new ProcessHandler(_ => action(), node.Name));
        return node.GetNodeHandler().OnEveryPhysicsProcess(every, handler);
    }

    public static InputEventNodeHandler OnInput(this Node node, Action<InputEvent> action) {
        var handler = new InputEventNodeHandler(node, new InputEventHandler(action, node.Name));
        node.GetNodeHandler().OnInput(handler);
        return handler;
    }

    public static InputEventNodeHandler OnUnhandledInput(this Node node, Action<InputEvent> action) {
        var handler = new InputEventNodeHandler(node, new InputEventHandler(action, node.Name));
        node.GetNodeHandler().OnUnhandledInput(handler);
        return handler;
    }

    public static InputEventNodeHandler OnShortcutInput(this Node node, Action<InputEvent> action) {
        var handler = new InputEventNodeHandler(node, new InputEventHandler(action, node.Name));
        node.GetNodeHandler().OnShortcutInput(handler);
        return handler;
    }

    public static InputEventNodeHandler OnUnhandledKeyInput(this Node node, Action<InputEvent> action) {
        var handler = new InputEventNodeHandler(node, new InputEventHandler(action, node.Name));
        node.GetNodeHandler().OnUnhandledKeyInput(handler);
        return handler;
    }

    public static ProcessHandler OnProcess(this NodeManager nodeManager, Action<double> action) {
        var handler = new ProcessHandler(action);
        nodeManager.OnProcess(handler);
        return handler;
    }

    public static ProcessHandler OnPhysicsProcess(this NodeManager nodeManager, Action<double> action) {
        var handler = new ProcessHandler(action);
        nodeManager.OnPhysicsProcess(handler);
        return handler;
    }

    public static InputEventHandler OnInput(this NodeManager nodeManager, Action<InputEvent> action) {
        var handler = new InputEventHandler(action);
        nodeManager.OnInput(handler);
        return handler;
    }

    public static InputEventHandler OnUnhandledInput(this NodeManager nodeManager, Action<InputEvent> action) {
        var handler = new InputEventHandler(action);
        nodeManager.OnUnhandledInput(handler);
        return handler;
    }

    public static InputEventHandler OnShortcutInput(this NodeManager nodeManager, Action<InputEvent> action) {
        var handler = new InputEventHandler(action);
        nodeManager.OnShortcutInput(handler);
        return handler;
    }

    public static InputEventHandler OnUnhandledKeyInput(this NodeManager nodeManager, Action<InputEvent> action) {
        var handler = new InputEventHandler(action);
        nodeManager.OnUnhandledKeyInput(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryProcess(this NodeManager nodeManager, float every, IProcessHandler processHandler) {
        var handler = new ProcessEveryWrapper(every, processHandler);
        nodeManager.OnProcess(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryProcess(this Node node, float every, IProcessHandler processHandler) {
        var handler = new ProcessEveryWrapper(every, processHandler);
        node.GetNodeHandler().OnProcess(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this NodeManager nodeManager, float every, IProcessHandler processHandler) {
        var handler = new ProcessEveryWrapper(every, processHandler);
        nodeManager.OnPhysicsProcess(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this Node node, float every, IProcessHandler processHandler) {
        var handler = new ProcessEveryWrapper(every, processHandler);
        node.GetNodeHandler().OnPhysicsProcess(handler);
        return handler;
    }
    
    public static Task<InputEvent> AwaitInput(this NodeManager nodeManager, Func<InputEvent, bool> predicate, bool setInputAsHandled = true, float timeout = 0) {
        TaskCompletionSource<InputEvent> promise = new();
        InputEventHandler handler = null;
        handler = new InputEventHandler(e => {
            if (handler.IsDestroyed || promise.Task.IsCompleted || !predicate(e)) return;
            if (setInputAsHandled) nodeManager.Node.GetViewport().SetInputAsHandled();
            promise.TrySetResult(e);
            handler.Destroy();
        }, "AwaitInput");
        if (timeout > 0) {
            nodeManager.Node.GetTree().CreateTimer(timeout).OnTimeout(() => {
                if (handler.IsDestroyed || promise.Task.IsCompleted) return;
                handler.Destroy();
                promise.TrySetResult(null);
            });
        }
        nodeManager.OnInput(handler);
        return promise.Task;
    }
    
    public static Task<InputEvent> AwaitUnhandledInput(this NodeManager nodeManager, Func<InputEvent, bool> predicate, bool setInputAsHandled = true, float timeout = 0) {
        TaskCompletionSource<InputEvent> promise = new();
        InputEventHandler handler = null; 
        handler = new InputEventHandler(e => {
            if (handler.IsDestroyed || promise.Task.IsCompleted || !predicate(e)) return;
            if (setInputAsHandled) nodeManager.Node.GetViewport().SetInputAsHandled();
            promise.TrySetResult(e);
            handler.Destroy();
        }, "AwaitUnhandledInput");
        if (timeout > 0) {
            nodeManager.Node.GetTree().CreateTimer(timeout).OnTimeout(() => {
                if (handler.IsDestroyed || promise.Task.IsCompleted) return;
                handler.Destroy();
                promise.TrySetResult(null);
            });
        }
        nodeManager.OnUnhandledInput(handler);
        return promise.Task;
    }
    
}                            */