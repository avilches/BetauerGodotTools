using System;
using Godot;

namespace Betauer.Nodes.Property {
    public abstract class ComputedProperty<TProperty> : IProperty<TProperty> {
        protected readonly IProperty<TProperty> Property;
        protected readonly Node Node;

        protected ComputedProperty(Node node, IProperty<TProperty> property) {
            Node = node;
            Property = property;
        }

        public TProperty GetValue(Node node) {
            if (!IsCompatibleWith(node))
                throw new NodeNotCompatibleWithPropertyException($"ComputedProperty {this} is not compatible with target type {node.GetType().Name}");                
            return Property.GetValue(node);
        }

        public void SetValue(Node node, TProperty value) {
            if (!IsCompatibleWith(node))
                throw new NodeNotCompatibleWithPropertyException($"ComputedProperty {this} is not compatible with target type {node.GetType().Name}");                
            var computeValue = ComputeValue(value);
            Property.SetValue(node, computeValue);
        }

        protected abstract TProperty ComputeValue(TProperty value);

        public abstract bool IsCompatibleWith(Node node);
    }
}