using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Nodes;

public static class DefaultNodeHandlerExtensions {
    public static IEventHandler OnProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeEventHandler(node, new ProcessEventHandler(node.Name, action, pauseMode));
        DefaultNodeHandler.Instance.OnProcess(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnEveryProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeEventHandler(node, new ProcessEventHandler(node.Name, _ => action(), pauseMode));
        DefaultNodeHandler.Instance.OnEveryProcess(every, nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnEveryPhysicsProcess(this Node node, float every, Action action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeEventHandler(node, new ProcessEventHandler(node.Name, _ => action(), pauseMode));
        DefaultNodeHandler.Instance.OnEveryPhysicsProcess(every, nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnPhysicsProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeEventHandler(node, new ProcessEventHandler(node.Name, action, pauseMode));
        DefaultNodeHandler.Instance.OnPhysicsProcess(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, new InputEventEventHandler(node.Name, action, pauseMode));
        DefaultNodeHandler.Instance.OnInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnUnhandledInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, new InputEventEventHandler(node.Name, action, pauseMode));
        DefaultNodeHandler.Instance.OnUnhandledInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnShortcutInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, new InputEventEventHandler(node.Name, action, pauseMode));
        DefaultNodeHandler.Instance.OnShortcutInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnUnhandledKeyInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, new InputEventEventHandler(node.Name, action, pauseMode));
        DefaultNodeHandler.Instance.OnUnhandledKeyInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnDraw(this Node node, Action<CanvasItem> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new DrawNodeEventHandler(node, new DrawEventHandler(node.Name, action, pauseMode));
        DefaultNodeHandler.Instance.OnDraw(nodeEvent);
        return nodeEvent;
    }

    public static void QueueDraw(this Node node, Action<CanvasItem> action) {
        DefaultNodeHandler.Instance.QueueDraw(action);
    }
}
    
public static class NodeHandlerExtensions {

    public static IEventHandler OnProcess(this NodeHandler nodeHandler, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessEventHandler(null, action, pauseMode);
        nodeHandler.OnProcess(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnPhysicsProcess(this NodeHandler nodeHandler, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessEventHandler(null, action, pauseMode);
        nodeHandler.OnPhysicsProcess(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventEventHandler(null, action, pauseMode);
        nodeHandler.OnInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnUnhandledInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventEventHandler(null, action, pauseMode);
        nodeHandler.OnUnhandledInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnShortcutInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventEventHandler(null, action, pauseMode);
        nodeHandler.OnShortcutInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnUnhandledKeyInput(this NodeHandler nodeHandler, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventEventHandler(null, action, pauseMode);
        nodeHandler.OnUnhandledKeyInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnDraw(this NodeHandler nodeHandler, Action<CanvasItem> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new DrawEventHandler(null, action, pauseMode);
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

    public static IEventHandler OnEveryProcess(this NodeHandler nodeHandler, float every, IProcessHandler processHandler, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessEveryEventHandler(every, processHandler);
        nodeHandler.OnProcess(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnEveryPhysicsProcess(this NodeHandler nodeHandler, float every, IProcessHandler processHandler, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessEveryEventHandler(every, processHandler);
        nodeHandler.OnPhysicsProcess(nodeEvent);
        return nodeEvent;
    }

    public static Task AwaitInput(this NodeHandler nodeHandler, Func<InputEvent, bool> func, bool setInputAsHandled = true) {
        TaskCompletionSource promise = new();
        InputEventEventHandler eventHandler = null; 
        eventHandler = new InputEventEventHandler("AwaitInput", e => {
            if (func(e)) {
                if (setInputAsHandled) nodeHandler.GetViewport().SetInputAsHandled();
                eventHandler.Destroy();
                promise.TrySetResult();
            }
        }, Node.ProcessModeEnum.Always);
        nodeHandler.OnInput(eventHandler);
        return promise.Task;
    }
    
    public static Task AwaitUnhandledInput(this NodeHandler nodeHandler, Func<InputEvent, bool> func, bool setInputAsHandled = true) {
        TaskCompletionSource promise = new();
        InputEventEventHandler eventHandler = null; 
        eventHandler = new InputEventEventHandler("AwaitUnhandledInput", e => {
            if (func(e)) {
                if (setInputAsHandled) nodeHandler.GetViewport().SetInputAsHandled();
                eventHandler.Destroy();
                promise.TrySetResult();
            }
        }, Node.ProcessModeEnum.Always);
        nodeHandler.OnUnhandledInput(eventHandler);
        return promise.Task;
    }
    
}