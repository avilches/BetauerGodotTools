using System;

namespace Betauer.Tools.Reflection; 

public interface IGetter : IMember {
    public object? GetValue(object instance);
}

public interface IGetter<out T> : IGetter {
    public T GetterAttribute { get; }
}