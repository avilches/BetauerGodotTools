using Betauer.DI.ServiceProvider;

namespace Betauer.DI;

public class ProviderResolved {
    private readonly Provider _provider;
    public readonly object Instance;

    public Lifetime Lifetime => _provider.Lifetime;
    public bool GetFlag(string key, bool @default = false) => _provider.GetFlag(key, @default);

    public ProviderResolved(Provider p, object instance) {
        _provider = p;
        Instance = instance;
    }
}