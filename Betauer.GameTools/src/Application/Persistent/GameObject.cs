using System;
using Betauer.Core;
using Betauer.Core.Nodes;

namespace Betauer.Application.Persistent;

public abstract class GameObject {
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public string? Alias { get; internal set; }

    public abstract void New();
    public abstract void OnRemove();
    public abstract SaveObject CreateSaveObject();
    public abstract void Load(SaveObject saveObject);
}

public abstract class GameObject<TNode> : GameObject
    where TNode : Godot.Node {
    
    public TNode? Node { get; internal set; }
    
    protected abstract Type SaveObjectType { get; }

    public sealed override void Load(SaveObject saveObject) {
        if (!saveObject.GetType().IsAssignableTo(SaveObjectType)) {
            var type = saveObject.GetType().GetTypeName();
            var expected = SaveObjectType.GetTypeName();
            throw new Exception($"Wrong call to Load({type} saveObject). Expected type is: {expected}");
        }
        DoLoad(saveObject);
    }
    
    protected abstract void DoLoad(SaveObject saveObject);

    public void LinkNode<TN>(TN node) where TN : Godot.Node, TNode {
        Node = node;
        if (node is INodeGameObject nodeItem) {
            nodeItem.GameObject = this;
        } 
    }

    public override void OnRemove() {
        UnlinkNode();
    }

    public void UnlinkNode() {
        if (Node is INodeGameObject nodeItem) {
            nodeItem.GameObject = null;
        } 
        Node!.RemoveFromParent();
        Node = null;
    }
}