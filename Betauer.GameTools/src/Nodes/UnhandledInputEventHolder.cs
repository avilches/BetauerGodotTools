using System;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Godot;

namespace Betauer.Nodes;

public class UnhandledInputEventHolder {
    public Node? Watcher { get; set; }
    public IUnhandledInputEvent Event { get; }
    public Action<InputEvent> UnhandledInputEventAction { get; }
    public bool IsEnabled { get; private set; } = false;

    public UnhandledInputEventHolder(IUnhandledInputEvent @event, Action<InputEvent> unhandledInputEventAction) {
        Event = @event;
        UnhandledInputEventAction = unhandledInputEventAction;
        Enable();
    }

    public UnhandledInputEventHolder(Node watcher, IUnhandledInputEvent @event, Action<InputEvent> unhandledInputEventAction) {
        Watcher = watcher;
        Event = @event;
        UnhandledInputEventAction = unhandledInputEventAction;
        Enable();
    }

    public void Enable(bool enable = true) {
        if (IsEnabled == enable) return;
        if (enable) Event.OnUnhandledInput += OnUnhandledInputEvent;
        else Event.OnUnhandledInput -= OnUnhandledInputEvent;
        IsEnabled = enable;
    }

    public void Disable() {
        Enable(false);
    }

    private void OnUnhandledInputEvent(InputEvent @event) {
        try {
            UnhandledInputEventAction(@event);
        } catch (Exception) {
            if (Watcher != null && !Watcher.IsInstanceValid()) Disable();
            else throw;
        }
    }
}