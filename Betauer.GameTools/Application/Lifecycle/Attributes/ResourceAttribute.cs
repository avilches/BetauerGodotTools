using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
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
        return new FactoryTemplate {
            // ResourceFactory resources must be transient: if they are unloaded and loaded again, Get() will return the new instance
            Lifetime = Lifetime.Transient,
            FactoryType = typeof(ResourceFactory<T>),
            Factory = () => new ResourceFactory<T>(Tag, Path),
            Name = Name,
            Primary = false,
        };
    }
}