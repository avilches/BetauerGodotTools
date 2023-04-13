using System;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PreloadAttribute<T> : Attribute, IConfigurationClassAttribute where T : Resource {
    public string? Name { get; set; }
    public string Path { get; set; }
    public bool Lazy { get; set; }

    public PreloadAttribute(string name, string path, bool lazy = false) {
        Name = name;
        Path = path;
        Lazy = lazy;
    }

    public void CreateProvider(object configuration, Container.Builder builder) {
        Func<T> factory = () => ResourceLoader.Load<T>(Path);
        builder.RegisterServiceAndAddFactory(typeof(T), typeof(T),
            Lifetime.Singleton,
            factory,
            Name, false, false);
    }
}
