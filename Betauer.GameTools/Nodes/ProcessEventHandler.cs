using System;
using Godot;

namespace Betauer.Nodes;

public class ProcessEventHandler : BaseEventHandler, IProcessHandler {
    private readonly Action<double> _delegate;
    public ProcessEventHandler(string? name, Action<double>? @delegate, Node.ProcessModeEnum processMode) : base(name, processMode) {
        _delegate = @delegate;
    }

    public void Handle(double delta) {
        _delegate(delta);
    }
}