using Godot;

namespace Betauer.Nodes.Property {
    public interface IIndexedProperty {
        public NodePath GetIndexedPropertyName(Node node);
    }

    public abstract class IndexedProperty<TProperty> : IProperty, IProperty<TProperty>, IIndexedProperty {
        object IProperty.GetValue(Node node) {
            return node.GetIndexed(GetIndexedPropertyName(node));
        }

        public void SetValue(Node node, object value) {
            node.SetIndexed(GetIndexedPropertyName(node), (Variant)value);
        }

        public virtual TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedPropertyName(node)).Obj;
        }

        public virtual void SetValue(Node node, TProperty value) {
            node.SetIndexed(GetIndexedPropertyName(node), (Variant)(object)value);
        }

        public abstract bool IsCompatibleWith(Node node);

        public abstract NodePath GetIndexedPropertyName(Node node);
    }
}