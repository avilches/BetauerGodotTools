using System;
using Betauer.Application.Lifecycle.Pool;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class PoolContainerAttribute : Attribute {
    public string Name { get; set; }
}

public class PoolContainerAttribute<T> : PoolContainerAttribute, IConfigurationClassAttribute where T : class {
    public PoolContainerAttribute(string name) {
        Name = name;
    }

    public void Apply(object configuration, Container.Builder builder) {
        PoolContainer<T> Factory() => new();
        var provider = Provider.Singleton<PoolContainer<T>, PoolContainer<T>>(Factory, Name, false);
        builder.Register(provider);
    }
}