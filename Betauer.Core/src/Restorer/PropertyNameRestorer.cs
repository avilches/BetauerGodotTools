using System;
using System.Linq;
using Godot;

namespace Betauer.Core.Restorer; 

public class PropertyNameRestorer : Restorer {
    public enum Behaviour {
        SaveSilently,
        SaveAndLog,
        LogOnly,
        DoNothing,
    }
    
    public static Behaviour? OverrideBehaviour = null;

    public Behaviour CurrentBehaviour { get; set; } = Behaviour.SaveSilently;
    private readonly Node _node;
    private Variant[] _values;
    private readonly NodePath[] _properties;

    public PropertyNameRestorer(Node node, params string[] properties) {
        _node = node;
        _properties = properties.Select(property => new NodePath(property)).ToArray();
    }

    public PropertyNameRestorer(Node node, params StringName[] properties) {
        _node = node;
        _properties = properties.Select(property => new NodePath(property)).ToArray();
    }

    public PropertyNameRestorer(Node node, params NodePath[] properties) {
        _node = node;
        _properties = properties;
    }
    
    private Behaviour RealBehaviour => OverrideBehaviour ?? CurrentBehaviour;

    protected override void DoSave() {
        if (RealBehaviour == Behaviour.DoNothing) return;
        _values = _properties.Select(property => {
            if (RealBehaviour is Behaviour.SaveAndLog or Behaviour.LogOnly) {
                Console.WriteLine($"[PropertyNameRestorer] Saving '{_node.Name}' {_node.GetType().GetTypeName()}.{property}: {_node.GetIndexed(property)}");
            }
            return _node.GetIndexed(property);
        }).ToArray();
    }

    protected override void DoRestore() {
        if (RealBehaviour == Behaviour.DoNothing) return;
        foreach (var (property, value) in _properties.Zip(_values, ValueTuple.Create)) {
            if (RealBehaviour is Behaviour.SaveAndLog or Behaviour.LogOnly) {
                if (!VariantHelper.EqualsVariant(_node.GetIndexed(property), value)) {
                    var backup = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"[PropertyNameRestorer] Restoring '{_node.Name}' {_node.GetType().GetTypeName()}.{property} ({_node.GetIndexed(property)}) to original: {value}");
                    Console.ForegroundColor = backup;
                }
            }
            if (RealBehaviour != Behaviour.LogOnly) {
                _node.SetIndexed(property, value);
            }
        }
    }
}