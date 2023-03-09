using System;
using System.Collections.Generic;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI; 

public class ResolveContext {
    internal readonly Dictionary<string, object> NewSingletonsCreated = new();
    internal readonly List<object> NewTransientsCreated = new();
    internal readonly Stack<string> TransientNameStack = new();
    internal readonly Container Container;

    private readonly Action? _onEnd;

    public ResolveContext(Container container, Action? onEnd = null) {
        Container = container;
        _onEnd = onEnd;
    }

    internal bool TryGetSingletonFromCache(Type type, string? name, out object? instanceFound) {
        var key = name ?? type.FullName;
        return NewSingletonsCreated.TryGetValue(key, out instanceFound);
    }

    internal void AddSingleton(Type type, object instance, string? name) {
        var key = name ?? type.FullName;
        NewSingletonsCreated[key] = instance;
    }

    // This stack avoid circular dependencies between transients
    internal void TryStartTransient(Type type, string? name) {
        var key = name ?? type.FullName;
        if (TransientNameStack.Contains(key)) {
            throw new CircularDependencyException(string.Join("\n", TransientNameStack));
        }
        TransientNameStack.Push(key);
    }

    internal void AddTransient(object instance) {
        NewTransientsCreated.Add(instance);
    }

    internal void EndTransient() {
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