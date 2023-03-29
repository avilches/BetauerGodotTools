using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ResourceAttribute<T> : FactoryTemplateAttribute where T : Resource {
    public string Name { get; set; }
    public string Path { get; set; }
    public string? Tag { get; set; }

    public ResourceAttribute(string name, string path) {
        Name = name;
        Path = path;
    }

    // Return a factory template that can be used to create the resource.
    public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
        var loaderConfiguration = memberInfo.GetAttribute<LoaderConfiguration>();
        return new FactoryTemplate {
            // ResourceFactory resources must be transient: if they are unloaded and loaded again, Get() will return the new instance
            Lifetime = Lifetime.Transient,
            FactoryType = typeof(ResourceFactory<T>),
            Factory = () => new ResourceFactory<T>(loaderConfiguration!.Name, Path, Tag ?? loaderConfiguration.Tag),
            Name = Name,
            Primary = false,
        };
    }
}