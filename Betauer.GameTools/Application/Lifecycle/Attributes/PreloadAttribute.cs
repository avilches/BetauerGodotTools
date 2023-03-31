using System;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PreloadAttribute<T> : ServiceTemplateClassAttribute where T : Resource {
    public string? Name { get; set; }
    public string Path { get; set; }
    public bool Lazy { get; set; }

    public PreloadAttribute(string name, string path, bool lazy = false) {
        Name = name;
        Path = path;
        Lazy = lazy;
    }

    public override ServiceTemplate CreateServiceTemplate(object configuration) {
        return new ServiceTemplate {
            Lifetime = Lifetime.Singleton,
            ProviderType = typeof(T),
            RegisterType = typeof(T),
            Factory = () => ResourceLoader.Load<T>(Path),
            Name = Name,
            Primary = false,
            Lazy = false,
        };
    }
}
