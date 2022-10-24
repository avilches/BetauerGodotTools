using System;
using Betauer.Signal;
using Godot;

namespace Betauer.UI {
    public static class NodeBuilderExtensions {

        public static NodeBuilder Child<T>(this Node parent) where T : Node {
            var child = Activator.CreateInstance<T>();
            return Child(parent, child);
        }

        public static NodeBuilder<T> Child<T>(this Node parent, T child) where T : Node {
            var nodeBuilderParent = new NodeBuilder<Node>(null, parent);
            return new NodeBuilder<T>(nodeBuilderParent, child);
        }
        
        public static NodeBuilder<Button> Button(this Node nodeBuilder, string label, Action action) {
            var b = new Button();
            b.Text = label;
            b.OnPressed(action);
            return nodeBuilder.Child(b);
        }
        
        public static NodeBuilder<Button> Button(this NodeBuilder nodeBuilder, string label, Action action) {
            var b = new Button();
            b.Text = label;
            b.OnPressed(action);
            return nodeBuilder.Child(b);
        }
    }

    public abstract class NodeBuilder {
        public NodeBuilder Parent { get; protected set; }
        public Node Node { get; protected set; }
        public NodeBuilder End() => Parent;

        public NodeBuilder<T> Child<T>() where T : Node {
            var child = Activator.CreateInstance<T>();
            return Child(child);
        }

        public NodeBuilder<T> Child<T>(T child) where T : Node {
            return new NodeBuilder<T>(this, child);
        }
    }

    public class NodeBuilder<TNode> : NodeBuilder where TNode : Node {
        internal NodeBuilder(NodeBuilder? parent, TNode node, Action<Node>? config = null) {
            Parent = parent;
            Node = node;
            parent?.Node.AddChild(Node);
            config?.Invoke(Node);
        }

        public NodeBuilder<TNode> Config(Action<TNode> config) {
            config.Invoke((TNode)Node);
            return this;
        }

    }
}