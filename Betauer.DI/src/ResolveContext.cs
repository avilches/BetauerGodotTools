using System;
using System.Collections.Generic;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public class ResolveContext {
    private readonly Dictionary<Provider, ProviderResolved> _newSingletonsCreated = new();
    private readonly List<ProviderResolved> _newTransientsCreated = new();
    private readonly Stack<Type> _transientNameStack = new();
    private readonly Action? _onEnd;

    internal Container Container { get; }

    public ResolveContext(Container container, Action? onEnd = null) {
        Container = container;
        _onEnd = onEnd;
    }

    internal bool TryGetSingletonFromCache(Provider provider, out object? instanceFound) {
        if (_newSingletonsCreated.TryGetValue(provider, out var resolved)) {
            instanceFound = resolved.Instance;
            return true;
        }
        instanceFound = null;
        return false;
    }

    internal void AddSingleton(Provider provider, object instance) {
        _newSingletonsCreated[provider] = new ProviderResolved(provider, instance);
    }

    // This stack avoid circular dependencies between transients
    internal void TryStartTransient(Type type, string? name) {
        if (_transientNameStack.Contains(type)) {
            throw new CircularDependencyException(string.Join("\n", _transientNameStack));
        }
        _transientNameStack.Push(type);
    }

    internal void PushTransient(Provider provider, object instance) {
        _newTransientsCreated.Add(new ProviderResolved(provider, instance));
    }

    internal void PopTransient() {
        _transientNameStack.Pop();
    }

    internal void End() {
        foreach (var providerResolved in _newSingletonsCreated.Values) {
            Container.ExecutePostInjectMethods(providerResolved.Instance);
            Container.ExecuteOnCreated(providerResolved);
        }
        foreach (var providerResolved in _newTransientsCreated) {
            Container.ExecutePostInjectMethods(providerResolved.Instance);
            Container.ExecuteOnCreated(providerResolved);
        }
        _newSingletonsCreated.Clear();
        _newTransientsCreated.Clear();
        _transientNameStack.Clear();
        _onEnd?.Invoke();
    }
}