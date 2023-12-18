using System.Collections.Generic;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public readonly struct InstanceCreatedEvent {
    private readonly Provider _provider;
    public readonly object Instance;

    public string Name => _provider.Name;
    public Lifetime Lifetime => _provider.Lifetime;
    public Dictionary<string, object> Metadata => _provider.Metadata;
    public object GetMetadata(string key) => _provider.GetMetadata(key);
    public T GetMetadata<T>(string key) => _provider.GetMetadata<T>(key);
    public T GetMetadata<T>(string key, T @default) => _provider.GetMetadata(key, @default);
    public bool GetFlag(string key, bool @default = false) => _provider.GetFlag(key, @default);

    internal InstanceCreatedEvent(Provider p, object instance) {
        _provider = p;
        Instance = instance;
    }
}