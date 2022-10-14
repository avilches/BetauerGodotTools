using System;
using Godot;

namespace Betauer.UI {
    public class NodeBuilder<T> where T : Node {
        private readonly T _node;
        private readonly Action<T>? _config;

        public NodeBuilder(Action<T>? config = null) {
            _node = Activator.CreateInstance<T>();
            _config = config;
        }

        public NodeBuilder(T node, Action<T>? config = null) {
            _node = node;
            _config = config;
        }

        public NodeBuilder<T> Child<TC>(Action<TC>? config = null) where TC : Node {
            var child = Activator.CreateInstance<TC>();
            return Child(child, config);
        }

        public NodeBuilder<T> Child<TC>(TC child, Action<TC>? config = null) where TC : Node {
            _node.AddChild(child);
            config?.Invoke(child);
            return this;
        }

        public NodeBuilder<T> Child<TC>(NodeBuilder<TC> nodeBuilder) where TC : Node {
            nodeBuilder.SetParent(_node);
            return this;
        }

        public NodeBuilder<T> SetParent(Node parent) {
            parent.AddChild(_node);
            _config?.Invoke(_node);
            return this;
        }
    }
}