using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class TransientAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; init; }
    public string? Flags { get; init; }


    public TransientAttribute() {
    }

    public TransientAttribute(string name) {
        Name = name;
    }

    public void Apply(Type type, Container.Builder builder) {
        var provider = new TransientProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from TransientAttribute<T>
            type,
            name: Name,
            metadata: Provider.FlagsToMetadata(Flags));
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }

    public void Apply(object configuration, IGetter getter, Container.Builder builder) {
        var provider = new TransientProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from TransientAttribute<T>
            getter.Type,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            Provider.FlagsToMetadata(Flags));
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }
}

public class TransientAttribute<T> : TransientAttribute {
}