using Godot;

namespace Betauer.Nodes.Property {
    public interface IProperty {
    }

    public interface IProperty<TProperty> : IProperty {
        public TProperty GetValue(Node node);
        public void SetValue(Node node, TProperty value);
        public bool IsCompatibleWith(Node node);
    }

    public interface IIndexedProperty<TProperty> : IProperty<TProperty> {
        public NodePath GetIndexedPropertyName(Node node);
    }
}