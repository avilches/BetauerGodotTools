using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI; 

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScanAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScanAttribute<T> : ScanAttribute {
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class ServiceAttribute : Attribute {
    public Type? Type { get; set; }
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;
    public bool Lazy { get; set; } = false;
    public Lifetime Lifetime { get; set; } = Lifetime.Singleton;

    public ServiceAttribute() {
    }

    public ServiceAttribute(Lifetime lifetime) {
        Lifetime = lifetime;
    }

    public ServiceAttribute(string name) {
        Name = name;
    }
    public ServiceAttribute(Type type) {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class FactoryAttribute : Attribute {
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;
    public bool Lazy { get; set; } = false;

    public FactoryAttribute() {
    }

    public FactoryAttribute(string name) {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class PrimaryAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class LazyAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Method  | AttributeTargets.Property)]
public class InjectAttribute : Attribute {
    public bool Nullable { get; set; } = false;
    public string? Name { get; set; }

    public InjectAttribute() {
    }

    public InjectAttribute(string name) {
        Name = name;
    }
}