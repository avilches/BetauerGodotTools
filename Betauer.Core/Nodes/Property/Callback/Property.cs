using Godot;

namespace Betauer.Core.Nodes.Property.Callback; 

public abstract class Property<TProperty> : IProperty<TProperty> {
    public abstract void SetValue(Node node, TProperty value);
    public abstract TProperty GetValue(Node node);
    public abstract bool IsCompatibleWith(Node node);
}