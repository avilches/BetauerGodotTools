using System;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class SingletonAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
        public string? Name { get; set; }
        public bool Primary { get; set; } = false;

        public SingletonAttribute() {
        }

        public SingletonAttribute(string name) {
            Name = name;
        }

        public void CreateProvider(Type type, Container.Builder builder) {
            AttributeTools.ValidateDuplicates<ScanAttribute, TransientAttribute, Betauer.DI.Attributes.TransientAttribute, Betauer.DI.Attributes.SingletonAttribute>(type, this);
            builder.RegisterFactory(type, Lifetime.Singleton, () => Activator.CreateInstance(type), Name, Primary);
        }

        public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
            builder.RegisterFactory(getter.Type, Lifetime.Singleton, () => getter.GetValue(configuration), Name ?? getter.Name, Primary);
        }
    }
}