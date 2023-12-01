using System;
using System.Threading.Tasks;
using Betauer.Core.Nodes.Events;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Nodes;

public static class NodeEventHandlerExtensions {

    public static T GetNodeEvent<T>(this Node node) where T : INodeEvent {
        if (node is T @event) return @event;
        return (T)(object)NodeEventHandler.DefaultInstance;
    }

    public static ProcessEventHolder AddOnProcess(this Node watcher, Action<double> processAction) {
        var processEvent = watcher.GetNodeEvent<IProcessEvent>();
        return new ProcessEventHolder(watcher, processEvent, processAction);
    }

    public static PhysicsProcessEventHolder AddOnPhysicsProcess(this Node watcher, Action<double> physicsProcessAction) {
        var processEvent = watcher.GetNodeEvent<IPhysicsProcessEvent>();
        return new PhysicsProcessEventHolder(watcher, processEvent, physicsProcessAction);
    }

    public static InputEventHolder AddOnInput(this Node watcher, Action<InputEvent> inputEventAction) {
        var inputEvent = watcher.GetNodeEvent<IInputEvent>();
        return new InputEventHolder(watcher, inputEvent, inputEventAction);
    }

    public static UnhandledInputEventHolder AddOnUnhandledInput(this Node watcher, Action<InputEvent> unhandledInputEventAction) {
        var unhandledInputEvent = watcher.GetNodeEvent<IUnhandledInputEvent>();
        return new UnhandledInputEventHolder(watcher, unhandledInputEvent, unhandledInputEventAction);
    }

    public static UnhandledKeyInputEventHolder AddOnUnhandledKeyInput(this Node watcher, Action<InputEvent> unhandledKeyInputEventAction) {
        var unhandledKeyInputEvent = watcher.GetNodeEvent<IUnhandledKeyInputEvent>();
        return new UnhandledKeyInputEventHolder(watcher, unhandledKeyInputEvent, unhandledKeyInputEventAction);
    }

    public static ShortcutInputEventHolder AddOnShortcutInput(this Node watcher, Action<InputEvent> shortcutInputEventAction) {
        var shortcutInputEvent = watcher.GetNodeEvent<IShortcutInputEvent>();
        return new ShortcutInputEventHolder(watcher, shortcutInputEvent, shortcutInputEventAction);
    }

    public static ProcessEventHolder OnEveryProcess(this Node node, float every, Action action) {
        var accumulated = 0d;
        return node.AddOnProcess((double delta) => {
            accumulated += delta;
            if (accumulated >= every) {
                action.Invoke();
                accumulated = every - accumulated;
            }
        });
    }

    public static PhysicsProcessEventHolder OnEveryPhysicsProcess(this Node node, float every, Action action) {
        var accumulated = 0d;
        return node.AddOnPhysicsProcess((double delta) => {
            accumulated += delta;
            if (accumulated >= every) {
                action.Invoke();
                accumulated = every - accumulated;
            }
        });
    }

    public static Task<InputEvent> AwaitInput(this Node node, Func<InputEvent, bool> predicate, bool setInputAsHandled = true, float timeout = 0) {
        TaskCompletionSource<InputEvent> promise = new();
        Action<InputEvent> handler = null!;
        var eventHandler = node.GetNodeEvent<IInputEvent>();
        handler = (inputEvent) => {
            if (promise.Task.IsCompleted || !predicate(inputEvent)) return;
            if (setInputAsHandled) node.GetViewport().SetInputAsHandled();
            promise.TrySetResult(inputEvent);
            eventHandler.OnInput -= handler;
        };
        eventHandler.OnInput += handler;
        if (timeout > 0) {
            node.GetTree().CreateTimer(timeout).OnTimeout(() => {
                if (promise.Task.IsCompleted) return;
                eventHandler.OnInput -= handler;
                promise.TrySetResult(null);
            });
        }
        return promise.Task;
    }

    
}