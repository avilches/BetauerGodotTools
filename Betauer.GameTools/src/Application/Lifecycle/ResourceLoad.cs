using Betauer.Core;
using Godot;

namespace Betauer.Application.Lifecycle;

public abstract class ResourceLoad {
    public const string DefaultTag = "(default)";
    
    public ResourceLoaderContainer? ResourceLoaderContainer { get; private set; }
    public string Path { get; }
    public string Tag { get; }
    public Resource? Resource { get; protected set; }

    protected ResourceLoad(string path, string? tag = null) {
        Path = path;
        Tag = tag ?? DefaultTag;
    }

    public void SetResourceLoaderContainer(ResourceLoaderContainer resourceLoaderContainer) {
        if (ResourceLoaderContainer != null && ResourceLoaderContainer != resourceLoaderContainer) {
            ResourceLoaderContainer.Remove(this);
        }
        ResourceLoaderContainer = resourceLoaderContainer;
        ResourceLoaderContainer.Add(this);
    }

    public void Load(Resource resource) {
        if (Resource != null && Resource == resource) return;
        Unload();
        Resource = resource;
    }

    public bool IsLoaded() => Resource != null && Resource.IsInstanceValid();

    public void Unload() {
        Resource?.Dispose();
        Resource = null;
    }
}