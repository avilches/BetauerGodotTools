using Godot;
using Godot.NativeInterop;

namespace Betauer.Core.Nodes.Property {
    public interface IIndexedProperty {
        public NodePath GetIndexedPropertyName(Node node);
    }

    public abstract class IndexedProperty<TProperty> : IProperty, IProperty<TProperty>, IIndexedProperty {
        Variant IProperty.GetValue(Node node) {
            return node.GetIndexed(GetIndexedPropertyName(node));
        }

        public void SetValue(Node node, Variant value) {
            node.SetIndexed(GetIndexedPropertyName(node), value);
        }

        public virtual TProperty GetValue(Node node) {
            Variant indexed = node.GetIndexed(GetIndexedPropertyName(node));
            return indexed.As<TProperty>();
        }

        public virtual void SetValue(Node node, TProperty value) {
            node.SetIndexed(GetIndexedPropertyName(node), Variant.From(value));
        }

        public abstract bool IsCompatibleWith(Node node);

        public abstract NodePath GetIndexedPropertyName(Node node);
    }
}