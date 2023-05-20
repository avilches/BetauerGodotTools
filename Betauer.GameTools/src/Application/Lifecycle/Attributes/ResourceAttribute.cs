using System;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
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

        ResourceHolder<T> Factory() {
            var resourceFactory = new ResourceHolder<T>(Path, Tag ?? loaderConfiguration.Tag);
            resourceFactory.PreInject(loaderConfiguration.Name);
            return resourceFactory;
        }

        var providerFactory = Provider.Create(typeof(ResourceHolder<T>), typeof(ResourceHolder<T>), Lifetime.Singleton, (Func<ResourceHolder<T>>)Factory, Name, false, false);
        builder.Register(providerFactory);
    }
}
