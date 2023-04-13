using System;

namespace Betauer.Tools.FastReflection; 

public interface IGetterSetter : IMember {
    public IGetter Getter { get; }
    public ISetter Setter { get; }
    public void SetValue(object instance, object? value);
    public object? GetValue(object instance);
    bool CanSetValue(Type type) => Type.IsAssignableFrom(type);
    bool CanSetValue(object instance) => Type.IsInstanceOfType(instance);
}

public interface IGetterSetter<out T> : IGetter<T>, ISetter<T> {
    public T Attribute { get; }
}