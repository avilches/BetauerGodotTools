using System;

namespace Betauer.Tools.Reflection; 

public interface ISetter : IMember {
    public void SetValue(object instance, object? value);
    bool CanAssign(Type type);
}

public interface ISetter<out T> : ISetter where T : Attribute {
    public T SetterAttribute { get; }
}