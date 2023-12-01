using System;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Godot;

namespace Betauer.Nodes;

public class ShortcutInputEventHolder {
    public Node? Watcher { get; set; }
    public IShortcutInputEvent Event { get; }
    public Action<InputEvent> ShortcutInputEventAction { get; }
    public bool IsEnabled { get; private set; } = false;

    public ShortcutInputEventHolder(IShortcutInputEvent @event, Action<InputEvent> shortcutInputEventAction) {
        Event = @event;
        ShortcutInputEventAction = shortcutInputEventAction;
        Enable();
    }

    public ShortcutInputEventHolder(Node watcher, IShortcutInputEvent @event, Action<InputEvent> shortcutInputEventAction) {
        Watcher = watcher;
        Event = @event;
        ShortcutInputEventAction = shortcutInputEventAction;
        Enable();
    }

    public void Enable(bool enable = true) {
        if (IsEnabled == enable) return;
        if (enable) Event.OnShortcutInput += OnShortcutInputEvent;
        else Event.OnShortcutInput -= OnShortcutInputEvent;
        IsEnabled = enable;
    }

    public void Disable() {
        Enable(false);
    }

    private void OnShortcutInputEvent(InputEvent @event) {
        try {
            ShortcutInputEventAction(@event);
        } catch (Exception) {
            if (Watcher != null && !Watcher.IsInstanceValid()) Disable();
            else throw;
        }
    }
}