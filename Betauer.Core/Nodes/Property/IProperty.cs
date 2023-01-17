using Godot;

namespace Betauer.Core.Nodes.Property; 

public interface IProperty {
    public Variant GetValue(Node node);
    public void SetValue(Node node, Variant value);
    public bool IsCompatibleWith(Node node);
}

public interface IProperty<TProperty> {
    public TProperty GetValue(Node node);
    public void SetValue(Node node, TProperty value);
    public bool IsCompatibleWith(Node node);
}