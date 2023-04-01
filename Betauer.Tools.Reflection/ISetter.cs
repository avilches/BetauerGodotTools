using System;

namespace Betauer.Tools.Reflection; 

public interface ISetter : IMember {
    public void SetValue(object instance, object? value);
    bool CanSetValue(Type type) => Type.IsAssignableFrom(type);
    bool CanSetValue(object instance) => Type.IsInstanceOfType(instance);
}

public interface ISetter<out T> : ISetter {
    public T SetterAttribute { get; }
}