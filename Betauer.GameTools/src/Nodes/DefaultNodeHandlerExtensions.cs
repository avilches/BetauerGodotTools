using System;
using System.Threading.Tasks;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Nodes;

public static class NodeHandlerExtensions {
    public static NodeHandler GetNodeHandler(this Node node) {
        return node.FirstChildInParentOrNull<NodeHandler>() ?? DefaultNodeHandler.Instance;
    } 
        
    public static ProcessNodeWrapper OnProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(action, pauseMode, node.Name));
        node.GetNodeHandler().OnProcess(handler);
        return handler;
    }

    public static ProcessNodeWrapper OnPhysicsProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(action, pauseMode, node.Name));
        node.GetNodeHandler().OnPhysicsProcess(handler);
        return handler;
    }

    public static ProcessEveryWrapper OnEveryProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(_ => action(), pauseMode, node.Name));
        return node.GetNodeHandler().OnEveryProcess(every, handler);
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new ProcessNodeWrapper(node, new ProcessHandler(_ => action(), pauseMode, node.Name));
        return node.GetNodeHandler().OnEveryPhysicsProcess(every, handler);
    }

    public static InputEventNodeWrapper OnInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        node.GetNodeHandler().OnInput(handler);
        return handler;
    }

    public static InputEventNodeWrapper OnUnhandledInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        node.GetNodeHandler().OnUnhandledInput(handler);
        return handler;
    }

    public static InputEventNodeWrapper OnShortcutInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        node.GetNodeHandler().OnShortcutInput(handler);
        return handler;
    }

    public static InputEventNodeWrapper OnUnhandledKeyInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var handler = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        node.GetNodeHandler().OnUnhandledKeyInput(handler);
        return handler;
    }

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