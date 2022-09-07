using Godot;

namespace Betauer.Nodes.Property {
    public class IndexedProperty<TProperty> : IIndexedProperty<TProperty> {
        private readonly NodePath _propertyName;

        public IndexedProperty(NodePath propertyName) {
            _propertyName = propertyName;
        }

        public virtual TProperty GetValue(Node node) {
            return (TProperty)node.GetIndexed(GetIndexedPropertyName(node));
        }

        public virtual void SetValue(Node target, TProperty value) {
            target.SetIndexed(GetIndexedPropertyName(target), value);
        }

        public virtual bool IsCompatibleWith(Node node) => true;

        public virtual NodePath GetIndexedPropertyName(Node node) {
            return _propertyName;
        }

        public override string ToString() {
            return $"IndexedProperty<{typeof(TProperty).Name}>(\"{_propertyName}\")";
        }

        public static implicit operator IndexedProperty<TProperty>(string from) => new IndexedProperty<TProperty>(from);

        public static implicit operator string(IndexedProperty<TProperty> from) => from._propertyName;
    }
}