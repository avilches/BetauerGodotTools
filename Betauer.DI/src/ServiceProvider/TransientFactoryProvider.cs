using System;
using Betauer.Core;
using Betauer.Tools.Logging;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.ServiceProvider; 

public class TransientFactoryProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Provider>();
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Transient;

    public TransientFactoryProvider(Type registerType, Type providerType, Func<object>? factory = null, string? name = null, bool primary = false) : base(registerType, providerType, name, primary) {
        _factory = factory ?? CreateDefaultFactory(providerType, Lifetime);
    }

    public override object Resolve(ResolveContext context) {
        if (context == null) throw new ArgumentNullException(nameof(context));
        context.TryStartTransient(RegisterType, Name); // This call could throw a CircularDependencyException
        var instance = _factory.Invoke();
        if (instance == null) throw new NullReferenceException($"Transient factory returned null for {RegisterType.GetTypeName()} {Name}");
        Logger.Debug($"Creating {Lifetime.Transient}:{instance.GetType().GetTypeName()}. Name: \"{Name}\". HashCode: {instance.GetHashCode():X}");
        context.PushTransient(instance);
        context.Container.InjectServices(Lifetime, instance, context);
        context.PopTransient();
        return instance;
    }
}