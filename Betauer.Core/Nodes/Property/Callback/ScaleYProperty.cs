using Godot;

namespace Betauer.Core.Nodes.Property.Callback {
    public class ScaleYProperty : Property<float> {
        internal ScaleYProperty() {
        }

        public override float GetValue(Node node) {
            return node switch {
                Control control => control.Scale.y,
                Node2D node2D => node2D.Scale.y,
                Node3D node3D => node3D.Scale.y,
                _ => throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D) node.SetIndexed("scale:y", value);
            else if (node is Control control) control.SetIndexed("rect_scale:y", value);
            else if (node is Node3D node3D) node3D.SetIndexed("scale:y", value);
            else throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Node3D || node is Control || node is Node2D;
        }

        public override string ToString() {
            return "ScaleYProperty<float>(node2D: \"sale:y\", control: \"rect_scale:y\", node3D: \"scale:y\")";
        }
    }
}