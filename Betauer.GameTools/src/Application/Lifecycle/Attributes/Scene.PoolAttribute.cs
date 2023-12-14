using System;
using System.Collections.Generic;
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
    /// <summary>
    /// If Name is null, the NodePool only can be resolved by type
    /// If Path is null, the tscn file must be located in the same folder as the Node class.
    /// The resource path will be extracted using the [ScriptPathAttribute]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodePoolAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string? Name { get; init; }
        public string? Path { get; init; }
        public string? Tag { get; init; }
        public string? Flags { get; init; }

        public void Apply(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(NodePoolAttribute<T>).FormatAttribute(new Dictionary<string, object> {
                        { "Name", Name },
                        { "Path", Path },
                        { "Tag", Tag },
                        { "Flags", Flags },
                    })} needs the attribute {typeof(LoaderAttribute).FormatAttribute()} in the same class. Type: {configuration.GetType()}");
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