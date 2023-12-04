using Godot;

namespace Betauer.Nodes;

public class ProcessNodeHandler : BaseNodeHandler, IProcessHandler {
    private readonly IProcessHandler _processDelegate;

    public ProcessNodeHandler(Node node, IProcessHandler @delegate, string? name = null) : base(node, name) {
        _processDelegate = @delegate;
    }

    public void Handle(double delta) => _processDelegate.Handle(delta);
}