using Godot;

namespace Betauer.Core.Nodes.Property.Callback {
    public class Rotate2DProperty : Property<float> {
        internal Rotate2DProperty() {
        }

        public override float GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.Rotation,
                Control control => control.Rotation,
                _ => throw new NodeNotCompatibleWithPropertyException($"Not rotation property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D node2D) node2D.Rotation = value;
            else if (node is Control control) control.Rotation = value;
            else throw new NodeNotCompatibleWithPropertyException($"Not rotation property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control or Node2D;
        }

        public override string ToString() {
            return "Rotate2DProperty<float>(node2D:\"Rotation\", control:\"Rotation\")";
        }
    }
}