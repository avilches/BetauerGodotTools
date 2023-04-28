using Betauer.Core.Nodes;

namespace Veronenger.Persistent;

public abstract class Item {
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public string? Alias { get; internal set; }

    public abstract void OnRemove();
}

public interface INodeWithItem {
    public Item Item { get; set; }
}

public abstract class Item<T> : Item
    where T : Godot.Node {
    public T? Node { get; internal set; }

    public void LinkNode<TN>(TN node) where TN : Godot.Node, T {
        Node = node;
        if (node is INodeWithItem nodeItem) {
            nodeItem.Item = this;
        } 
    }

    public override void OnRemove() {
        UnlinkNode();
    }

    public void UnlinkNode() {
        if (Node is INodeWithItem nodeItem) {
            nodeItem.Item = null;
        } 
        Node!.RemoveFromParent();
        Node = null;
    }
}