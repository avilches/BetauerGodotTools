using System;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Godot;

namespace Betauer.Nodes;

public class UnhandledKeyInputEventHolder {
    public Node? Watcher { get; set; }
    public IUnhandledKeyInputEvent Event { get; }
    public Action<InputEvent> UnhandledKeyInputEventAction { get; }
    public bool IsEnabled { get; private set; } = false;

    public UnhandledKeyInputEventHolder(IUnhandledKeyInputEvent @event, Action<InputEvent> unhandledKeyInputEventAction) {
        Event = @event;
        UnhandledKeyInputEventAction = unhandledKeyInputEventAction;
        Enable();
    }

    public UnhandledKeyInputEventHolder(Node watcher, IUnhandledKeyInputEvent @event, Action<InputEvent> unhandledKeyInputEventAction) {
        Watcher = watcher;
        Event = @event;
        UnhandledKeyInputEventAction = unhandledKeyInputEventAction;
        Enable();
    }

    public void Enable(bool enable = true) {
        if (IsEnabled == enable) return;
        if (enable) Event.OnUnhandledKeyInput += OnUnhandledKeyInputEvent;
        else Event.OnUnhandledKeyInput -= OnUnhandledKeyInputEvent;
        IsEnabled = enable;
    }

    public void Disable() {
        Enable(false);
    }

    private void OnUnhandledKeyInputEvent(InputEvent @event) {
        try {
            UnhandledKeyInputEventAction(@event);
        } catch (Exception) {
            if (Watcher != null && !Watcher.IsInstanceValid()) Disable();
            else throw;
        }
    }
}