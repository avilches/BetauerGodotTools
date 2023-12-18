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
    public string? Scope { get; }
        
    public SingletonProvider(Type publicType, Type realType, string? scope, Func<object>? factory = null, string? name = null, bool lazy = false, Dictionary<string, object>? metadata = null) : base(publicType, realType, name, metadata) {
        Scope = scope;
        _factory = factory ?? CreateCtor(realType, Lifetime.Singleton);
        Lazy = lazy;
    }
    
    public override object Get() {
        return Instance == null ? Container.FromContext(Resolve) : Instance!;
    }

    public override object Resolve(ResolveContext context) {
        if (Instance != null) return Instance!;
        if (context.TryGetSingletonFromCache(this, out var singleton)) {
            Logger.Debug("Get from context {0} {1} exposed as {2}: {3:X}", Lifetime.Singleton, singleton.GetType().GetTypeName(), PublicType.GetTypeName(), singleton.GetHashCode());
            return singleton;
        }
        Instance = _factory.Invoke();
        if (Instance == null) throw new NullReferenceException($"Singleton factory returned null for {RealType.GetTypeName()} {Name}");
        Logger.Debug("Creating {0}:{1}. Name: \"{2}\". HashCode: {3:X}", Lifetime.Singleton, Instance.GetType().GetTypeName(), Name, Instance.GetHashCode());
        context.NewSingleton(this, Instance);
        context.InjectServices(Lifetime.Singleton, Instance);
        return Instance;
    }
}