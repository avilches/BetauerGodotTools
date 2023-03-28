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
    public string? Tag { get; set; }
    public string Path { get; set; }

    public ResourceAttribute() {
    }

    // Return a factory template that can be used to create the resource.
    public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
        var tag = Tag ?? memberInfo.GetAttribute<LoaderConfiguration>()?.Tag;
        return new FactoryTemplate {
            // ResourceFactory resources must be transient: if they are unloaded and loaded again, Get() will return the new instance
            Lifetime = Lifetime.Transient,
            FactoryType = typeof(ResourceFactory<T>),
            Factory = () => new ResourceFactory<T>(tag, Path),
            Name = Name,
            Primary = false,
        };
    }
}