using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class TransientAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
        public string? Name { get; set; }

        public TransientAttribute() {
        }

        public TransientAttribute(string name) {
            Name = name;
        }

        public void CreateProvider(Type type, Container.Builder builder) {
            builder.RegisterFactoryFactory(type, Lifetime.Transient, () => Activator.CreateInstance(type), Name);
        }

        public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
            builder.RegisterFactoryFactory(getter.Type, Lifetime.Transient, () => getter.GetValue(configuration), Name ?? getter.Name);
        }
    }
}