using Godot;

namespace Betauer.Core.Nodes.Property.Callback {
    public class ScaleZProperty : Property<float> {
        internal ScaleZProperty() {
        }

        public override float GetValue(Node node) {
            return node switch {
                Node3D node3D => node3D.Scale.z,
                _ => throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node3D) node.SetIndexed("scale:z", value);
            else throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Node3D;
        }

        public override string ToString() {
            return "ScaleZProperty<float>(node3D: \"scale:z\")";
        }
    }
}