using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class SingletonProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<SingletonProvider>();
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Singleton;
    public bool IsInstanceCreated => Instance != null;
    public bool Lazy { get; }
    public object? Instance { get; private set; }
        
    public SingletonProvider(Type publicType, Type realType, Func<object>? factory = null, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) : base(publicType, realType, name, metadata) {
        _factory = factory ?? CreateCtor(realType, Lifetime.Singleton);
        Lazy = lazy;
    }
    
    public override object Get() {
        if (Instance != null) return Instance;

        Instance = CreateSingleton();

        if (Injector.NeedsInjection(Instance)) {
            var context = Container.CreateContext();
            context.NewSingleton(this, Instance);
            Injector.InjectServices(context, Lifetime.Singleton, Instance);
            context.End();
        } else {
            Container.ExecutePostInjectMethods(Instance);
            Container.ExecuteOnCreated(new InstanceCreatedEvent(this, Instance));
        }
        return Instance;
    }

    public override object Resolve(ResolveContext context) {
        if (Instance != null) return Instance;
        if (context.TryGetSingletonFromCache(this, out var singleton)) {
            Logger.Debug("Get from context {0} {1} exposed as {2}: {3:X}", Lifetime.Singleton, singleton.GetType().GetTypeName(), PublicType.GetTypeName(), singleton.GetHashCode());
            return singleton;
        }
        
        Instance = CreateSingleton();
        
        context.NewSingleton(this, Instance);
        Injector.InjectServices(context, Lifetime.Singleton, Instance);
        return Instance;
    }

    private object CreateSingleton() {
        var instance = _factory.Invoke();
        if (instance == null) throw new NullReferenceException($"Singleton factory returned null for {RealType.GetTypeName()} {Name}");
        Logger.Debug("Creating {0}:{1}. Name: \"{2}\". HashCode: {3:X}", Lifetime.Singleton, instance.GetType().GetTypeName(), Name, instance.GetHashCode());
        return instance;
    }
}