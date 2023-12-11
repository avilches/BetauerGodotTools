using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
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
            builder.RegisterSingletonFactory(Activator.CreateInstance(type)!, Name, Lazy, Provider.FlagsToMetadata(Flags));
        }

        public void Apply(object configuration, IGetter getter, Container.Builder builder) {
            builder.RegisterSingletonFactory(getter.GetValue(configuration)!, Name ?? getter.Name, Lazy, Provider.FlagsToMetadata(Flags));
        }
    }
}