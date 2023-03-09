using System;
using System.Collections.Generic;

namespace Betauer.Core.Pool.Basic;

/// <summary>
/// Returns the first element added (the oldest) from the pool
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseStackPool<T> : IBasicPool<T> {

    public readonly Stack<T> Pool = new();

    public T Get() {
        var element = Pool.Count == 0 ? Create() : Pool.Pop();
        OnGet(element);
        return element;
    }

    public void Return(T element) {
        OnReturn(element);
        Pool.Push(element);
    }

    public abstract T Create();

    public virtual void OnGet(T element) {
    }

    public virtual void OnReturn(T element) {
    }
}

public class StackPool<T> : BaseStackPool<T> {

    private readonly Func<T> _factory;

    public event Action<T>? OnGetEvent;
    public event Action<T>? OnReturnEvent;
    
    public StackPool(Func<T> factory) {
        _factory += factory;
    }

    public override T Create() {
        return _factory.Invoke();
    }

    public override void OnGet(T element) {
        OnGetEvent?.Invoke(element);
    }

    public override void OnReturn(T element) {
        OnReturnEvent?.Invoke(element);
    }
}