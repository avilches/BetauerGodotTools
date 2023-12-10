using System;
using System.Linq;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }
    public bool Lazy { get; set; } = false;
    public string? Flags { get; set; }

    public SingletonAttribute() {
    }

    public SingletonAttribute(string name) {
        Name = name;
    }

    public virtual void Apply(Type type, Container.Builder builder) {
        var provider = new SingletonFactoryProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from SingletonAttribute<T>
            type,
            name: Name,
            lazy: Lazy,
            metadata: Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true));
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }

    public virtual void Apply(object configuration, IGetter getter, Container.Builder builder) {
        var provider = new SingletonFactoryProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from SingletonAttribute<T>
            getter.Type,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            Lazy,
            Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true));
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }
}

public class SingletonAttribute<T> : SingletonAttribute { }
