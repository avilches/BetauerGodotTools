using Godot;

namespace Betauer.Nodes.Property.Callback {
    public class Scale3DProperty : Property<Vector3> {
        internal Scale3DProperty() {
        }

        public override Vector3 GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale,
                _ => throw new NodeNotCompatibleWithPropertyException($"Not Scale3D property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, Vector3 value) {
            if (node is Spatial spatial) spatial.Scale = value;
            else throw new NodeNotCompatibleWithPropertyException($"Not Scale3D property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Spatial;
        }

        public override string ToString() {
            return "Scale3DProperty<Vector3>(spatial: \"Scale\")";
        }
    }
}