using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Attributes;

public class FactoryTemplate {
    public Lifetime Lifetime { get; init; }
    public Type FactoryType { get; init; }
    public Func<object> Factory { get; init; }
    public string? Name { get; init; }
    public bool Primary { get; init; }
}