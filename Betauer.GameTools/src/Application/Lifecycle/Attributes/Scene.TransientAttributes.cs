using System;
using System.Linq;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

public static partial class Scene {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransientAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string Name { get; init; }
        public string Path { get; init; }
        public string? Tag { get; init; }
        public string? Flags { get; init; }

        public TransientAttribute(string name) {
            Name = name;
        }

        public TransientAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public void Apply(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(TransientAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
            }
            var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
            var metadata = Provider.FlagsToMetadata(Flags);
            var providers = builder.RegisterTransientFactory<T, SceneFactory<T>>(
                sceneFactory,
                Name,
                metadata);
            
            builder.OnBuildFinished += () => {
                var loader = builder.Container.Resolve<ResourceLoaderContainer>(loaderConfiguration.Name);
                sceneFactory.SetResourceLoaderContainer(loader);
            };

        }
    }
}