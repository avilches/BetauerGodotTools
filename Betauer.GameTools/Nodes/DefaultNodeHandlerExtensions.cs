using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Nodes;

public static class DefaultNodeHandlerExtensions {
    public static ProcessNodeWrapper OnProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeWrapper(node, new ProcessHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnProcess(nodeEvent);
        return nodeEvent;
    }

    public static ProcessNodeWrapper OnPhysicsProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeWrapper(node, new ProcessHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnPhysicsProcess(nodeEvent);
        return nodeEvent;
    }

    public static ProcessEveryWrapper OnEveryProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeWrapper(node, new ProcessHandler(_ => action(), pauseMode, node.Name));
        return DefaultNodeHandler.Instance.OnEveryProcess(every, nodeEvent);
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeWrapper(node, new ProcessHandler(_ => action(), pauseMode, node.Name));
        return DefaultNodeHandler.Instance.OnEveryPhysicsProcess(every, nodeEvent);
    }

    public static InputEventNodeWrapper OnInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnInput(nodeEvent);
        return nodeEvent;
    }

    public static InputEventNodeWrapper OnUnhandledInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnUnhandledInput(nodeEvent);
        return nodeEvent;
    }

    public static InputEventNodeWrapper OnShortcutInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnShortcutInput(nodeEvent);
        return nodeEvent;
    }

    public static InputEventNodeWrapper OnUnhandledKeyInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeWrapper(node, new InputEventHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnUnhandledKeyInput(nodeEvent);
        return nodeEvent;
    }

    public static DrawNodeWrapper OnDraw(this Node node, Action<CanvasItem> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new DrawNodeWrapper(node, new DrawHandler(action, pauseMode, node.Name));
        DefaultNodeHandler.Instance.OnDraw(nodeEvent);
        return nodeEvent;
    }

    public static void QueueDraw(this Node node, Action<CanvasItem> action) {
        DefaultNodeHandler.Instance.QueueDraw(action);
    }
}
    
public static class NodeHandlerExtensions {

    public static ProcessHandler OnProcess(this NodeHandler nodeHandler, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessHandler(action, pauseMode);
        nodeHandler.OnProcess(nodeEvent);
        return nodeEvent;
    }

    public static ProcessHandler OnPhysicsProcess(this NodeHandler nodeHandler, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessHandler(action, pauseMode);
        nodeHandler.OnPhysicsProcess(nodeEvent);
        return nodeEvent;
    }

    public static InputEventHandler OnInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventHandler(action, pauseMode);
        nodeHandler.OnInput(nodeEvent);
        return nodeEvent;
    }

    public static InputEventHandler OnUnhandledInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventHandler(action, pauseMode);
        nodeHandler.OnUnhandledInput(nodeEvent);
        return nodeEvent;
    }

    public static InputEventHandler OnShortcutInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventHandler(action, pauseMode);
        nodeHandler.OnShortcutInput(nodeEvent);
        return nodeEvent;
    }

    public static InputEventHandler OnUnhandledKeyInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventHandler(action, pauseMode);
        nodeHandler.OnUnhandledKeyInput(nodeEvent);
        return nodeEvent;
    }

    public static DrawHandler OnDraw(this NodeHandler nodeHandler, Action<CanvasItem> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new DrawHandler(action, pauseMode);
        nodeHandler.OnDraw(nodeEvent);
        return nodeEvent;
    }

    public static void QueueDraw(this NodeHandler nodeHandler, Action<CanvasItem> action) {
        IEventHandler nodeEvent = null;
        nodeEvent = nodeHandler.OnDraw(canvas => {
            action(canvas);
            nodeEvent.Disable();
        }, Node.ProcessModeEnum.Always);
    }

    public static ProcessEveryWrapper OnEveryProcess(this NodeHandler nodeHandler, float every, IProcessHandler processHandler, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessEveryWrapper(every, processHandler);
        nodeHandler.OnProcess(nodeEvent);
        return nodeEvent;
    }

    public static ProcessEveryWrapper OnEveryPhysicsProcess(this NodeHandler nodeHandler, float every, IProcessHandler processHandler, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessEveryWrapper(every, processHandler);
        nodeHandler.OnPhysicsProcess(nodeEvent);
        return nodeEvent;
    }

    public static Task AwaitInput(this NodeHandler nodeHandler, Func<InputEvent, bool> func, bool setInputAsHandled = true) {
        TaskCompletionSource promise = new();
        InputEventHandler eventHandler = null; 
        eventHandler = new InputEventHandler(e => {
            if (func(e)) {
                if (setInputAsHandled) nodeHandler.GetViewport().SetInputAsHandled();
                eventHandler.Destroy();
                promise.TrySetResult();
            }
        }, Node.ProcessModeEnum.Always, "AwaitInput");
        nodeHandler.OnInput(eventHandler);
        return promise.Task;
    }
    
    public static Task AwaitUnhandledInput(this NodeHandler nodeHandler, Func<InputEvent, bool> func, bool setInputAsHandled = true) {
        TaskCompletionSource promise = new();
        InputEventHandler eventHandler = null; 
        eventHandler = new InputEventHandler(e => {
            if (func(e)) {
                if (setInputAsHandled) nodeHandler.GetViewport().SetInputAsHandled();
                eventHandler.Destroy();
                promise.TrySetResult();
            }
        }, Node.ProcessModeEnum.Always, "AwaitUnhandledInput");
        nodeHandler.OnUnhandledInput(eventHandler);
        return promise.Task;
    }
    
}