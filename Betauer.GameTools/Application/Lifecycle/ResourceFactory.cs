using Betauer.DI;
using Betauer.DI.Factory;
using Godot;

namespace Betauer.Application.Lifecycle;

public abstract class ResourceFactory : IInjectable {
    [Inject] public ResourceLoaderContainer ResourceLoaderContainer { get; set; }

    public const string DefaultTag = "(default)";

    public string ResourcePath { get; }
    public string Tag { get; }
    public Resource? Resource { get; protected set; }

    protected ResourceFactory(string? tag, string resourcePath) {
        Tag = tag ?? DefaultTag;
        ResourcePath = resourcePath;
    }
    
    public void PostInject() {
        ResourceLoaderContainer.Add(this);
        // Resource = LoadTools.PackedScene(ResourcePath);
    }

    public void Load(Resource resource) {
        Resource = resource;
    }

    public void Unload() {
        Resource?.Dispose();
        Resource = null;
    }
}

public class ResourceFactory<T> : ResourceFactory, IFactory<T> where T : Resource {

    public ResourceFactory(string tag, string resourcePath) : base(tag, resourcePath) {
    }

    public ResourceFactory(string resourcePath) : base(null, resourcePath) {
    }

    public T Get() {
        return (T)Resource!;
    }
}