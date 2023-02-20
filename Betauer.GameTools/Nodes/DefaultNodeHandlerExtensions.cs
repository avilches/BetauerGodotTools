using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Nodes;

public static class DefaultNodeHandlerExtensions {
    public static IEventHandler OnProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeEventHandler(node, action, pauseMode);
        DefaultNodeHandler.Instance.OnProcess(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnPhysicsProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new ProcessNodeEventHandler(node, action, pauseMode);
        DefaultNodeHandler.Instance.OnPhysicsProcess(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, action, pauseMode);
        DefaultNodeHandler.Instance.OnInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnUnhandledInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, action, pauseMode);
        DefaultNodeHandler.Instance.OnUnhandledInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnShortcutInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, action, pauseMode);
        DefaultNodeHandler.Instance.OnShortcutInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnUnhandledKeyInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new InputEventNodeEventHandler(node, action, pauseMode);
        DefaultNodeHandler.Instance.OnUnhandledKeyInput(nodeEvent);
        return nodeEvent;
    }

    public static IEventHandler OnDraw(this Node node, Action<CanvasItem> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) {
        var nodeEvent = new DrawNodeEventHandler(node, action, pauseMode);
        DefaultNodeHandler.Instance.OnDraw(nodeEvent);
        return nodeEvent;
    }

    public static void QueueDraw(this Node node, Action<CanvasItem> action) {
        IEventHandler nodeEvent = null;
        nodeEvent = node.OnDraw(canvas => {
            action(canvas);
            nodeEvent.Disable();
        }, Node.ProcessModeEnum.Always);
    }
}