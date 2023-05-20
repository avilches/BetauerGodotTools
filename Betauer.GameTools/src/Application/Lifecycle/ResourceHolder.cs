using Godot;

namespace Betauer.Application.Lifecycle;

public class ResourceHolder<T> : ResourceLoad where T : Resource {
    public ResourceHolder(string path, string? tag = null) : base(path, tag) {
    }
    
    public T Get() {
        return (T)Resource!;
    }
}