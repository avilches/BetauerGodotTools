using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle;

public class ResourceAttribute<T> : FactoryTemplateAttribute where T : Resource {
    public string Tag { get; set; }
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
    public override FactoryTemplate CreateFactoryTemplate(FieldInfo fieldInfo) {
        return new FactoryTemplate {
            Name = fieldInfo.Name,
            Primary = false,
            RegisterType = fieldInfo.FieldType,
            ProviderType = typeof(ResourceFactory<T>),
            // Resources must be transient: if they are unloaded and loaded again, Get() will return the new instance
            Lifetime = Lifetime.Transient,
            Factory = () => new ResourceFactory<T>(Tag, Resource),
        };
    }
}