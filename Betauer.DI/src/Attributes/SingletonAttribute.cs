using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

/// <summary>
/// If Name is null, the singleton only can be resolved by type
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; init; }
    public string? Flags { get; init; }
    public bool Lazy { get; init; } = false;

    public void Apply(Type type, Container.Builder builder) {
        var provider = new SingletonProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from SingletonAttribute<T>
            type,
            name: Name,
            lazy: Lazy,
            metadata: Provider.FlagsToMetadata(Flags));
        builder.Register(provider);
        if (Lazy) builder.Register(Provider.Lazy(provider)); // Non lazy singletons don't need the ILazy<T>
    }

    public void Apply(object configuration, IGetter getter, Container.Builder builder) {
        var provider = new SingletonProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.MemberType, // Trick to get the <T> from SingletonAttribute<T>
            getter.MemberType,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            Lazy,
            Provider.FlagsToMetadata(Flags));
        builder.Register(provider);
        if (Lazy) builder.Register(Provider.Lazy(provider)); // Non lazy singletons don't need the ILazy<T>
    }
}

public class SingletonAttribute<T> : SingletonAttribute {
}