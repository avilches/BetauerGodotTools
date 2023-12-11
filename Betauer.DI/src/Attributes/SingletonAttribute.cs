using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; init; }
    public string? Flags { get; init; }
    public bool Lazy { get; init; } = false;

    public SingletonAttribute() {
    }

    public SingletonAttribute(string name) {
        Name = name;
    }

    public void Apply(Type type, Container.Builder builder) {
        var provider = new SingletonProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from SingletonAttribute<T>
            type,
            name: Name,
            lazy: Lazy,
            metadata: Provider.FlagsToMetadata(Flags));
        builder.Register(provider);
        if (Lazy) builder.Register(ProxyFactoryProvider.Create(provider));
    }

    public void Apply(object configuration, IGetter getter, Container.Builder builder) {
        var provider = new SingletonProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from SingletonAttribute<T>
            getter.Type,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            Lazy,
            Provider.FlagsToMetadata(Flags));
        builder.Register(provider);
        if (Lazy) builder.Register(ProxyFactoryProvider.Create(provider));
    }
}

public class SingletonAttribute<T> : SingletonAttribute {
 }