using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class SingletonProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Provider>();
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Singleton;
    public bool IsInstanceCreated { get; private set; }
    public bool Lazy { get; }
    public object? Instance { get; private set; }
        
    public SingletonProvider(Type registerType, Type providerType, Func<object>? factory = null, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) : base(registerType, providerType, name, metadata) {
        _factory = factory ?? CreateDefaultFactory(providerType, Lifetime);
        Lazy = lazy;
    }
    
    public override object Get() {
        return IsInstanceCreated ? Instance! : Container.Resolve(this);
    }

    public override object Resolve(ResolveContext context) {
        if (IsInstanceCreated) return Instance!;
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (context.TryGetSingletonFromCache(this, out var singleton)) {
            Logger.Debug($"Get from context {Lifetime} {singleton.GetType().GetTypeName()} exposed as {RegisterType.GetTypeName()}: {singleton.GetHashCode():X}");
            return singleton;
        }
        lock (this) {
            // Just in case another thread was waiting for the lock
            if (IsInstanceCreated) return Instance!;
            Instance = _factory.Invoke();
            if (Instance == null) throw new NullReferenceException($"Singleton factory returned null for {RegisterType.GetTypeName()} {Name}");
            Logger.Debug($"Creating {Lifetime.Singleton}:{Instance.GetType().GetTypeName()}. Name: \"{Name}\". HashCode: {Instance.GetHashCode():X}");
            context.AddSingleton(this, Instance);
            IsInstanceCreated = true;
            context.Container.InjectServices(Lifetime, Instance, context);
        }
        return Instance;
    }
}