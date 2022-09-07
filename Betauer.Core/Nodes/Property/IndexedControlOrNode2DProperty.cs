using System;
using Godot;

namespace Betauer.Nodes.Property {
    public class IndexedControlOrNode2DProperty<TProperty> : IndexedProperty<TProperty> {
        private readonly NodePath? _node2DProperty;
        private readonly NodePath? _controlProperty;

        public IndexedControlOrNode2DProperty(NodePath node2DProperty, NodePath controlProperty) {
            _node2DProperty = node2DProperty;
            _controlProperty = controlProperty;
        }

        public override bool IsCompatibleWith(Node node) {
            return
                (node is Control && _controlProperty != null) ||
                (node is Node2D && _node2DProperty != null);
        }

        public override NodePath GetIndexedPropertyName(Node node) {
            return node switch {
                Control _ when _controlProperty != null => _controlProperty,
                Node2D _ when _node2DProperty != null => _node2DProperty,
                _ => throw new NodeNotCompatibleWithPropertyException($"Property Node2D.{_node2DProperty} or Control.{_controlProperty} not compatible with node type {node.GetType().Name}")
            };
        }

        public override string ToString() {
            return $"IndexedControlOrNode2DProperty<{typeof(TProperty).Name}>(node2D: \"{_node2DProperty}\", control: \"{_controlProperty}\")";
        }
    }
}