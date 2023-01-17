using System;
using System.Linq;
using Godot;

namespace Betauer.Core.Nodes.Property; 

public class IndexedMultipleProperty<TProperty> : IndexedProperty<TProperty> {
    private readonly Type[] _types;
    private readonly NodePath[] _propertyNames;

    internal IndexedMultipleProperty(Type[] types, NodePath[] propertyNames) {
        _types = types;
        _propertyNames = propertyNames;
        if (types == null || propertyNames == null || types.Length != propertyNames.Length || types.Length == 0) {
            throw new Exception("types and propertyNames parameters should have more than 0 elements and same size");
        }
    }

    public override bool IsCompatibleWith(Node node) {
        return _types.Any(type => type.IsInstanceOfType(node));
    }

    public override NodePath GetIndexedPropertyName(Node node) {
        foreach (var (type, propertyName) in _types.Zip(_propertyNames, ValueTuple.Create)) {
            if (type.IsInstanceOfType(node)) return propertyName;
        }
        throw new NodeNotCompatibleWithPropertyException(
            $"IndexedMultipleProperty {this} is not compatible with target type {node.GetType().Name}");
    }

#if DEBUG        
    public override string ToString() {
        return
            $"IndexedMultipleProperty<{string.Join(",", _types.ToList())}>(\"{string.Join(",", _propertyNames.ToList())}\")";
    }
#endif
}