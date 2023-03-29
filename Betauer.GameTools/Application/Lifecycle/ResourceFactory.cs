using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;

namespace Betauer.Application.Lifecycle;

public abstract class ResourceFactory : IInjectable {
    [Inject] public DI.Container Container { get; set; }
    public ResourceLoaderContainer ResourceLoaderContainer { get; private set; }

    public const string DefaultTag = "(default)";

    private readonly string _resourceLoaderContainerName;
    public string Path { get; }
    public string Tag { get; }
    public Resource? Resource { get; protected set; }

    protected ResourceFactory(string resourceLoaderContainerName, string path, string? tag = null) {
        _resourceLoaderContainerName = resourceLoaderContainerName;
        Path = path;
        Tag = tag ?? DefaultTag;
    }
    
    public void PostInject() {
        var resourceLoaderContainer = Container.Resolve<ResourceLoaderContainer>(_resourceLoaderContainerName);
        SetResourceLoaderContainer(resourceLoaderContainer);
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

public class ResourceFactory<T> : ResourceFactory, IFactory<T> where T : Resource {
    
    public ResourceFactory(string resourceLoaderContainerName, string path, string? tag = null) : base(resourceLoaderContainerName, path, tag) {
    }

    public T Get() {
        return (T)Resource!;
    }
}