using Betauer.DI.Factory;
using Betauer.NodePath;
using Godot;

namespace Betauer.Application.Lifecycle;

public class SceneFactory<T> : ResourceFactory, IFactory<T> where T : Node {

    public SceneFactory(string tag, string path) : base(tag, path) {
    }

    public SceneFactory(string path) : base(null, path) {
    }

    public PackedScene Scene => (PackedScene)Resource!;

    public T Get() {
        var instantiate = Scene.Instantiate<T>();
        NodePathScanner.ScanAndInject(instantiate);
        return instantiate;
    }
}