using System;
using System.Linq;
using Godot;

namespace Betauer.Core.Restorer; 

public class PropertyNameRestorer : Restorer {
    private readonly Node _node;
    private Variant[] _values;
    private readonly NodePath[] _properties;

    public PropertyNameRestorer(Node node, params string[] properties) {
        _node = node;
        _properties = properties.Select(property => new NodePath(property)).ToArray();
    }

    protected override void DoSave() {
        _values = _properties.Select(property => _node.GetIndexed(property)).ToArray();
    }

    protected override void DoRestore() {
        foreach (var (property, value) in _properties.Zip(_values, ValueTuple.Create)) {
            _node.SetIndexed(property, value);
        }
    }
}