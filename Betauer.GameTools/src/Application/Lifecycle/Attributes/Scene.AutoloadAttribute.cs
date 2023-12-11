using System;
using System.Collections.Generic;
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
    public class AutoloadAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string Name { get; init; }
        public string Path { get; init; }
        public string? Tag { get; init; }
        public string? Flags { get; init; }

        public AutoloadAttribute(string name) {
            Name = name;
        }

        public AutoloadAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public void Apply(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(SingletonAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
            }
            var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
            var providers = builder.RegisterFactory<T, SceneFactory<T>>(
                Lifetime.Singleton,
                sceneFactory,
                Name,
                true,
                Provider.FlagsToMetadata(Flags, "AddToTree"));
            sceneFactory.PreInject(loaderConfiguration.Name, providers.Provider);
        }
    }
}
