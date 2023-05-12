using System;
using Betauer.DI.Factory;
using Betauer.NodePath;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Lifecycle;

public class SceneFactory<T> : ResourceFactory, IGet<T> where T : Node {
    private static readonly Logger Logger = LoggerFactory.GetLogger<ResourceLoaderContainer>();
    
    public SceneFactory(string path, string? tag = null) : base(path, tag) {
    }

    public PackedScene Scene => (PackedScene)Resource!;

    public T Get() {
        try {
            var instantiate = Scene.Instantiate<T>();
            NodePathScanner.ScanAndInject(instantiate);
            return instantiate;
        } catch (Exception) {
            Logger.Error("Error instantiating scene: {0} Tag: {1}",Path, Tag);
            throw;
        }
    }
}