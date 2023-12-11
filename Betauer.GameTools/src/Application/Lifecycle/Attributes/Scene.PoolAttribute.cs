using System;
using Betauer.Application.Lifecycle.Pool;
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
    public class PoolAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string Name { get; init; }
        public string Path { get; init; }
        public string? Tag { get; init; }
        public string? Flags { get; init; }

        public PoolAttribute(string name) {
            Name = name;
        }

        public PoolAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public void Apply(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(TransientAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
            }
            var poolContainerAttribute = configuration.GetType().GetAttribute<PoolContainerAttribute>()!;
            if (poolContainerAttribute == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(PoolContainerAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(PoolAttribute<T>).FormatAttribute()}");
            }
            
            var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
            builder.OnBuildFinished += () => {
                var loader = builder.Container.Resolve<ResourceLoaderContainer>(loaderConfiguration.Name);
                sceneFactory.SetResourceLoaderContainer(loader);
            };
            
            var providers = builder.RegisterTransientFactory(sceneFactory, Name, Provider.FlagsToMetadata(Flags));
            var nodePool = new NodePool<T>(() => (T)providers.Provider.Get());
            var provider = Provider.Static(nodePool, "Pool:" + Name);
            builder.Register(provider);
            builder.OnBuildFinished += () => {
                var poolContainer = builder.Container.Resolve<IPoolContainer>(poolContainerAttribute.Name);
                nodePool.SetPoolContainer(poolContainer);
            };
        }
    }
}