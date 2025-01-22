using System;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.NodePath;
using Betauer.Tools.FastReflection;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Lifecycle;

public class SceneFactory<T> : ResourceLoad, IFactory<T> where T : Node {
    private static readonly Logger Logger = LoggerFactory.GetLogger<ResourceLoaderContainer>();

    public event Action<T>? OnInstantiate;

    public SceneFactory(string? path = null, string? tag = null) : base(ExtractScenePathFromScriptPathIfNull(path), tag) {
    }

    public PackedScene Scene => (PackedScene)Resource!;

    public T Create() {
        try {
            if (Scene == null) throw new Exception($"Can't instantiate scene without resource: {Path}. Load the tag '{Tag}' first");
            var instance = Scene.Instantiate<T>();
            NodePathScanner.ScanAndInject(instance);
            OnInstantiate?.Invoke(instance);
            return instance;
        } catch (Exception e) {
            Logger.Error("Error instantiating scene {0} Tag {1}:", Path, Tag, e.Message);
            throw;
        }
    }

    private static string ExtractScenePathFromScriptPathIfNull(string? path) {
        if (path != null) return path;
        var attribute = typeof(T).GetAttribute<ScriptPathAttribute>();
        if (attribute == null) {
            throw new Exception($"Can get path from {typeof(T).GetTypeName()} scene with");
        }
        path = attribute.Path.Replace(".cs", ".tscn");
        if (!ResourceLoader.Exists(path)) {
            throw new Exception($"Scene from {typeof(T).GetTypeName()} class doesn't exist: {path}");
        }
        return path;
    }
}