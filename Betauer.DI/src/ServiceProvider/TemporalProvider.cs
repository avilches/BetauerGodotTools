using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.Tools.Logging;

namespace Betauer.DI.ServiceProvider; 

public class TemporalProvider : Provider {
    private static readonly Logger Logger = LoggerFactory.GetLogger<TemporalProvider>();
    private readonly Func<object> _factory;
    public override Lifetime Lifetime => Lifetime.Transient;
    public bool IsInstanceCreated => Instance != null;
    public object? Instance { get; private set; }
    public string? Group { get; set; }

    public TemporalProvider(Type publicType, Type realType, Func<object>? factory = null, string? name = null, Dictionary<string, object>? metadata = null) : base(publicType, realType, name, metadata) {
        _factory = factory ?? CreateCtor(realType, Lifetime.Transient);
    }

    public override object Get() {
        return Instance == null ? Container.FromContext(Resolve) : Instance!;
    }

    public override object Resolve(ResolveContext context) {
        if (Instance != null) return Instance!;
        if (context == null) throw new ArgumentNullException(nameof(context));
        Instance = _factory.Invoke();
        if (Instance == null) throw new NullReferenceException($"Temporal factory returned null for {RealType.GetTypeName()} {Name}");
        Logger.Debug("Creating {0}:{1}. Name: \"{2}\". HashCode: {3:X}", Lifetime.Transient, Instance.GetType().GetTypeName(), Name, Instance.GetHashCode());
        // context.NewTransient(this, Instance);
        Injector.InjectServices(context, Lifetime, Instance);
        // context.PopTransient();
        return Instance;
    }

    public void Clear() {
        Instance = null;
    }
}