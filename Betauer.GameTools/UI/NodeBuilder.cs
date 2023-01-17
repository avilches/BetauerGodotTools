using System;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.UI;

public static class NodeBuilderExtensions {

    public static NodeBuilder<TParent> NodeBuilder<TParent>(this TParent parent) where TParent : Node {
        return new NodeBuilder<TParent>(parent);
    }
}

public interface INode {
    public Node Node { get;  }
}

public class NodeBuilder<T> : INode where T : Node {
    public T TypedNode { get; protected set; }
    public Node Node => TypedNode;

    public NodeBuilder(T typedNode) {
        TypedNode = typedNode;
    }

    public NodeBuilder<NodeBuilder<T>, TC> Child<TC>(string? name = null) where TC : Node {
        var child = Activator.CreateInstance<TC>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        return Child(child);
    }

    public NodeBuilder<NodeBuilder<T>, TC> Child<TC>(TC child) where TC : Node {
        return new NodeBuilder<NodeBuilder<T>, TC>(this, child);
    }
    
    public NodeBuilder<T> Config(Action<T> config) {
        config.Invoke(TypedNode);
        return this;
    }

    public NodeBuilder<NodeBuilder<T>, Button> Button(string label, Action<Button> action) {
        return Button<Button>(label, action);
    }

    public NodeBuilder<NodeBuilder<T>, Button> Button(string label, Action action) {
        return Button<Button>(label, action);
    }

    public NodeBuilder<NodeBuilder<T>, TButton> Button<TButton>(string label, Action action) where TButton : Button {
        return Button<TButton>(label, (_) => action());
    }

    public NodeBuilder<NodeBuilder<T>, TButton> Button<TButton>(string label, Action<TButton> action) where TButton : Button {
        var b = Activator.CreateInstance<TButton>();
        b.Text = label;
        b.OnPressed(() => action(b));
        return Child(b);
    }

    public NodeBuilder<NodeBuilder<T>, Label> Label(string label) {
        return Child(new Label {
            Text = label
        });
    }
    
    public NodeBuilder<NodeBuilder<T>, ToggleButton> ToggleButton(string label, Action action, Func<bool> pressedId) {
        return ToggleButton(label, (_) => action(), pressedId);
    }

    public NodeBuilder<NodeBuilder<T>, ToggleButton> ToggleButton(string label, Action<ToggleButton> action, Func<bool> pressedId) {
        var b = new ToggleButton();
        b.Text = label;
        b.PressedIf = pressedId;
        // TODO use Button group instead
        b.OnPressed(() => {
            action(b);
            b.Refresh();
        });
        return Child(b);
    }
}

public class NodeBuilder<TNodeBuilder, T> : INode where T : Node where TNodeBuilder : INode {
    public T TypedNode { get; protected set; }
    public Node Node => TypedNode;

    public TNodeBuilder Parent { get; protected set; }
    public TNodeBuilder End() => Parent;

    internal NodeBuilder(TNodeBuilder parent, T typedNode, Action<Node>? config = null) {
        Parent = parent;
        TypedNode = typedNode;
        Parent.Node.AddChild(TypedNode);
        config?.Invoke(TypedNode);
    }

    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, TC> Child<TC>(string? name = null) where TC : Node {
        var child = Activator.CreateInstance<TC>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        return Child(child);
    }

    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, TC> Child<TC>(TC child) where TC : Node {
        return new NodeBuilder<NodeBuilder<TNodeBuilder, T>, TC>(this, child);
    }

    public NodeBuilder<TNodeBuilder, T> Config(Action<T> config) {
        config.Invoke(TypedNode);
        return this;
    }

    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, Button> Button(string label, Action action) {
        var b = new Button();
        b.Text = label;
        b.OnPressed(action);
        return Child(b);
    }
    
    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, TButton> Button<TButton>(string label, Action action) where TButton : Button {
        var b = Activator.CreateInstance<TButton>();
        b.Text = label;
        b.OnPressed(action);
        return Child(b);
    }
    
}