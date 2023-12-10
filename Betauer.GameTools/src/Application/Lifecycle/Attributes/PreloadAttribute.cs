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
    public string? TypeHint { get; set; }

    public PreloadAttribute(string name, string path, string typeHint = null) {
        Name = name;
        Path = path;
        TypeHint = typeHint;
    }

    public void Apply(object configuration, Container.Builder builder) {
        T Factory() => ResourceLoader.Load<T>(Path, TypeHint);
        var provider = Provider.Singleton<T, T>(Factory, Name, false);
        builder.Register(provider);
    }
}
