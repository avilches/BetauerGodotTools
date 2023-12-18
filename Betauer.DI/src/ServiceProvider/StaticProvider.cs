using System;
using System.Collections.Generic;

namespace Betauer.DI.ServiceProvider; 

public class StaticProvider : Provider {
    public override Lifetime Lifetime => Lifetime.Singleton;
    public bool IsInitialized { get; private set; }
    public object? Instance { get; }
        
    public StaticProvider(Type publicType, Type providerType, object instance, string? name = null, Dictionary<string, object>? metadata = null) : base(publicType, providerType, name, metadata) {
        Instance = instance;
    }
    
    public override object Get() {
        if (!IsInitialized) {
            Container.WithContext(context => Resolve(context));
        }
        return Instance!;
    }

    public override object Resolve(ResolveContext context) {
        if (IsInitialized) return Instance!;
        context.InjectServices(Lifetime, Instance!);
        IsInitialized = true;
        return Instance!;
    }
}