using Betauer.Core.Nodes;

namespace Betauer.Application.Persistent;

public abstract class GameObject {
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public string? Alias { get; internal set; }

    public abstract void OnInitialize();
    public abstract void OnRemove();
    public abstract void OnLoad(SaveObject saveObject);

    public abstract SaveObject CreateSaveObject();
}

public abstract class GameObject<TNode> : GameObject
    where TNode : Godot.Node {
    public TNode? Node { get; internal set; }

    public override void OnRemove() => UnlinkNode();

    public void LinkNode<TN>(TN node) where TN : Godot.Node, TNode {
        Node = node;
        if (node is INodeGameObject nodeItem) {
            nodeItem.GameObject = this;
        }
    }

    public void UnlinkNode() {
        if (Node is INodeGameObject nodeItem) {
            nodeItem.GameObject = null;
        }
        Node!.RemoveFromParent();
        Node = null;
    }
}