using Betauer.Core.Nodes;
using Veronenger.Persistent.Node;

namespace Veronenger.Persistent;

public abstract class Item {
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public string? Alias { get; internal set; }

    public abstract Godot.Node? Node { get; }
    public abstract void UnlinkNode();
}

public abstract class Item<T> : Item
    where T : class, ILinkableItem {
    public T? ItemNode { get; internal set; }

    public void LinkNode<TN>(TN itemNode) where TN : Godot.Node, T {
        ItemNode = itemNode;
        itemNode.LinkItem(this);
    }

    public override Godot.Node? Node => ItemNode as Godot.Node;

    public override void UnlinkNode() {
        Node!.RemoveFromParent();
        ItemNode = null;
    }
}