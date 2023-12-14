using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
    /// <summary>
    /// If Name is null, the singleton only can be resolved by type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
        public string? Scope { get; init; }
        public string? Name { get; init; }
        public string? Flags { get; init; }
        public bool Lazy { get; init; } = false;

        public void Apply(Type type, Container.Builder builder) {
            var factory = Activator.CreateInstance(type)!;
            var provider = Provider.SingletonFactory(factory, Scope, Name, Lazy, Provider.FlagsToMetadata(Flags));
            builder.RegisterFactory(factory);
            builder.Register(provider);
            if (Lazy) builder.Register(Provider.Proxy(provider)); // Non lazy singletons don't need the ILazy<T>
        }

        public void Apply(object configuration, IGetter getter, Container.Builder builder) {
            var factory = getter.GetValue(configuration)!;
            var provider = Provider.SingletonFactory(factory, Scope, Name ?? getter.Name, Lazy, Provider.FlagsToMetadata(Flags));
            builder.RegisterFactory(factory);
            builder.Register(provider);
            if (Lazy) builder.Register(Provider.Proxy(provider)); // Non lazy singletons don't need the ILazy<T>
        }
    }
}