using System;

namespace Betauer.DI.Factory;

public interface ITemporal : IProxy {
    public string? Group { get; }
    public object Get();
    public bool HasValue();
    public void Remove();
}

public interface ITemporal<out T> : ITemporal where T : class {
    public new T Get();
    public event Action<T>? OnRemove;
}