using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
    /// <summary>
    /// If Name is null, the transient only can be resolved by type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class TransientAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {

        public string? Name { get; init; }
        public string? Flags { get; init; }

        
        public void Apply(Type type, Container.Builder builder) {
            var factory = Activator.CreateInstance(type)!;
            var provider = Provider.TransientFactory(factory, Name, Provider.FlagsToMetadata(Flags));
            builder.RegisterFactory(factory);
            builder.Register(provider);
            builder.Register(Provider.Proxy(provider));
        }

        public void Apply(object configuration, IGetter getter, Container.Builder builder) {
            var factory = getter.GetValue(configuration)!;
            var provider = Provider.TransientFactory(factory, Name ?? getter.Name, Provider.FlagsToMetadata(Flags));
            builder.RegisterFactory(factory);
            builder.Register(provider);
            builder.Register(Provider.Proxy(provider));
        }
    }
}