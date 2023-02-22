using System;
using Godot;

namespace Betauer.Nodes;

public class ProcessHandler : BaseHandler, IProcessHandler {
    private readonly Action<double> _delegate;

    public ProcessHandler(Action<double>? @delegate, Node.ProcessModeEnum processMode, string? name = null) : base(
        processMode, name) {
        _delegate = @delegate;
    }

    public void Handle(double delta) {
        _delegate(delta);
    }
}