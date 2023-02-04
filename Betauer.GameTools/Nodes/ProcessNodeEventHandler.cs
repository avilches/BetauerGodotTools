using System;
using Godot;

namespace Betauer.Nodes;

public class ProcessNodeEventHandler : BaseNodeEventHandler, IProcessHandler {
    private readonly Action<double> _delegate;
    public ProcessNodeEventHandler(Node? node, Action<double> @delegate, Node.ProcessModeEnum processMode) : base(node, processMode) {
        _delegate = @delegate;
    }

    public void Handle(double delta) {
        _delegate(delta);
    }
}