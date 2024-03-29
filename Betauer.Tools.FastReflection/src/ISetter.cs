using System;

namespace Betauer.Tools.FastReflection; 

public interface ISetter : IMember {
    public void SetValue(object instance, object? value);
    bool CanSetValue(Type type) => MemberType.IsAssignableFrom(type);
    bool CanSetValue(object instance) => MemberType.IsInstanceOfType(instance);
}

public interface ISetter<out T> : ISetter {
    public T SetterAttribute { get; }
}