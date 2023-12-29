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
        var instance = CreateTransient();

        if (Injector.NeedsInjection(instance)) {
            var context = Container.CreateContext();
            context.PushTransient(this, instance);
            Injector.InjectServices(context, Lifetime, instance);
            context.PopTransient();
            context.End();
        } else {
            Container.ExecutePostInjectMethods(instance);
            Container.ExecuteOnCreated(new InstanceCreatedEvent(this, instance));
        }
        return instance;
    }

    public override object Resolve(ResolveContext context) {
        if (context == null) throw new ArgumentNullException(nameof(context));
        var instance = CreateTransient();
        context.PushTransient(this, instance);  // This call could throw a CircularDependencyException            
        Injector.InjectServices(context, Lifetime, instance);
        context.PopTransient();
        return instance;
    }

    private object CreateTransient() {
        var instance = _factory.Invoke();
        if (instance == null) throw new NullReferenceException($"Transient factory returned null for {RealType.GetTypeName()} {Name}");
        Logger.Debug("Creating {0}:{1}. Name: \"{2}\". HashCode: {3:X}", Lifetime.Transient, instance.GetType().GetTypeName(), Name, instance.GetHashCode());
        return instance;
    }
}