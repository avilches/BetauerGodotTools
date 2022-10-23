using System;
using Godot;

namespace Betauer.UI {
    public static class NodeBuilderExtensions {
        public static NodeBuilder Scene<T>(this T node, Action<T>? config = null) where T : Node {
            return new NodeBuilder(null, node, config != null ? (node) => config((T)node) : null);
        }
    }

    public class NodeBuilder {
        public NodeBuilder Parent { get; }
        public Node Node { get; }

        internal NodeBuilder(NodeBuilder? parent, Node node, Action<Node>? config = null) {
            Parent = parent;
            Node = node;
            parent?.AddChild(Node);
            config?.Invoke(Node);
        }

        public NodeBuilder AddChild<T>(Action<T>? config = null) where T : Node {
            var child = Activator.CreateInstance<T>();
            return AddChild(child, config);
        }

        public NodeBuilder AddChild<T>(T child, Action<T>? config = null) where T : Node {
            Node.AddChild(child);
            config?.Invoke(child);
            return this;
        }

        public NodeBuilder Child<T>(Action<T>? config = null) where T : Node {
            var child = Activator.CreateInstance<T>();
            return Child(child, config);
        }

        public NodeBuilder Child<T>(T child, Action<T>? config = null) where T : Node {
            return new NodeBuilder(this, child, config != null ? (node) => config((T)node) : null);
        }
    }
}