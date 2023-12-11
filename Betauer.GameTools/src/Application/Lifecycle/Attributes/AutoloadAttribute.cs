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
        var metadata = Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true) ?? new Dictionary<string, object>();
        metadata["AddToTree"] = true;
        // if (!type.IsSubclassOf(typeof(Node))) throw new Exception();
        var provider = new SingletonFactoryProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from SingletonAttribute<T>
            type,
            name: Name,
            lazy: false,
            metadata: metadata);
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }

    public void Apply(object configuration, IGetter getter, Container.Builder builder) {
        var metadata = Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true) ?? new Dictionary<string, object>();
        metadata["AddToTree"] = true;
        // if (!getter.Type.IsSubclassOf(typeof(Node))) throw new Exception();
        var provider = new SingletonFactoryProvider(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from SingletonAttribute<T>
            getter.Type,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            false,
            metadata);
        builder.Register(provider);
        builder.RegisterFactory(provider);
    }
}


public class AutoloadAttribute<T> : AutoloadAttribute {
}