using Godot;

namespace Betauer.Core.Nodes.Property.Callback; 

public class ScaleYProperty : Property<float> {
    internal ScaleYProperty() {
    }

    public override float GetValue(Node node) {
        return node switch {
            Control control => control.Scale.Y,
            Node2D node2D => node2D.Scale.Y,
            Node3D node3D => node3D.Scale.Y,
            _ => throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}")
        };
    }

    public override void SetValue(Node node, float value) {
        if (node is Node2D) node.SetIndexed("scale:y", value);
        else if (node is Control control) control.SetIndexed("scale:y", value);
        else if (node is Node3D node3D) node3D.SetIndexed("scale:y", value);
        else throw new NodeNotCompatibleWithPropertyException($"No ScaleY property for node type {node.GetType()}");
    }

    public override bool IsCompatibleWith(Node node) {
        return node is Node3D or Control or Node2D;
    }

    public override string ToString() {
        return "ScaleYProperty<float>(node2D: \"sale:y\", control: \"scale:y\", node3D: \"scale:y\")";
    }
}