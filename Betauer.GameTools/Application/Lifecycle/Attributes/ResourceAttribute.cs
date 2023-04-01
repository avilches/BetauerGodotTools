using System;
using Betauer.Application.Settings.Attributes;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ResourceAttribute<T> : Attribute, IConfigurationClassAttribute where T : Resource {
    public string Name { get; set; }
    public string Path { get; set; }
    public string? Tag { get; set; }

    public ResourceAttribute(string name, string path) {
        Name = name;
        Path = path;
    }

    public void CreateProvider(object configuration, Container.Builder builder) {
        var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
        if (loaderConfiguration == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(ResourceAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
        }
        Func<ResourceFactory<T>> customFactory = () => {
            var resourceFactory = new ResourceFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
            resourceFactory.PreInject(loaderConfiguration.Name);
            return resourceFactory;
        };
        // ResourceFactory already caches the resource, but it still needs to be transient: if they are unloaded and loaded again,
        // Get() will always return a valid resource instance
        builder.RegisterFactory(typeof(ResourceFactory<T>), Lifetime.Transient, customFactory, Name, false);
    }
}