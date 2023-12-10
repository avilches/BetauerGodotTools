using System;
using System.Linq;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Attributes;

public static partial class Factory {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class TransientAttribute : Attribute, IClassAttribute, IConfigurationMemberAttribute {
        public string? Name { get; set; }
        public string? Flags { get; set; }

        public TransientAttribute() {
        }

        public TransientAttribute(string name) {
            Name = name;
        }

        public virtual void Apply(Type type, Container.Builder builder) {
            builder.RegisterFactory(
                type, 
                Lifetime.Transient, 
                () => Activator.CreateInstance(type)!, 
                Name, 
                Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true));
        }

        public virtual void Apply(object configuration, IGetter getter, Container.Builder builder) {
            builder.RegisterFactory(
                getter.Type, 
                Lifetime.Transient, 
                () => getter.GetValue(configuration)!, 
                Name ?? getter.Name, 
                Flags?.Split(",").ToDictionary(valor => valor, _ => (object)true));
        }
    }
}