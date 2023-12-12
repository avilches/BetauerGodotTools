using System;
using System.Collections.Generic;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class StaticProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Provider>();
    
    public override Lifetime Lifetime => Lifetime.Singleton;
    public bool IsInitialized { get; private set; }
    public object? Instance { get; private set; }
        
    public StaticProvider(Type exposedType, Type providerType, object instance, string? name = null, Dictionary<string, object>? metadata = null) : base(exposedType, providerType, name, metadata) {
        Instance = instance;
    }
    
    public override object Get() {
        return IsInitialized ? Instance! : Container.Resolve(this);
    }

    public override object Resolve(ResolveContext context) {
        if (IsInitialized) return Instance!;
        IsInitialized = true;
        context.Container.InjectServices(Lifetime, Instance!, context);
        context.AddSingleton(this, Instance!);
        return Instance!;
    }
}