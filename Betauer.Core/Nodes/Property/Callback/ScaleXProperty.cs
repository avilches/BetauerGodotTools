using Godot;

namespace Betauer.Nodes.Property.Callback {
    public class ScaleXProperty : Property<float> {
        internal ScaleXProperty() {
        }

        public override float GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.Scale.x,
                Control control => control.Scale.x,
                Node3D node3D => node3D.Scale.x,
                _ => throw new NodeNotCompatibleWithPropertyException($"Not ScaleX property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D) node.SetIndexed("scale:x", value);
            else if (node is Control control) control.SetIndexed("rect_scale:x", value);
            else if (node is Node3D node3D) node3D.SetIndexed("scale:x", value);
            else throw new NodeNotCompatibleWithPropertyException($"Not ScaleX property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control || node is Node2D || node is Node3D;
        }

        public override string ToString() {
            return "ScaleXProperty<float>(node2D: \"sale:x\", control: \"rect_scale:x\", node3D: \"scale:x\")";
        }
    }
}