using System;

namespace Betauer.Tools.Reflection; 

public interface IGetterSetter : IMember {
    public void SetValue(object instance, object? value);
    public object? GetValue(object instance);
    public bool CanAssign(Type type);
}

public interface IGetterSetter<out T> : IGetter<T>, ISetter<T> where T : Attribute {
    public T Attribute { get; }
}