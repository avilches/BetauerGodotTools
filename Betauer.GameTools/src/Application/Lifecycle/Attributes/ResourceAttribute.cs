using System;
using System.Collections.Generic;
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
    public string Name { get; init; }
    public string Path { get; init; }
    public string? Tag { get; init; }

    public ResourceAttribute(string name, string path) {
        Name = name;
        Path = path;
    }

    public void Apply(object configuration, Container.Builder builder) {
        if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(Name)) {
            throw new ArgumentNullException(nameof(Path),
                $"Path or Name can't be null or empty in {typeof(ResourceAttribute<T>).FormatAttribute(new Dictionary<string, object> {
                    { "Path", Path },
                    { "Name", Name },
                    { "Tag", Tag },
                })} in Configuration class: {configuration.GetType()}");
        }
        var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
        if (loaderConfiguration == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(ResourceAttribute<T>).FormatAttribute(new Dictionary<string, object> {
                    { "Name", Name },
                    { "Path", Path },
                    { "Tag", Tag },
                })} needs the attribute {typeof(LoaderAttribute).FormatAttribute()} in the same class. Type: {configuration.GetType()}");
        }

        var resourceHolder = new ResourceHolder<T>(Path, Tag ?? loaderConfiguration.Tag);
        var providerFactory = Provider.Static(resourceHolder, Name);
        builder.Register(providerFactory);
        builder.OnBuildFinished += () => {
            var loader = builder.Container.Resolve<ResourceLoaderContainer>(loaderConfiguration.Name);
            resourceHolder.SetResourceLoaderContainer(loader);
        };
    }
}