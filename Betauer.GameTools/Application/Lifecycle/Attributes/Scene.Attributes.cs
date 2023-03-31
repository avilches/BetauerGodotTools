using System;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

public static class Scene {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SingletonAttribute<T> : FactoryTemplateClassAttribute where T : Node {
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Tag { get; set; }

        public SingletonAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public override FactoryTemplate CreateFactoryTemplate(object configuration) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(SingletonAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
            }
            return new FactoryTemplate {
                Lifetime = Lifetime.Singleton,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => {
                    var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
                    sceneFactory.PreInject(loaderConfiguration.Name);
                    return sceneFactory;
                },
                Name = Name,
                Primary = false,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransientAttribute<T> : FactoryTemplateClassAttribute where T : Node {
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Tag { get; set; }

        public TransientAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public override FactoryTemplate CreateFactoryTemplate(object configuration) {
            var loaderConfiguration = configuration.GetType().GetAttribute<LoaderAttribute>();
            if (loaderConfiguration == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(TransientAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(LoaderAttribute).FormatAttribute()}");
            }
            return new FactoryTemplate {
                Lifetime = Lifetime.Transient,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => {
                    var sceneFactory = new SceneFactory<T>(Path, Tag ?? loaderConfiguration.Tag);
                    sceneFactory.PreInject(loaderConfiguration.Name);
                    return sceneFactory;
                },
                Name = Name,
                Primary = false,
            };
        }
    }
}