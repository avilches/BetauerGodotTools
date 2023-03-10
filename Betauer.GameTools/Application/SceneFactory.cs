using Betauer.DI;
using Betauer.DI.Factory;
using Betauer.Loader;
using Godot;

namespace Betauer.Application;

public class SceneFactory<T> : IFactory<T>, IInjectable where T : class {
    private readonly string _resource;
    public PackedScene Resource;

    public SceneFactory(string resource) {
        _resource = resource;
    }

    public void PostInject() {
        Resource = LoadTools.PackedScene(_resource);
    }

    public T Get() {
        return Resource.Instantiate<T>();
    }
}