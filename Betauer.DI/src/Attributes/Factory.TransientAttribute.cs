using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
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
            builder.RegisterTransientFactory(Activator.CreateInstance(type)!, Name, Provider.FlagsToMetadata(Flags));
        }

        public void Apply(object configuration, IGetter getter, Container.Builder builder) {
            builder.RegisterTransientFactory(getter.GetValue(configuration)!, Name ?? getter.Name, Provider.FlagsToMetadata(Flags));
        }
    }
}