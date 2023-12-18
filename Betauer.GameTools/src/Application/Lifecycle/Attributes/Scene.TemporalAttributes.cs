using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Lifecycle.Attributes;

public static partial class Scene {
    /// <summary>
    /// If Name is null, the transient only can be resolved by type
    /// If Path is null, the tscn file must be located in the same folder as the Node class.
    /// The resource path will be extracted using the [ScriptPathAttribute]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransientAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string? Name { get; init; }
        public string? Path { get; init; }
        public string? Tag { get; init; }
        public string? Flags { get; init; }

        public void Apply(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(TransientAttribute<T>).FormatAttribute(new Dictionary<string, object> {
                        { "Name", Name },
                        { "Path", Path },
                        { "Tag", Tag },
                        { "Flags", Flags },
                    })} needs the attribute {typeof(LoaderAttribute).FormatAttribute()} in the same class. Type: {configuration.GetType()}");
            }
            var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
            var metadata = Provider.FlagsToMetadata(Flags);
            var create = FactoryTools.From(sceneFactory);
            var transientProvider = new TransientProvider(typeof(T), typeof(T), create, Name, metadata);
            builder.Register(transientProvider);
            builder.Register(Provider.TransientFactory(transientProvider));

            builder.OnBuildFinished += () => {
                var loader = builder.Container.Resolve<ResourceLoaderContainer>(loaderConfiguration.Name);
                sceneFactory.SetResourceLoaderContainer(loader);
            };
        }
    }
}