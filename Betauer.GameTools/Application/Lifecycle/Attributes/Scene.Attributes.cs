using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

public static class Scene {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SingletonAttribute<T> : FactoryTemplateAttribute where T : Node {
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Tag { get; set; }

        public SingletonAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
            var loaderConfiguration = memberInfo.GetAttribute<LoaderConfiguration>();
            return new FactoryTemplate {
                Lifetime = Lifetime.Singleton,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(loaderConfiguration!.Name, Path, Tag ?? loaderConfiguration.Tag),
                Name = Name ?? memberInfo.Name,
                Primary = false,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransientAttribute<T> : FactoryTemplateAttribute where T : Node {
        public string Name { get; set; }
        public string Path { get; set; }
        public string? Tag { get; set; }

        public TransientAttribute(string name, string path) {
            Name = name;
            Path = path;
        }

        public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
            var loaderConfiguration = memberInfo.GetAttribute<LoaderConfiguration>();
            return new FactoryTemplate {
                Lifetime = Lifetime.Transient,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(loaderConfiguration!.Name, Path, Tag ?? loaderConfiguration.Tag),
                Name = Name ?? memberInfo.Name,
                Primary = false,
            };
        }
    }
}