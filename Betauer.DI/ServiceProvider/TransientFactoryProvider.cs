using System;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class TransientFactoryProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(SingletonFactoryProvider));
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Transient;

    public TransientFactoryProvider(Type registerType, Type providerType, Func<object> factory, string? name = null, bool primary = false) : base(registerType, providerType, name, primary) {
        _factory = factory;
    }

    public override object Get(ResolveContext context) {
        if (context == null) throw new ArgumentNullException(nameof(context));
        context.TryStartTransient(RegisterType, Name); // This call could throw a CircularDependencyException
        var instance = _factory.Invoke();
        if (instance == null) throw new NullReferenceException($"Transient factory returned null for {RegisterType.Name} {Name}");
        Logger.Debug($"Creating {Lifetime.Transient} {instance.GetType().Name} exposed as {RegisterType.Name}: {instance.GetHashCode():X}");
        context.AddTransient(instance);
        context.Container.InjectServices(Lifetime, instance, context);
        context.EndTransient();
        return instance;
    }
}