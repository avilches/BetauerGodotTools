using System;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Godot;

namespace Betauer.Nodes;

public class InputEventHolder {
    public Node? Watcher { get; set; }
    public IInputEvent Event { get; }
    public Action<InputEvent> InputEventAction { get; }
    public bool IsEnabled { get; private set; } = false;

    public InputEventHolder(IInputEvent @event, Action<InputEvent> inputEventAction) {
        Event = @event;
        InputEventAction = inputEventAction;
        Enable();
    }

    public InputEventHolder(Node watcher, IInputEvent @event, Action<InputEvent> inputEventAction) {
        Watcher = watcher;
        Event = @event;
        InputEventAction = inputEventAction;
        Enable();
    }

    public void Enable(bool enable = true) {
        if (IsEnabled == enable) return;
        if (enable) Event.OnInput += OnInputEvent;
        else Event.OnInput -= OnInputEvent;
        IsEnabled = enable;
    }

    public void Disable() {
        Enable(false);
    }

    private void OnInputEvent(InputEvent @event) {
        try {
            // TODO memory leak, what if the input is never catch?
            InputEventAction(@event);
        } catch (Exception) {
            if (Watcher != null && !Watcher.IsInstanceValid()) Disable();
            else throw;
        }
    }
}