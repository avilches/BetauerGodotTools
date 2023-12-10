using System.Collections.Generic;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public class ProviderResolved {
    private readonly IProvider _provider;
    public readonly object Instance;

    public Lifetime Lifetime => _provider.Lifetime;
    public Dictionary<string, object> Metadata => _provider.Metadata;
    public T GetMetadata<T>(string key) => (T)_provider.Metadata[key];
    public T GetMetadata<T>(string key, T @default) => _provider.Metadata.TryGetValue(key, out var value) ? (T)value : @default;
    public bool GetFlag(string key, bool @default = false) => _provider.Metadata.TryGetValue(key, out var value) ? (bool)value : @default;

    public ProviderResolved(IProvider p, object instance) {
        _provider = p;
        Instance = instance;
    }
}