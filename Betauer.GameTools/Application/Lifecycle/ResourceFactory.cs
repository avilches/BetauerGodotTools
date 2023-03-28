using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;

namespace Betauer.Application.Lifecycle;

public abstract class ResourceFactory : IInjectable {
    [Inject] public ResourceLoaderContainer ResourceLoaderContainer { get; set; }

    public const string DefaultTag = "(default)";

    public string Path { get; }
    public string Tag { get; }
    public Resource? Resource { get; protected set; }

    protected ResourceFactory(string? tag, string path) {
        Tag = tag ?? DefaultTag;
        Path = path;
    }
    
    public void PostInject() {
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

    public ResourceFactory(string? tag, string path) : base(tag, path) {
    }

    public ResourceFactory(string path) : base(null, path) {
    }

    public T Get() {
        return (T)Resource!;
    }
}