using System;
using Betauer.Core;
using Betauer.DI.Factory;
using Betauer.DI.ServiceProvider;
using Betauer.NodePath;
using Betauer.Tools.FastReflection;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Lifecycle;

public class SceneFactory<T> : ResourceLoad, IFactory<T> where T : Node {
    private static readonly Logger Logger = LoggerFactory.GetLogger<ResourceLoaderContainer>();
    
    private IProvider? _autoload;

    public SceneFactory(string? path = null, string? tag = null) : base(ExtractScenePathFromScriptPathIfNull<T>(path), tag) {
    }

    public PackedScene Scene => (PackedScene)Resource!;

    public T Create() {
        if (Scene == null) {
            throw new Exception($"Can't instantiate scene from null resource: {Path}. Load the tag '{Tag}' first");
        }
        try {
            var instance = Scene.Instantiate<T>();
            NodePathScanner.ScanAndInject(instance);
            return instance;
        } catch (Exception) {
            Logger.Error("Error instantiating scene: {0} Tag: {1}",Path, Tag);
            throw;
        }
    }

    public void PreInject(string resourceLoaderContainerName, IProvider? autoload) {
        base.PreInject(resourceLoaderContainerName);
        _autoload = autoload;
    }

    public override void PostInject() {
        base.PostInject();
        if (_autoload != null) {
            ResourceLoaderContainer!.OnLoadFinished += () => _autoload.Get();
        }
    }

    private static string ExtractScenePathFromScriptPathIfNull<T>(string? path) where T : Node {
        if (path != null) return path;
        var attribute = typeof(T).GetAttribute<ScriptPathAttribute>();
        if (attribute == null) {
            throw new Exception($"Can get path from {typeof(T).GetTypeName()} scene with");
        }
        path = attribute.Path.Replace(".cs", ".tscn");
        if (!FileAccess.FileExists(path)) {
            throw new Exception($"Scene from {typeof(T).GetTypeName()} class doesn't exist: {path}");
        }
        return path;
    }
}