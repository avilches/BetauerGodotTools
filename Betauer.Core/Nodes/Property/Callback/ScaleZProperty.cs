using Godot;

namespace Betauer.Nodes.Property.Callback {
    public class ScaleZProperty : Property<float> {
        internal ScaleZProperty() {
        }

        public override float GetValue(Node node) {
            return node switch {
                Spatial spatial => spatial.Scale.z,
                _ => throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Spatial) node.SetIndexed("scale:z", value);
            else throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Spatial;
        }

        public override string ToString() {
            return "ScaleZProperty<float>(spatial: \"scale:z\")";
        }
    }
}