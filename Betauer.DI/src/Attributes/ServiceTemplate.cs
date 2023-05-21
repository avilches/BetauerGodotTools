using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Attributes;

public class ServiceTemplate {
    public Lifetime Lifetime { get; init; }
    public Type RegisterType { get; init; }
    public Type ProviderType { get; init; }
    public Func<object> Factory { get; init; }
    public string? Name { get; init; }
    public bool Lazy { get; init; }
}