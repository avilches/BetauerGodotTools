using System;
using Betauer.Application.Lifecycle.Pool;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PoolAttribute : Attribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }

    public PoolAttribute() {
    }

    public PoolAttribute(string name) {
        Name = name;
    }

    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
        var poolContainerAttribute = configuration.GetType().GetAttribute<PoolContainerAttribute>()!;
        if (poolContainerAttribute == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(PoolContainerAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(PoolAttribute).FormatAttribute()}");
        }
        Func<object> factory = () => {
            IManagedPool nodePool = (IManagedPool)getter.GetValue(configuration)!;
            nodePool.PreInject(poolContainerAttribute.Name);
            return nodePool;
        };
        var name = Name ?? getter.Name;
        var provider = Provider.Create(getter.Type, getter.Type, Lifetime.Singleton, factory, name, false);
        builder.Register(provider);
    }
}