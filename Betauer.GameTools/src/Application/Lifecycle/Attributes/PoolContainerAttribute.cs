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

    public void CreateProvider(object configuration, Container.Builder builder) {
        Func<PoolContainer<T>> factory = () => new PoolContainer<T>();
        builder.RegisterServiceAndAddFactory(typeof(PoolContainer<T>), typeof(PoolContainer<T>),
            Lifetime.Singleton,
            factory,
            Name, false, false);
    }
}