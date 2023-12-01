using System;
using Betauer.Core;
using Betauer.Core.Nodes.Events;
using Godot;

namespace Betauer.Nodes;

public class ProcessEventHolder {
    public Node? Watcher { get; set; }
    public IProcessEvent Event { get; }
    public Action<double> ProcessAction { get; }
    public bool IsEnabled { get; private set; } = false;

    public ProcessEventHolder(IProcessEvent @event, Action<double> processAction) {
        Event = @event;
        ProcessAction = processAction;
        Enable();
    }

    public ProcessEventHolder(Node watcher, IProcessEvent @event, Action<double> processAction) {
        Watcher = watcher;
        Event = @event;
        ProcessAction = processAction;
        Enable();
    }

    public void Enable(bool enable = true) {
        if (IsEnabled == enable) return;
        if (enable) Event.OnProcess += OnProcess;
        else Event.OnProcess -= OnProcess;
        IsEnabled = enable;
    }

    public void Disable() {
        Enable(false);
    }

    private void OnProcess(double delta) {
        try {
            // TODO memory leak, what if the holder is disabled?
            ProcessAction(delta);
        } catch (Exception) {
            if (Watcher != null && !Watcher.IsInstanceValid()) Disable();
            else throw;
        }
    }
}