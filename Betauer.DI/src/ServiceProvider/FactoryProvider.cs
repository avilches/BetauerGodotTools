using System;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider;

public class FactoryProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Provider>();
    public override Lifetime Lifetime { get; }
    public bool IsInstanceCreated { get; private set; }
    public bool Lazy { get; }
    public object? Instance { get; private set; }
        
    public FactoryProvider(object factory, Lifetime lifetime, bool lazy = false) : base(typeof(object), typeof(object), null, null) {
        if (!factory.GetType().ImplementsInterface(typeof(IFactory<>))) {
            throw new InvalidCastException($"Factory {factory.GetType().GetTypeName()} must implement IFactory<T>");
        }
        Instance = factory;
        Lifetime = lifetime;
        Lazy = lazy;
    }
    
    public override object Get() {
        return IsInstanceCreated ? Instance! : Container.Resolve(this);
    }

    public override object Resolve(ResolveContext context) {
        if (IsInstanceCreated) return Instance!;
        IsInstanceCreated = true;
        context.Container.InjectServices(Lifetime, Instance, context);
        context.AddSingleton(this, Instance);
        return Instance;
    }
}