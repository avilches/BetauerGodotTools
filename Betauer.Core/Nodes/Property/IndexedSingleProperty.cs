using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Nodes.Property; 

public static class IndexedSingleProperty {
    public static readonly Dictionary<(string, Type), IProperty> Cache = new();

    public static IndexedSingleProperty<TProperty> Create<[MustBeVariant] TProperty>(string propertyName, Type? type) {
        type ??= typeof(Node);
        if (!type.IsAssignableTo(typeof(Node)))
            throw new ArgumentException($"{type.Name} should be Node or inherit from Node");

        var key = (propertyName, type);
        if (Cache.TryGetValue(key, out var property)) {
            return (IndexedSingleProperty<TProperty>)property;
        }
        var indexedSingleProperty = new IndexedSingleProperty<TProperty>(propertyName, type);
        Cache[key] = indexedSingleProperty;
        return indexedSingleProperty;
    }
}

public class IndexedSingleProperty<[MustBeVariant] TProperty> : IndexedProperty<TProperty> {
    private readonly NodePath _propertyName;
    private readonly Type _type;
        
    internal IndexedSingleProperty(NodePath propertyName, Type? type) {
        _propertyName = propertyName;
        _type = type;
    }

    public override bool IsCompatibleWith(Node node) => node.GetType().IsAssignableTo(_type);

    public override NodePath GetIndexedPropertyName(Node node) => _propertyName;

    public override string ToString() {
        return $"IndexedSingleProperty<{typeof(TProperty).Name}>(\"{_propertyName}\")";
    }

    public static implicit operator IndexedSingleProperty<TProperty>(string from) => IndexedSingleProperty.Create<TProperty>(from, typeof(Node));

    public static implicit operator string(IndexedSingleProperty<TProperty> from) => from._propertyName;
}