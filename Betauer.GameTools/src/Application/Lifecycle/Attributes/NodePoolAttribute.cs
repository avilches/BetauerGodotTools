using System;
using Betauer.Core;
using Betauer.Core.Pool.Lifecycle;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NodePoolAttribute<T> : Attribute, IConfigurationMemberAttribute where T : Node, IPoolLifecycle {
    public string? Name { get; set; }

    public NodePoolAttribute() {
    }

    public NodePoolAttribute(string name) {
        Name = name;
    }

    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
        var poolContainerAttribute = configuration.GetType().GetAttribute<PoolContainerAttribute>()!;
        if (poolContainerAttribute == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(PoolContainerAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(NodePoolAttribute<T>).FormatAttribute()}");
        }
		
        Func<object> factory = () => {
            NodePool<T> nodePool = (NodePool<T>)getter.GetValue(configuration)!;
            nodePool.PreInject(poolContainerAttribute.Name);
            return nodePool;
        };
        var name = Name ?? getter.Name;
        builder.RegisterServiceAndAddFactory(typeof(NodePool<T>), typeof(NodePool<T>), Lifetime.Singleton, factory, name, false, false);
    }
}