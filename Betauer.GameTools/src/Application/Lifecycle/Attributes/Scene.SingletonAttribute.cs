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
    /// If Name is null, the singleton only can be resolved by type
    /// If Path is null, the tscn file must be located in the same folder as the Node class.
    /// The resource path will be extracted using the [ScriptPathAttribute]
    ///
    /// Not lazy singletons will be created when the Tag loader is loaded. That's different than regular singleton, where the not lazy are
    /// created during container initialization.
    /// In any case, a ILazy<> provider will be registered, so you can inject it in your classes before 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SingletonAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string? Name { get; init; }
        public string? Path { get; init; }
        public string? Tag { get; init; }
        public string? Flags { get; init; }
        public bool Lazy { get; init; } = false;

        public void Apply(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(SingletonAttribute<T>).FormatAttribute(new Dictionary<string, object> {
                        { "Name", Name },
                        { "Path", Path },
                        { "Tag", Tag },
                        { "Flags", Flags },
                    })} needs the attribute {typeof(LoaderAttribute).FormatAttribute()} in the same class. Type: {configuration.GetType()}");
            }
            var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
            var metadata = Provider.FlagsToMetadata(Flags);
            var create = FactoryTools.From(sceneFactory);
            var provider = new SingletonProvider(typeof(T), typeof(T), create, Name, true, // must be lazy so the Loader can load the scene first
                metadata);
            builder.Register(provider);
            builder.Register(Provider.Lazy(provider));

            builder.OnBuildFinished += () => {
                var loader = builder.Container.Resolve<ResourceLoaderContainer>(loaderConfiguration.Name);
                sceneFactory.SetResourceLoaderContainer(loader);
                if (!Lazy) {
                    sceneFactory.OnLoad += () => provider.Get();
                }
            };
        }
    }
}