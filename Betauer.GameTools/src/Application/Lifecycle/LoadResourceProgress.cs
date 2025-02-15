using System;
using Godot;

namespace Betauer.Application.Lifecycle;

public class ResourceLoadProgress {
    internal Resource? Resource { get; private set; }
    internal readonly string Path;
    internal float Progress = 0;
    private readonly Action<Resource>? _onLoad;

    public ResourceLoadProgress(string path) {
        Path = path;
    }

    public ResourceLoadProgress(string path, Action<Resource> onLoad) {
        Path = path;
        _onLoad = onLoad;
    }

    public void Load(Resource resource) {
        Progress = 1;
        Resource = resource ?? throw new ResourceLoaderException("Resource can't be null");
        _onLoad?.Invoke(resource);
    }
}