using System;
using Betauer.Application.Settings.Attributes;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ResourceAttribute<T> : FactoryTemplateClassAttribute where T : Resource {
    public string Name { get; set; }
    public string Path { get; set; }
    public string? Tag { get; set; }

    public ResourceAttribute(string name, string path) {
        Name = name;
        Path = path;
    }

    // Return a factory template that can be used to create the resource.
    public override FactoryTemplate CreateFactoryTemplate(object configuration) {
        var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
        if (loaderConfiguration == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(ResourceAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
        }
        return new FactoryTemplate {
            // ResourceFactory resources must be transient: if they are unloaded and loaded again, Get() will return the new instance
            Lifetime = Lifetime.Transient,
            FactoryType = typeof(ResourceFactory<T>),
            Factory = () => {
                var resourceFactory = new ResourceFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
                resourceFactory.PreInject(loaderConfiguration.Name);
                return resourceFactory;
            },
            Name = Name,
            Primary = false,
        };
    }
}