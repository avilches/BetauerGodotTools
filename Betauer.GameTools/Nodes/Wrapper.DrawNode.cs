using Godot;

namespace Betauer.Nodes;

public class DrawNodeWrapper : IDrawHandler {
    public Node Node { get; }
    public string Name { get; }
    private readonly IDrawHandler _delegate;
    
    public DrawNodeWrapper(Node node, IDrawHandler @delegate, string? name = null) {
        Node = node;
        Name = name ?? @delegate.Name ?? node.Name;
        _delegate = @delegate;
    }

    public void Handle(CanvasItem canvas) => _delegate.Handle(canvas);

    public void Disable() => _delegate.Disable();
    public void Enable() => _delegate.Enable();
    public void Destroy() => _delegate.Destroy();
    public bool IsEnabled(bool isTreePaused) => _delegate.IsEnabled(isTreePaused) && Node.IsInsideTree();
    public bool IsDestroyed => _delegate.IsDestroyed && !GodotObject.IsInstanceValid(Node);
}