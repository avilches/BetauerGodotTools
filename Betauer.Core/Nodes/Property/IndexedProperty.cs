using Godot;

namespace Betauer.Core.Nodes.Property; 

public interface IIndexedProperty {
    public NodePath GetIndexedPropertyName(Node node);
}

public abstract class IndexedProperty<[MustBeVariant] TProperty> : IProperty, IProperty<TProperty>, IIndexedProperty {
    Variant IProperty.GetValue(Node node) {
        return node.GetIndexed(GetIndexedPropertyName(node));
    }

    public void SetValue(Node node, Variant value) {
        node.SetIndexed(GetIndexedPropertyName(node), value);
    }

    public virtual TProperty GetValue(Node node) {
        return node.GetIndexed(GetIndexedPropertyName(node)).As<TProperty>();
    }

    public virtual void SetValue(Node node, TProperty value) {
        SetValue(node, Variant.From(value));
    }

    public abstract bool IsCompatibleWith(Node node);

    public abstract NodePath GetIndexedPropertyName(Node node);
}