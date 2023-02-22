using Godot;

namespace Betauer.Nodes;

public class InputEventNodeWrapper : IInputEventHandler {
    public Node Node { get; }
    public string Name { get; }
    private readonly IInputEventHandler _delegate;
    
    public InputEventNodeWrapper(Node node, IInputEventHandler @delegate, string? name = null) {
        Node = node;
        Name = name ?? @delegate.Name ?? node.Name;
        _delegate = @delegate;
    }

    public void Handle(InputEvent e) => _delegate.Handle(e);

    public void Disable() => _delegate.Disable();
    public void Enable() => _delegate.Enable();
    public void Destroy() => _delegate.Destroy();
    public bool IsEnabled(bool isTreePaused) => _delegate.IsEnabled(isTreePaused) && Node.IsInsideTree();
    public bool IsDestroyed => _delegate.IsDestroyed && !GodotObject.IsInstanceValid(Node);
}