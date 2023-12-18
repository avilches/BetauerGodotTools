using System;
using System.Collections.Generic;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public class ResolveContext {
    private readonly Dictionary<Provider, InstanceCreatedEvent> _newSingletonsCreated = new();
    private readonly List<InstanceCreatedEvent> _newTransientsCreated = new();
    private readonly Stack<Type> _transientStack = new();

    internal Container Container { get; }

    public ResolveContext(Container container) {
        Container = container;
    }

    internal bool TryGetSingletonFromCache(Provider provider, out object? instanceFound) {
        if (_newSingletonsCreated.TryGetValue(provider, out var resolved)) {
            instanceFound = resolved.Instance;
            return true;
        }
        instanceFound = null;
        return false;
    }

    internal void NewSingleton(Provider provider, object instance) {
        _newSingletonsCreated[provider] = new InstanceCreatedEvent(provider, instance);
    }

    // This stack avoid circular dependencies between transients
    internal void PushTransient(Type type) {
        if (_transientStack.Contains(type)) {
            throw new CircularDependencyException(string.Join("\n", _transientStack));
        }
        _transientStack.Push(type);
    }

    internal void NewTransient(Provider provider, object instance) {
        _newTransientsCreated.Add(new InstanceCreatedEvent(provider, instance));
    }

    internal void PopTransient() {
        _transientStack.Pop();
    }

    internal void End() {
        foreach (var instanceCreatedEvent in _newSingletonsCreated.Values) {
            Container.ExecutePostInjectMethods(instanceCreatedEvent.Instance);
        }
        foreach (var instanceCreatedEvent in _newTransientsCreated) {
            Container.ExecutePostInjectMethods(instanceCreatedEvent.Instance);
        }
        foreach (var instanceCreatedEvent in _newSingletonsCreated.Values) {
            Container.ExecuteOnCreated(instanceCreatedEvent);
        }
        foreach (var instanceCreatedEvent in _newTransientsCreated) {
            Container.ExecuteOnCreated(instanceCreatedEvent);
        }
        _newSingletonsCreated.Clear();
        _newTransientsCreated.Clear();
        _transientStack.Clear();
    }

    public void InjectServices(Lifetime lifetime, object o) {
        Injector.InjectServices(this, lifetime, o);
    }
}