using System;
using System.Collections.Generic;
using Betauer.DI.Factory;

namespace Betauer.DI.ServiceProvider;

public class ProxyProvider : Provider {
    public override Lifetime Lifetime => Lifetime.Singleton;
    public Proxy ProxyInstance { get; }

    public Provider Provider { get;  }

    public static readonly string FactoryPrefix = "Factory:";

    public ProxyProvider(Type proxyFactoryType, Lifetime lifetime, Proxy factory, string? name = null, bool serviceLazy = false,
        Dictionary<string, object>? metadata = null) : base(proxyFactoryType, proxyFactoryType, name, metadata) {
        ProxyInstance = factory;
    }

    public override object Get() {
        return ProxyInstance;
    }

    public override object Resolve(ResolveContext context) {
        return ProxyInstance;
    }

    public abstract class Proxy {
        public Provider Provider { get; }

        protected Proxy(Provider provider) {
            Provider = provider;
        }

        public class Transient<T> : Proxy, ITransient<T> where T : class {
            public Transient(Provider provider) : base(provider) {
            }

            public T Create() => (T)Provider.Get();
        }

        public class Singleton<T> : Proxy, ILazy<T> where T : class {
            public Singleton(Provider provider) : base(provider) {
            }

            public T Get() => (T)Provider.Get();
        }
    }
}