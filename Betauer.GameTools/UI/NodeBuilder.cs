using System;
using Godot;

namespace Betauer.UI {
    public static class NodeBuilderExtensions {

        public static NodeBuilder Child<T>(this Node parent, Action<T>? config = null) where T : Node {
            var child = Activator.CreateInstance<T>();
            return Child(parent, child, config);
        }

        public static NodeBuilder Child<T>(this Node parent, T child, Action<T>? config = null) where T : Node {
            var nodeBuilderParent = new NodeBuilder(null, parent);
            return new NodeBuilder(nodeBuilderParent, child, config != null ? (node) => config((T)node) : null);
        }
    }

    public class NodeBuilder {
        public NodeBuilder Parent { get; }
        public Node Node { get; }
        public NodeBuilder End() => Parent;

        internal NodeBuilder(NodeBuilder? parent, Node node, Action<Node>? config = null) {
            Parent = parent;
            Node = node;
            parent?.Node.AddChild(Node);
            config?.Invoke(Node);
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