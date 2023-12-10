using System;
using System.Linq;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class TransientAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }
    public string? Flags { get; set; }

    
    public TransientAttribute() {
    }

    public TransientAttribute(string name) {
        Name = name;
    }

    public virtual void Apply(Type type, Container.Builder builder) {
        var provider = new TransientFactoryProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from TransientAttribute<T>
            type,
            name: Name,
            metadata: Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true));
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }

    public virtual void Apply(object configuration, IGetter getter, Container.Builder builder) {
        var provider = new TransientFactoryProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from TransientAttribute<T>
            getter.Type,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true));
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }
}

public class TransientAttribute<T> : TransientAttribute { }