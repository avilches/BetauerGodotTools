using System;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Godot;

namespace Betauer.Nodes;

public class PhysicsProcessEventHolder {
    public Node? Watcher { get; set; }
    public IPhysicsProcessEvent Event { get; }
    public Action<double> PhysicsProcessAction { get; }
    public bool IsEnabled { get; private set; } = false;

    public PhysicsProcessEventHolder(IPhysicsProcessEvent @event, Action<double> physicsProcessAction) {
        Event = @event;
        PhysicsProcessAction = physicsProcessAction;
        Enable();
    }

    public PhysicsProcessEventHolder(Node watcher, IPhysicsProcessEvent @event, Action<double> physicsProcessAction) {
        Watcher = watcher;
        Event = @event;
        PhysicsProcessAction = physicsProcessAction;
        Enable();
    }

    public void Enable(bool enable = true) {
        if (IsEnabled == enable) return;
        if (enable) Event.OnPhysicsProcess += OnPhysicsProcess;
        else Event.OnPhysicsProcess -= OnPhysicsProcess;
        IsEnabled = enable;
    }

    public void Disable() {
        Enable(false);
    }

    private void OnPhysicsProcess(double delta) {
        try {
            PhysicsProcessAction(delta);
        } catch (Exception) {
            if (Watcher != null && !Watcher.IsInstanceValid()) Disable();
            else throw;
        }
    }
}