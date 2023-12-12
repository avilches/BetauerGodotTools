using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class TransientProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Provider>();
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Transient;

    public TransientProvider(Type exposedType, Type instanceType, Func<object>? factory = null, string? name = null, Dictionary<string, object>? metadata = null) : base(exposedType, instanceType, name, metadata) {
        _factory = factory ?? CreateDefaultFactory(instanceType, Lifetime.Transient);
    }

    public override object Get() {
        return Container.Resolve(this);
    }

    public override object Resolve(ResolveContext context) {
        if (context == null) throw new ArgumentNullException(nameof(context));
        context.TryStartTransient(ExposedType, Name); // This call could throw a CircularDependencyException
        var instance = _factory.Invoke();
        if (instance == null) throw new NullReferenceException($"Transient factory returned null for {ExposedType.GetTypeName()} {Name}");
        Logger.Debug($"Creating {Lifetime.Transient}:{instance.GetType().GetTypeName()}. Name: \"{Name}\". HashCode: {instance.GetHashCode():X}");
        context.PushTransient(this, instance);
        context.Container.InjectServices(Lifetime, instance, context);
        context.PopTransient();
        return instance;
    }
}