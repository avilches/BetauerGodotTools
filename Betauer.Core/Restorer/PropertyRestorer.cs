using System;
using System.Linq;
using Betauer.Nodes.Property;
using Godot;

namespace Betauer.Restorer {
    public class PropertyRestorer : Restorer {
        private readonly Node _node;
        private readonly IProperty[] _properties;

        private object[] _values;

        public PropertyRestorer(Node node, params IProperty[] properties) {
            _node = node;
            _properties = properties;
        }

        protected override void DoSave() {
            _values = _properties.Select(property => property.GetValue(_node)).ToArray();                
        }

        protected override void DoRestore() {
            foreach (var (property, value) in _properties.Zip(_values, ValueTuple.Create)) {
                property.SetValue(_node, value);
            }
        }
    }
}