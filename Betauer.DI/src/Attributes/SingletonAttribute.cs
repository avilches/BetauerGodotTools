using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;
    public bool Lazy { get; set; } = false;

    public SingletonAttribute() {
    }

    public SingletonAttribute(string name) {
        Name = name;
    }

    public void CreateProvider(Type type, Container.Builder builder) {
        builder.RegisterServiceAndAddFactory(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : type, // Trick to get the <T> from SingletonAttribute<T>
            type,
            Lifetime.Singleton,
            () => Activator.CreateInstance(type)!,
            Name,
            Primary,
            Lazy);
    }

    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
        builder.RegisterServiceAndAddFactory(
            GetType().IsGenericType ? GetType().GetGenericArguments()[0] : getter.Type, // Trick to get the <T> from SingletonAttribute<T>
            getter.Type,
            Lifetime.Singleton,
            () => getter.GetValue(configuration)!,
            Name ?? getter.Name,
            Primary,
            Lazy);
    }
}

public class SingletonAttribute<T> : SingletonAttribute { }
