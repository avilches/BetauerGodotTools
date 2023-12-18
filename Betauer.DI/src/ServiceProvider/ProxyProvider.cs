using System;
using System.Collections.Generic;

namespace Betauer.DI.ServiceProvider;

public class ProxyProvider : Provider {
    public override Lifetime Lifetime => Lifetime.Singleton;
    public Proxy ProxyInstance { get; }

    public ProxyProvider(Type proxyFactoryType, Proxy factory, string? name = null, Dictionary<string, object>? metadata = null) : base(proxyFactoryType, proxyFactoryType, name, metadata) {
        ProxyInstance = factory;
    }

    public override object Get() {
        return ProxyInstance;
    }

    public override object Resolve(ResolveContext context) {
        return ProxyInstance;
    }
}