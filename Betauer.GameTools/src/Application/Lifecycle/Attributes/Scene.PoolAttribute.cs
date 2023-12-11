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
    public class NodePoolAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string Name { get; init; }
        public string Path { get; init; }
        public string? Tag { get; init; }

        public NodePoolAttribute(string name) {
            Name = name;
        }

        public NodePoolAttribute(string name, string path) {
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
                    $"Attribute {typeof(PoolContainerAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(NodePoolAttribute<T>).FormatAttribute()}");
            }
            
            var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
            sceneFactory.OnInstantiate += instance => {
                builder.Container.InjectServices(instance);
            };
            var nodePool = new NodePool<T>(sceneFactory.Create);
            builder.Register(Provider.Static(nodePool, Name));
            builder.OnBuildFinished += () => {
                var loader = builder.Container.Resolve<ResourceLoaderContainer>(loaderConfiguration.Name);
                sceneFactory.SetResourceLoaderContainer(loader);

                var poolContainer = builder.Container.Resolve<IPoolContainer>(poolContainerAttribute.Name);
                nodePool.SetPoolContainer(poolContainer);
            };
        }
    }
}