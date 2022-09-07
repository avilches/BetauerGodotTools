using System;
using Godot;

namespace Betauer.Nodes.Property {
    public class Rotate2DProperty : Property<float> {
        public override float GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.RotationDegrees,
                Control control => control.RectRotation,
                _ => throw new NodeNotCompatibleWithPropertyException($"Not rotation property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, float value) {
            if (node is Node2D node2D) node2D.RotationDegrees = value;
            else if (node is Control control) control.RectRotation = value;
            else throw new NodeNotCompatibleWithPropertyException($"Not rotation property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control || node is Node2D;
        }

        public override string ToString() {
            return "Rotate2DProperty<float>(node2D:\"RotationDegrees\", control:\"RectRotation\")";
        }
    }
}