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
    
    public SceneFactory(string? path = null, string? tag = null) : base(ExtractScenePathFromScriptPathIfNull<T>(path), tag) {
    }

    public PackedScene Scene => (PackedScene)Resource!;

    public T Create() {
        if (Scene == null) throw new Exception($"Can't instantiate scene from null resource: {Path}. Try to load tag '{Tag}' first");
        try {
            var instantiate = Scene.Instantiate<T>();
            NodePathScanner.ScanAndInject(instantiate);
            return instantiate;
        } catch (Exception) {
            Logger.Error("Error instantiating scene: {0} Tag: {1}",Path, Tag);
            throw;
        }
    }

    private static string ExtractScenePathFromScriptPathIfNull<T>(string? path) where T : Node {
        if (path != null) return path;
        var attribute = typeof(T).GetAttribute<ScriptPathAttribute>();
        if (attribute == null) {
            throw new Exception($"Can get path from {typeof(T).GetTypeName()} scene with");
        }
        return attribute.Path.Replace(".cs", ".tscn");
    }
}