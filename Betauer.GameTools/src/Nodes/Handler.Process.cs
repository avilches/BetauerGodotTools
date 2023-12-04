using System;

namespace Betauer.Nodes;

public class ProcessHandler : BaseHandler, IProcessHandler {
    private readonly Action<double> _delegate;

    public ProcessHandler(Action<double>? @delegate, string? name = null) : base(name) {
        _delegate = @delegate;
    }

    public void Handle(double delta) {
        _delegate(delta);
    }
}