using System;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.NodePath;
using Godot;

namespace Betauer.Application.Lifecycle;

public class SceneFactory<T> : ResourceFactory, IFactory<T> where T : Node {

    public SceneFactory(string tag, string resourcePath) : base(tag, resourcePath) {
    }

    public SceneFactory(string resourcePath) : base(null, resourcePath) {
    }

    public PackedScene Scene => (PackedScene)Resource!;

    public T Get() {
        var instantiate = Scene.Instantiate<T>();
        NodePathScanner.ScanAndInject(instantiate);
        return instantiate;
    }
}


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