using System.Collections.Generic;
using Godot;

namespace Betauer.Nodes.Property {
    public static class IndexedSingleProperty {
        public static readonly Dictionary<string, IProperty> Cache = new Dictionary<string, IProperty>();

        public static IndexedSingleProperty<TProperty> Create<TProperty>(string propertyName) {
            if (Cache.TryGetValue(propertyName, out var property)) {
                return (IndexedSingleProperty<TProperty>)property;
            }
            var indexedSingleProperty = new IndexedSingleProperty<TProperty>(propertyName);
            Cache[propertyName] = indexedSingleProperty;
            return indexedSingleProperty;
        }
    }

    public class IndexedSingleProperty<TProperty> : IndexedProperty<TProperty> {
        private readonly NodePath _propertyName;

        internal IndexedSingleProperty(NodePath propertyName) {
            _propertyName = propertyName;
        }

        public override bool IsCompatibleWith(Node node) => true;

        public override NodePath GetIndexedPropertyName(Node node) => _propertyName;

        public override string ToString() {
            return $"IndexedSingleProperty<{typeof(TProperty).Name}>(\"{_propertyName}\")";
        }

        public static implicit operator IndexedSingleProperty<TProperty>(string from) => IndexedSingleProperty.Create<TProperty>(from);

        public static implicit operator string(IndexedSingleProperty<TProperty> from) => from._propertyName;
    }
}