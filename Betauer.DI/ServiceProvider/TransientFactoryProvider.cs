using System;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class TransientFactoryProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Provider));
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Transient;

    public TransientFactoryProvider(Type registerType, Type providerType, Func<object>? factory = null, string? name = null, bool primary = false) : base(registerType, providerType, name, primary) {
        _factory = factory ?? CreateDefaultFactory(providerType, Lifetime);
    }

    public override object Resolve(ResolveContext context) {
        if (context == null) throw new ArgumentNullException(nameof(context));
        context.TryStartTransient(RegisterType, Name); // This call could throw a CircularDependencyException
        var instance = _factory.Invoke();
        if (instance == null) throw new NullReferenceException($"Transient factory returned null for {RegisterType.Name} {Name}");
        Logger.Debug($"Creating {Lifetime.Transient} {instance.GetType().Name} exposed as {RegisterType.Name}: {instance.GetHashCode():X}");
        context.PushTransient(instance);
        context.Container.InjectServices(Lifetime, instance, context);
        context.PopTransient();
        return instance;
    }
}