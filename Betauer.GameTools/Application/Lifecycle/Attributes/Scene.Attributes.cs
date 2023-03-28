using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

public static class Scene {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SingletonAttribute<T> : FactoryTemplateAttribute where T : Node {
        public string? Name { get; set; }
        public string Tag { get; set; }
        public string Path { get; set; }

        public SingletonAttribute() {
        }

        public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
            return new FactoryTemplate {
                Lifetime = Lifetime.Singleton,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(Tag, Path),
                Name = Name ?? memberInfo.Name,
                Primary = false,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TransientAttribute<T> : FactoryTemplateAttribute where T : Node {
        public string? Name { get; set; }
        public string Tag { get; set; }
        public string Path { get; set; }

        public TransientAttribute() {
        }

        public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
            return new FactoryTemplate {
                Lifetime = Lifetime.Transient,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(Tag, Path),
                Name = Name ?? memberInfo.Name,
                Primary = false,
            };
        }
    }
}