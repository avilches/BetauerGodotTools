using System;
using Betauer.DI.ServiceProvider;

namespace Betauer.DI; 

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScanAttribute : Attribute {
}

public class ScanAttribute<T> : ScanAttribute {
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public abstract class ServiceAttribute : Attribute {
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;
    public Lifetime Lifetime { get; }

    protected ServiceAttribute(Lifetime lifetime) {
        Lifetime = lifetime;
    }
}

public class SingletonAttribute : ServiceAttribute {
    public bool Lazy { get; set; } = false;
    public SingletonAttribute() : base(Lifetime.Singleton) {
    }

    public SingletonAttribute(string name) : base(Lifetime.Singleton) {
        Name = name;
    }
}

public class SingletonAttribute<T> : SingletonAttribute { }


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class TransientAttribute : ServiceAttribute {
    public TransientAttribute() : base(Lifetime.Transient) {
    }

    public TransientAttribute(string name) : base(Lifetime.Transient) {
        Name = name;
    }
}

public class TransientAttribute<T> : TransientAttribute { }


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public abstract class FactoryAttribute : Attribute {
    public string? Name { get; set; }
    public bool Primary { get; set; } = false;
    public Lifetime Lifetime { get; }

    protected FactoryAttribute(Lifetime lifetime) {
        Lifetime = lifetime;
    }
}

public class SingletonFactoryAttribute : FactoryAttribute {
    public SingletonFactoryAttribute() : base(Lifetime.Singleton) { }

    public SingletonFactoryAttribute(string name) : base(Lifetime.Singleton) {
        Name = name;
    }
}

public class TransientFactoryAttribute : FactoryAttribute {

    public TransientFactoryAttribute() : base(Lifetime.Transient) {
    }

    public TransientFactoryAttribute(string name) : base(Lifetime.Transient) {
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