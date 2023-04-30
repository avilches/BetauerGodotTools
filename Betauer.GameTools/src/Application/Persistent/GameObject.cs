using Betauer.Core.Nodes;

namespace Betauer.Application.Persistent;

public abstract class GameObject {
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public string? Alias { get; internal set; }

    public abstract void OnRemove();
}

public abstract class GameObject<T> : GameObject
    where T : Godot.Node {
    public T? Node { get; internal set; }

    public void LinkNode<TN>(TN node) where TN : Godot.Node, T {
        Node = node;
        if (node is INodeWithGameObject nodeItem) {
            nodeItem.GameObject = this;
        } 
    }

    public override void OnRemove() {
        UnlinkNode();
    }

    public void UnlinkNode() {
        if (Node is INodeWithGameObject nodeItem) {
            nodeItem.GameObject = null;
        } 
        Node!.RemoveFromParent();
        Node = null;
    }
}