using System;
using Godot;

namespace Betauer.UI {
    public static class NodeBuilderExtensions {
        public static NodeBuilder<T> Scene<T>(this T node, Action<T>? config = null) where T : Node {
            return new NodeBuilder<T>(node, config);
        }
    }

    public class NodeBuilder<T> where T : Node {
        public T Node { get; }

        public NodeBuilder(Action<T>? config = null) {
            Node = Activator.CreateInstance<T>();
            config?.Invoke(Node);
        }

        public NodeBuilder(T node, Action<T>? config = null) {
            Node = node;
            config?.Invoke(Node);
        }

        public NodeBuilder(Node parent, T node, Action<T>? config = null) {
            Node = node;
            parent.AddChild(Node);
            config?.Invoke(Node);
        }

        public NodeBuilder<T> AddChild<TC>(Action<TC>? config = null) where TC : Node {
            var child = Activator.CreateInstance<TC>();
            return AddChild(child, config);
        }

        public NodeBuilder<T> AddChild<TC>(TC child, Action<TC>? config = null) where TC : Node {
            Node.AddChild(child);
            config?.Invoke(child);
            return this;
        }

        public NodeBuilder<TC> Child<TC>(Action<TC>? config = null) where TC : Node {
            return Child(Activator.CreateInstance<TC>(), config);
        }

        public NodeBuilder<TC> Child<TC>(TC child, Action<TC>? config = null) where TC : Node {
            return new NodeBuilder<TC>(Node, child, config);
        }

        public void _InternalAddChild(Node child, Action<Node>? config = null) {
            Node.AddChild(child);
            config?.Invoke(child);
        }
    }
}