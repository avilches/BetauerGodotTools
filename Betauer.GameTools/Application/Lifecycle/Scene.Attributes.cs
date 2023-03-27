using System;
using Betauer.DI.Attributes;
using Godot;

namespace Betauer.Application.Lifecycle;

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

        public override Func<object> GetCustomFactory() {
            return () => new SceneFactory<T>(Tag, Resource);
        }

        public override FactoryAttribute GetFactoryAttribute() {
            return new Factory.SingletonAttribute {
                Name = null,
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

        public override Func<object> GetCustomFactory() {
            return () => new SceneFactory<T>(Tag, Resource);
        }

        public override FactoryAttribute GetFactoryAttribute() {
            return new Factory.TransientAttribute {
                Name = null,
                Primary = false,
            };
        }
    }
}