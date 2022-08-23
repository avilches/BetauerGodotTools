using System;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScanAttribute : Attribute {
        public Type[] Types { get; set; }

        public ScanAttribute(params Type[] types) {
            Types = types;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostCreateAttribute : Attribute {
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
    public class PrimaryAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class LazyAttribute : Attribute {
    }
}