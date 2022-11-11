using System;
using Betauer.Core.Nodes.Property;
using Godot;

namespace Betauer.Animation {
    public class NodeCallbackProperty<TProperty> : IProperty<TProperty> {
        private readonly Action<Node, TProperty> _action;

        public NodeCallbackProperty(Action<Node, TProperty> action) {
            _action = action;
        }

        public TProperty GetValue(Node node) {
            return default;
        }

        public void SetValue(Node node, TProperty value) {
            _action.Invoke(node, value);
        }

        public bool IsCompatibleWith(Node node) {
            return true;
        }

        public override string ToString() {
            return $"CallbackProperty<Node, {typeof(TProperty).Name}>";
        }
    }
}