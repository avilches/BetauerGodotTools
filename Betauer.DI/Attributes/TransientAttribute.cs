using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class TransientAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;

    
    public TransientAttribute() {
    }

    public TransientAttribute(string name) {
        Name = name;
    }

    public void CreateProvider(Type type, Container.Builder builder) {
        builder.RegisterServiceAndAddFactory(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from TransientAttribute<T>
            type,
            Lifetime.Transient,
            () => Activator.CreateInstance(type),
            Name,
            Primary,
            true); // lazy flag is ignored in transient services
    }

    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
        builder.RegisterServiceAndAddFactory(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from TransientAttribute<T>
            getter.Type,
            Lifetime.Transient,
            () => getter.GetValue(configuration),
            Name ?? getter.Name,
            Primary,
            true); // lazy flag is ignored in transient services
    }
}

public class TransientAttribute<T> : TransientAttribute { }