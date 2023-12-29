using System;
using System.Collections.Generic;

namespace Betauer.DI.ServiceProvider; 

public class StaticProvider : Provider {
    public override Lifetime Lifetime => Lifetime.Singleton;
    public bool IsInitialized { get; private set; }
    public object Instance { get; }
        
    public StaticProvider(Type publicType, Type providerType, object instance, string? name = null, Dictionary<string, object>? metadata = null) : base(publicType, providerType, name, metadata) {
        Instance = instance;
    }
    
    public override object Get() {
        if (IsInitialized) return Instance;
        
        if (Injector.NeedsInjection(Instance)) {
            var context = Container.CreateContext();
            Injector.InjectServices(context, Lifetime.Singleton, Instance);
            Container.ExecutePostInjectMethods(Instance);
            context.End();
        } else {
            Container.ExecutePostInjectMethods(Instance);
        }
        return Instance;
    }

    public override object Resolve(ResolveContext context) {
        if (IsInitialized) return Instance;
        Injector.InjectServices(context, Lifetime, Instance);
        Container.ExecutePostInjectMethods(Instance);
        IsInitialized = true;
        return Instance;
    }
}