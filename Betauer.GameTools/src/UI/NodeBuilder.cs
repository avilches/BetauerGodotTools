using System;
using Godot;

namespace Betauer.UI;

public abstract class NodeBuilder {
    public abstract Node Node { get; }
    
    public static NodeBuilder<T> Children<T>(T node) where T : Node {
        return new NodeBuilder<T>(node);
    }
    
    public NodeBuilder Add<T>(Action<T>? config = null) where T : Node {
        return Add((string)null!, config);
    }

    public NodeBuilder Add<T>(string name, Action<T>? config = null) where T : Node {
        var child = Activator.CreateInstance<T>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        return Add(child, config);
    }

    public NodeBuilder Add<T>(T child, Action<T>? config = null) where T : Node {
        Node.AddChild(child);
        config?.Invoke(child);
        return this;
    }

    public NodeBuilder Add(NodeBuilder child) {
        Node.AddChild(child.Node);
        return this;
    }

    public T Create<T>(string? name = null) where T : Node {
        var child = Activator.CreateInstance<T>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        Add(child);
        return child;
    }
}

public class NodeBuilder<T> : NodeBuilder where T : Node {
    public override T Node { get; }

    internal NodeBuilder(T node) {
        Node = node;
    }
}