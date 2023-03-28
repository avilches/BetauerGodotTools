using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

public static class Scene {
    [AttributeUsage(AttributeTargets.Field)]
    public class SingletonAttribute<T> : FactoryTemplateAttribute where T : Node {
        public string? Name { get; set; }
        public string Tag { get; set; }
        public string Resource { get; set; }

        public SingletonAttribute(string tag, string resource) {
            Tag = tag;
            Resource = resource;
        }

        public SingletonAttribute(string resource) {
            Tag = null;
            Resource = resource;
        }

        public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
            return new FactoryTemplate {
                Lifetime = Lifetime.Singleton,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(Tag, Resource),
                Name = Name ?? memberInfo.Name,
                Primary = false,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TransientAttribute<T> : FactoryTemplateAttribute where T : Node {
        public string? Name { get; set; }
        public string Tag { get; set; }
        public string Resource { get; set; }

        public TransientAttribute(string tag, string resource) {
            Tag = tag;
            Resource = resource;
        }

        public TransientAttribute(string resource) {
            Tag = null;
            Resource = resource;
        }

        public override FactoryTemplate CreateFactoryTemplate(MemberInfo memberInfo) {
            return new FactoryTemplate {
                Lifetime = Lifetime.Transient,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(Tag, Resource),
                Name = Name ?? memberInfo.Name,
                Primary = false,
            };
        }
    }
}