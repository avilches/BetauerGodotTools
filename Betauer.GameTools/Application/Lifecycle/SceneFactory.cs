using Betauer.DI.Factory;
using Godot;

namespace Betauer.Application.Lifecycle;

public class SceneFactory<T> : ResourceFactory, IFactory<T> where T : Node {

    public SceneFactory(string tag, string resourcePath) : base(tag, resourcePath) {
    }

    public SceneFactory(string resourcePath) : base(null, resourcePath) {
    }

    public T Get() {
        return ((PackedScene)Resource!).Instantiate<T>();
    }
}