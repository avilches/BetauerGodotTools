using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Godot;

namespace Betauer.Application.Lifecycle;

public abstract class ResourceLoad : IInjectable {
    public const string DefaultTag = "(default)";
    
    public ResourceLoaderContainer? ResourceLoaderContainer { get; private set; }
    public string Path { get; }
    public string Tag { get; }
    public Resource? Resource { get; protected set; }

    protected ResourceLoad(string path, string? tag = null) {
        Path = path;
        Tag = tag ?? DefaultTag;
    }

    [Inject] public DI.Container Container { get; set; }
    private string? _resourceLoaderContainerName;
    public void PreInject(string resourceLoaderContainerName) {
        _resourceLoaderContainerName = resourceLoaderContainerName;
    }

    public virtual void PostInject() {
        SetResourceLoaderContainer(Container.Resolve<ResourceLoaderContainer>(_resourceLoaderContainerName!));
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