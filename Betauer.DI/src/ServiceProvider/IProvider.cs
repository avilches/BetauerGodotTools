using System;
using System.Collections.Generic;

namespace Betauer.DI.ServiceProvider; 

public interface IProvider {
    public Container Container { get; set; }
    public string? Name { get; }
    public Type ExposedType { get; }
    public Type InstanceType { get; }
    public Lifetime Lifetime { get; }
    public object Get();
    public object Resolve(ResolveContext context);
    public Dictionary<string, object> Metadata { get; }
}