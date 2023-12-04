using Godot;

namespace Betauer.Nodes;

public class InputEventNodeHandler : BaseNodeHandler, IInputEventHandler {
    private readonly IInputEventHandler _processDelegate;

    public InputEventNodeHandler(Node node, IInputEventHandler @delegate, string? name = null) : base(node, name) {
        _processDelegate = @delegate;
    }

    public void Handle(InputEvent e) => _processDelegate.Handle(e);
}