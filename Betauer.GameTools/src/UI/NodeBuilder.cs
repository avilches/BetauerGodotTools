using System;
using Godot;

namespace Betauer.UI;

public class NodeBuilder {
    public Node Node { get; }

    public NodeBuilder(Node node) {
        Node = node;
    }

    public NodeBuilder Add<TC>(Action<TC>? config = null) where TC : Node {
        return Add((string)null!, config);
    }

    public NodeBuilder Add<TC>(string name, Action<TC>? config = null) where TC : Node {
        var child = Activator.CreateInstance<TC>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        return Add(child, config);
    }

    public NodeBuilder Add<TC>(TC child, Action<TC>? config = null) where TC : Node {
        Node.AddChild(child);
        config?.Invoke(child);
        return this;
    }

    public NodeBuilder Add(NodeBuilder child) {
        Node.AddChild(child.Node);
        return this;
    }

    public TC Create<TC>(string? name = null) where TC : Node {
        var child = Activator.CreateInstance<TC>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        Add(child);
        return child;
    }
}