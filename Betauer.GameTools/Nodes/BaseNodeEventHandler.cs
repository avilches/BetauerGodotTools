using Godot;

namespace Betauer.Nodes;

public abstract class BaseNodeEventHandler : BaseEventHandler {
    public Node Node { get; }

    protected BaseNodeEventHandler(Node? node, Node.ProcessModeEnum processMode) : base(node.Name, processMode) {
        Node = node;
    }

    public override bool IsEnabled(bool isTreePaused) {
        return base.IsEnabled(isTreePaused) && (Node == null || Node.IsInsideTree());
    }

    // Node can be null, so the Event will last forever
    public override bool IsDestroyed => base.IsDestroyed || (Node != null && !Godot.GodotObject.IsInstanceValid(Node));
}