using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ResourceAttribute<T> : FactoryTemplateAttribute where T : Resource {
    public string? Name { get; set; }
    public string? Tag { get; set; }
    public string Resource { get; set; }

    public ResourceAttribute(string tag, string resource) {
        Tag = tag;
        Resource = resource;
    }

    public ResourceAttribute(string resource) {
        Tag = null;
        Resource = resource;
    }

    // Return a factory template that can be used to create the resource.
    public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
        return new FactoryTemplate {
            // ResourceFactory resources must be transient: if they are unloaded and loaded again, Get() will return the new instance
            Lifetime = Lifetime.Transient,
            FactoryType = typeof(ResourceFactory<T>),
            Factory = () => new ResourceFactory<T>(Tag, Resource),
            Name = Name ?? memberInfo.Name,
            Primary = false,
        };
    }
}