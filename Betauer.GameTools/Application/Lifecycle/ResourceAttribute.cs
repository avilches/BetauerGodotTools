using System;
using Betauer.DI.Attributes;
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

    public override Func<object> GetCustomFactory() {
        return () => new ResourceFactory<T>(Tag, Resource);
    }

    public override FactoryAttribute GetFactoryAttribute() {
        // Resources must be transient, so they can be unloaded and loaded again.
        // If resources were singletons, they would never be unloaded. 
        return new Factory.TransientAttribute {
            Name = null,
            Primary = false,
        };
    }
}