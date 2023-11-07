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

    public NodeBuilder<NodeBuilder<T>, TC> Child<TC>(Action<TC>? config = null) where TC : Node {
        return Child((string)null!, config);
    }

    public NodeBuilder<NodeBuilder<T>, TC> Child<TC>(string name, Action<TC>? config = null) where TC : Node {
        var child = Activator.CreateInstance<TC>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        return Child(child, config);
    }

    public NodeBuilder<NodeBuilder<T>, TC> Child<TC>(TC child, Action<TC>? config = null) where TC : Node {
        return new NodeBuilder<NodeBuilder<T>, TC>(this, child, config);
    }
    
    public T End(Action<T>? config = null) {
        config?.Invoke(TypedNode);
        return TypedNode;
    }

    public NodeBuilder<NodeBuilder<T>, Button> Button(string label, Action<Button> action) {
        return Button<Button>(label, action);
    }

    public NodeBuilder<NodeBuilder<T>, TButton> Button<TButton>(string label, Action<TButton> action) where TButton : Button {
        var b = Activator.CreateInstance<TButton>();
        b.ToggleMode = false;
        b.Text = label;
        b.Pressed += () => action(b);
        return Child(b);
    }

    public NodeBuilder<NodeBuilder<T>, Label> Label(string label) {
        return Child(new Label {
            Text = label
        });
    }
    
    public NodeBuilder<NodeBuilder<T>, Button> ToggleButton(string label, Action<Button> action, Func<bool> isPressed, ButtonGroup? group = null) {
        return ToggleButton<Button>(label, action, isPressed, group);
    }

    public NodeBuilder<NodeBuilder<T>, TButton> ToggleButton<TButton>(string label, Action<TButton> action, Func<bool> isPressed, ButtonGroup? group = null) where TButton : Button {
        var b = Activator.CreateInstance<TButton>();
        b.ToggleMode = true;
        b.Text = label;
        b.Pressed += () => action(b);
        b.ButtonGroup = group;
        b.OnReady(() => b.SetPressedNoSignal(isPressed()), true);
        return Child(b);
    }
}

public class NodeBuilder<TNodeBuilder, T> : INode where T : Node where TNodeBuilder : INode {
    public T TypedNode { get; protected set; }
    public Node Node => TypedNode;

    public TNodeBuilder Parent { get; protected set; }

    public TNodeBuilder End(Action<T>? config = null) {
        config?.Invoke(TypedNode);
        return Parent;
    }

    internal NodeBuilder(TNodeBuilder parent, T typedNode, Action<T>? config = null) {
        Parent = parent;
        TypedNode = typedNode;
        Parent.Node.AddChild(TypedNode);
        config?.Invoke(TypedNode);
    }

    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, TC> Child<TC>(Action<TC>? config = null) where TC : Node {
        return Child((string)null!, config);
    }

    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, TC> Child<TC>(string name, Action<TC>? config = null) where TC : Node {
        var child = Activator.CreateInstance<TC>();
        if (!string.IsNullOrWhiteSpace(name)) child.Name = name;
        return Child(child, config);
    }

    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, TC> Child<TC>(TC child, Action<TC>? config = null) where TC : Node {
        return new NodeBuilder<NodeBuilder<TNodeBuilder, T>, TC>(this, child, config);
    }

    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, Button> Button(string label, Action action) {
        return Button<Button>(label, action);
    }
    
    public NodeBuilder<NodeBuilder<TNodeBuilder, T>, TButton> Button<TButton>(string label, Action action) where TButton : Button {
        var b = Activator.CreateInstance<TButton>();
        b.Text = label;
        b.Pressed += action;
        return Child(b);
    }
    
}