using System.Collections.Generic;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public class ResolveContext {
    private readonly Dictionary<Provider, InstanceCreatedEvent> _newSingletonsCreated = new();
    private readonly List<InstanceCreatedEvent> _newTransientsCreated = new();
    private readonly Stack<TransientProvider> _transientStack = new();

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
    internal void PushTransient(TransientProvider provider, object instance) {
        if (_transientStack.Contains(provider)) {
            throw new CircularDependencyException(string.Join("\n", _transientStack));
        }
        _transientStack.Push(provider);
        _newTransientsCreated.Add(new InstanceCreatedEvent(provider, instance));
    }

    internal void PopTransient() {
        _transientStack.Pop();
    }

    internal void End() {
        if (_newSingletonsCreated.Count > 0) {
            foreach (var instanceCreatedEvent in _newSingletonsCreated.Values) {
                Container.ExecutePostInjectMethods(instanceCreatedEvent.Instance);
                Container.ExecuteOnCreated(instanceCreatedEvent);
            }
            _newSingletonsCreated.Clear();
        }
        if (_newTransientsCreated.Count > 0) {
            foreach (var instanceCreatedEvent in _newTransientsCreated) {
                Container.ExecutePostInjectMethods(instanceCreatedEvent.Instance);
                Container.ExecuteOnCreated(instanceCreatedEvent);
            }
            _newTransientsCreated.Clear();
        }
        _transientStack.Clear();
        Container.ReleaseContext(this);
    }
}