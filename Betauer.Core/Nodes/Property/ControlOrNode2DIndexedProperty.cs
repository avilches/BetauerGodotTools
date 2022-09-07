using System;
using Godot;

namespace Betauer.Nodes.Property {
    public class ControlOrNode2DIndexedProperty<TProperty> : IIndexedProperty<TProperty> {
        private readonly NodePath? _node2DProperty;
        private readonly NodePath? _controlProperty;

        public ControlOrNode2DIndexedProperty(NodePath node2DProperty, NodePath controlProperty) {
            _node2DProperty = node2DProperty;
            _controlProperty = controlProperty;
        }

        public TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedPropertyName(node));
        }

        public void SetValue(Node target, TProperty value) {
            target.SetIndexed(GetIndexedPropertyName(target), value);
        }

        public bool IsCompatibleWith(Node node) {
            return
                (node is Control && _controlProperty != null) ||
                (node is Node2D && _node2DProperty != null);
        }

        public NodePath GetIndexedPropertyName(Node node) {
            return node switch {
                Control _ when _controlProperty != null => _controlProperty,
                Node2D _ when _node2DProperty != null => _node2DProperty,
                _ => throw new NodeNotCompatibleWithPropertyException($"Property Node2D.{_node2DProperty} or Control.{_controlProperty} not compatible with target type {node.GetType().Name}")
            };
        }

        public override string ToString() {
            return $"ControlOrNode2DIndexedProperty<{typeof(TProperty).Name}>(node2D: \"{_node2DProperty}\", control: \"{_controlProperty}\")";
        }
    }
}