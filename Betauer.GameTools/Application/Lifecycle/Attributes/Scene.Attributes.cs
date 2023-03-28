using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

public static class Scene {
    public class SingletonAttribute<T> : FactoryTemplateAttribute where T : Node {
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

        public override FactoryTemplate CreateFactoryTemplate(FieldInfo fieldInfo) {
            return new FactoryTemplate {
                Lifetime = Lifetime.Singleton,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(Tag, Resource),
                Name = fieldInfo.Name,
                Primary = false,
            };
        }
    }

    public class TransientAttribute<T> : FactoryTemplateAttribute where T : Node {
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

        public override FactoryTemplate CreateFactoryTemplate(FieldInfo fieldInfo) {
            return new FactoryTemplate {
                Lifetime = Lifetime.Transient,
                FactoryType = typeof(SceneFactory<T>),
                Factory = () => new SceneFactory<T>(Tag, Resource),
                Name = fieldInfo.Name,
                Primary = false,
            };
        }
    }
}