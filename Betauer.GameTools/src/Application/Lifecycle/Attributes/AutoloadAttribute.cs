using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class AutoloadAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; init; }
    public string? Flags { get; init; }

    public AutoloadAttribute() {
    }

    public AutoloadAttribute(string name) {
        Name = name;
    }

    public void Apply(Type type, Container.Builder builder) {
        // if (!type.IsSubclassOf(typeof(Node))) throw new Exception();
        var provider = new SingletonProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from SingletonAttribute<T>
            type,
            name: Name,
            lazy: false,
            metadata: Provider.FlagsToMetadata(Flags, "AddToTree"));
        builder.Register(provider);
        builder.Register(ProxyFactoryProvider.Create(provider));
    }

    public void Apply(object configuration, IGetter getter, Container.Builder builder) {
        // if (!getter.Type.IsSubclassOf(typeof(Node))) throw new Exception();
        var provider = new SingletonProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from SingletonAttribute<T>
            getter.Type,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            false,
            Provider.FlagsToMetadata(Flags, "AddToTree"));
        builder.Register(provider);
        builder.Register(ProxyFactoryProvider.Create(provider));
    }
}


public class AutoloadAttribute<T> : AutoloadAttribute {
}