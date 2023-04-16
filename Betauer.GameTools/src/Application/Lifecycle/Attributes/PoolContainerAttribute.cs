using System;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PoolContainerAttribute : Attribute, IConfigurationClassAttribute {
    public string Name { get; set; }

    public PoolContainerAttribute(string name) {
        Name = name;
    }

    public void CreateProvider(object configuration, Container.Builder builder) {
        Func<PoolContainer<IPoolLifecycle>> factory = () => new PoolContainer<IPoolLifecycle>();
        builder.RegisterServiceAndAddFactory(typeof(PoolContainer<IPoolLifecycle>), typeof(PoolContainer<IPoolLifecycle>),
            Lifetime.Singleton,
            factory,
            Name, false, false);
    }
}