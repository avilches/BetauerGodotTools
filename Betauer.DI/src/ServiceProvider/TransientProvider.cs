using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class TransientProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<TransientProvider>();
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Transient;

    public TransientProvider(Type publicType, Type realType, Func<object>? factory = null, string? name = null, Dictionary<string, object>? metadata = null) : base(publicType, realType, name, metadata) {
        _factory = factory ?? CreateCtor(realType, Lifetime.Transient);
    }

    public override object Get() {
        return Container.FromContext(Resolve);
    }

    public override object Resolve(ResolveContext context) {
        if (context == null) throw new ArgumentNullException(nameof(context));
        context.PushTransient(PublicType); // This call could throw a CircularDependencyException
        var instance = _factory.Invoke();
        if (instance == null) throw new NullReferenceException($"Transient factory returned null for {PublicType.GetTypeName()} {Name}");
        Logger.Debug("Creating {0}:{1}. Name: \"{2}\". HashCode: {3:X}", Lifetime.Transient, instance.GetType().GetTypeName(), Name, instance.GetHashCode());
        context.NewTransient(this, instance);
        context.InjectServices(Lifetime, instance);
        context.PopTransient();
        return instance;
    }
}