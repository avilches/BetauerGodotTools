using System;
using Godot;

namespace Betauer.Nodes.Property {
    public class ScaleXProperty : Property<float> {
        public override float GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.Scale.x,
                Control control => control.RectScale.x,
                Spatial spatial => spatial.Scale.x,
                _ => throw new NodeNotCompatibleWithPropertyException($"Not ScaleX property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D) node.SetIndexed("scale:x", value);
            else if (node is Control control) control.SetIndexed("rect_scale:x", value);
            else if (node is Spatial spatial) spatial.SetIndexed("scale:x", value);
            else throw new NodeNotCompatibleWithPropertyException($"Not ScaleX property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control || node is Node2D || node is Spatial;
        }

        public override string ToString() {
            return "ScaleXProperty<float>(node2D: \"sale:x\", control: \"rect_scale:x\", spatial: \"scale:x\")";
        }
    }
}