using System;
using System.Collections.Generic;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI; 

public class ResolveContext {
    internal readonly Dictionary<(Type, string?), object> NewSingletonsCreated = new();
    internal readonly List<object> NewTransientsCreated = new();
    internal readonly Stack<Type> TransientNameStack = new();
    internal readonly Container Container;

    private readonly Action? _onEnd;

    public ResolveContext(Container container, Action? onEnd = null) {
        Container = container;
        _onEnd = onEnd;
    }

    internal bool TryGetSingletonFromCache(Type type, string? name, out object? instanceFound) {
        var key = name ?? type.FullName;
        return NewSingletonsCreated.TryGetValue((type, name), out instanceFound);
    }

    internal void AddSingleton(Type type, string? name, object instance) {
        NewSingletonsCreated[(type, name)] = instance;
    }

    // This stack avoid circular dependencies between transients
    internal void TryStartTransient(Type type, string? name) {
        if (TransientNameStack.Contains(type)) {
            throw new CircularDependencyException(string.Join("\n", TransientNameStack));
        }
        TransientNameStack.Push(type);
    }

    internal void PushTransient(object instance) {
        NewTransientsCreated.Add(instance);
    }

    internal void PopTransient() {
        TransientNameStack.Pop();
    }

    internal void End() {
        foreach (var instance in NewSingletonsCreated.Values) {
            Container.ExecutePostInjectMethods(instance);
            Container.ExecuteOnCreated(Lifetime.Singleton, instance);
        }
        foreach (var instance in NewTransientsCreated) {
            Container.ExecutePostInjectMethods(instance);
            Container.ExecuteOnCreated(Lifetime.Transient, instance);
        }
        _onEnd?.Invoke();
    }

    public void Clear() {
        NewSingletonsCreated.Clear();
        NewTransientsCreated.Clear();
        TransientNameStack.Clear();
    }
}