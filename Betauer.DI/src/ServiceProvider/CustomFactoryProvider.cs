using System;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider;

public class CustomFactoryProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Provider>();
    public override Lifetime Lifetime => Lifetime.Singleton;
    public bool IsCustomFactoryInjected { get; private set; }
    public object? CustomFactory { get; }
        
    public CustomFactoryProvider(object /* IFactory<> */ customFactory)
        //ignored because custom factories are not added to the container registry
        : base(typeof(void), typeof(void), null, null) {
        
        if (!customFactory.GetType().ImplementsInterface(typeof(IFactory<>))) {
            throw new InvalidCastException($"Factory {customFactory.GetType().GetTypeName()} must implement IFactory<T>");
        }
        CustomFactory = customFactory;
    }
    
    public override object Get() {
        return IsCustomFactoryInjected ? CustomFactory! : Container.Resolve(this);
    }

    public override object Resolve(ResolveContext context) {
        if (IsCustomFactoryInjected) return CustomFactory!;
        IsCustomFactoryInjected = true;
        context.Container.InjectServices(Lifetime, CustomFactory!, context);
        context.AddSingleton(this, CustomFactory!);
        return CustomFactory!;
    }
}