using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.DI.Factory;

namespace Betauer.DI.ServiceProvider;

public class ProxyFactoryProvider : Provider {
    public override Lifetime Lifetime => Lifetime.Singleton;
    public Proxy ProxyInstance { get; }


    public Lifetime ServiceLifetime { get; }
    public bool ServiceLazy { get; }

    public static readonly string FactoryPrefix = "Factory:";

    public static ProxyFactoryProvider Create(IProvider provider) {
        if (provider is not SingletonProvider &&
            provider is not TransientProvider) {
            throw new ArgumentException($"Provider must be a {nameof(SingletonProvider)} or {nameof(TransientProvider)}, but was {provider.GetType().GetTypeName()}");
        }
        var proxyFactory = CreateProxyFactory(provider.InstanceType, provider);
        var proxyFactoryName = provider.Name == null ? null : $"{FactoryPrefix}{provider.Name}";
        var proxyFactoryType = (provider.Lifetime == Lifetime.Singleton ? typeof(ILazy<>) : typeof(ITransient<>)).MakeGenericType(provider.InstanceType);
        var lazy = provider is not SingletonProvider singletonProvider || singletonProvider.Lazy;
        var proxyProvider = new ProxyFactoryProvider(proxyFactoryType, provider.Lifetime, proxyFactory, proxyFactoryName, lazy);
        return proxyProvider;
    }

    public ProxyFactoryProvider(Type proxyFactoryType, Lifetime lifetime, Proxy factory, string? name = null, bool serviceLazy = false,
        Dictionary<string, object>? metadata = null) : base(proxyFactoryType, proxyFactoryType, name, metadata) {
        ServiceLifetime = lifetime;
        ProxyInstance = factory;
        ServiceLazy = serviceLazy;
    }

    public override object Get() {
        return ProxyInstance;
    }

    public override object Resolve(ResolveContext context) {
        return ProxyInstance;
    }

    /// <summary>
    /// Returns a class that wraps a provider (implementing ITransient<T> or ILazy<T>) so it can be exposed to the users 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    private static Proxy CreateProxyFactory(Type type, IProvider provider) {
        if (provider.Lifetime == Lifetime.Singleton) {
            var factoryType = typeof(Proxy.Singleton<>).MakeGenericType(type);
            Proxy instance = (Proxy)Activator.CreateInstance(factoryType, provider)!;
            return instance;
        } else {
            var factoryType = typeof(Proxy.Transient<>).MakeGenericType(type);
            Proxy instance = (Proxy)Activator.CreateInstance(factoryType, provider)!;
            return instance;
        }
    }

    public abstract class Proxy {
        protected readonly IProvider Provider;

        protected Proxy(IProvider provider) {
            Provider = provider;
        }

        public class Transient<T> : Proxy, ITransient<T> where T : class {
            public Transient(IProvider provider) : base(provider) {
            }

            public T Create() => (T)Provider.Get();
        }

        public class Singleton<T> : Proxy, ILazy<T> where T : class {
            public Singleton(IProvider provider) : base(provider) {
            }

            public T Get() => (T)Provider.Get();
        }
    }
}