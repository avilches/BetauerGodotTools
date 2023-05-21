using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
        public string? Name { get; set; }

        public SingletonAttribute() {
        }

        public SingletonAttribute(string name) {
            Name = name;
        }

        public void CreateProvider(Type type, Container.Builder builder) {
            builder.RegisterFactory(type, Lifetime.Singleton, () => Activator.CreateInstance(type), Name);
        }

        public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
            builder.RegisterFactory(getter.Type, Lifetime.Singleton, () => getter.GetValue(configuration), Name ?? getter.Name);
        }
    }
}