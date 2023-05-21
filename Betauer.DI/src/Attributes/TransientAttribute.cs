using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class TransientAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }

    
    public TransientAttribute() {
    }

    public TransientAttribute(string name) {
        Name = name;
    }

    public void CreateProvider(Type type, Container.Builder builder) {
        var provider = Provider.Create(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from TransientAttribute<T>
            type,
            Lifetime.Transient,
            () => Activator.CreateInstance(type)!,
            Name,
            true); // lazy flag is ignored in transient services
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }

    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
        var provider = Provider.Create(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from TransientAttribute<T>
            getter.Type,
            Lifetime.Transient,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            true); // lazy flag is ignored in transient services
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }
}

public class TransientAttribute<T> : TransientAttribute { }