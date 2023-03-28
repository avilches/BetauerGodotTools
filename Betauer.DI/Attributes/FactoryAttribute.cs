using System;
using System.Reflection;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public abstract class FactoryAttribute : BaseProviderAttribute {
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;
    public Lifetime Lifetime { get; }

    protected FactoryAttribute(Lifetime lifetime) {
        Lifetime = lifetime;
    }
}

public static class Factory {
    public class SingletonAttribute : FactoryAttribute {
        public SingletonAttribute() : base(Lifetime.Singleton) {
        }

        public SingletonAttribute(string name) : base(Lifetime.Singleton) {
            Name = name;
        }
    }

    public class TransientAttribute : FactoryAttribute {
        public TransientAttribute() : base(Lifetime.Transient) {
        }

        public TransientAttribute(string name) : base(Lifetime.Transient) {
            Name = name;
        }
    }
}