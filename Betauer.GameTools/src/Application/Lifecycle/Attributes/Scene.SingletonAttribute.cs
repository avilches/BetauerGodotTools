using System;
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
    public class SingletonAttribute<T> : Attribute, IConfigurationClassAttribute where T : Node {
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Tag { get; set; }

        public SingletonAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public void CreateProvider(object configuration, Container.Builder builder) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(SingletonAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
            }
            builder.RegisterFactory(typeof(SceneFactory<T>), Lifetime.Singleton, () => {
                var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
                sceneFactory.PreInject(loaderConfiguration.Name);
                return sceneFactory;
            }, Name);
        }
    }
}