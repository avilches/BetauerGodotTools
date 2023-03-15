using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.Signal;
using Betauer.DI;
using Godot;

namespace Betauer.Application.Lifecycle;

public class ResourceLoaderContainer {
    public readonly List<ResourceFactory> ResourceFactories = new();

    public Func<Task>? Awaiter { get; private set; }

    [Inject] public SceneTree SceneTree { get; set; }

    public void Add(ResourceFactory resourceFactory) {
        ResourceFactories.Add(resourceFactory);
    }
                  
    public Task LoadResources(Action<ResourceProgress>? progressAction = null) {
        return LoadResources(new [] { ResourceFactory.DefaultTag }, progressAction);
    }

    public Task LoadResources(string tag, Action<ResourceProgress>? progressAction = null) {
        return LoadResources(new [] { tag }, progressAction);
    }

    public virtual void OnLoadResourceProgress(ResourceProgress resourceProgress) {
    }
    
    public async Task LoadResources(string[] tags, Action<ResourceProgress>? progressAction = null) { 
        Func<Task> awaiter = Awaiter ?? (async () => {
            if (SceneTree != null) await SceneTree.AwaitProcessFrame();
            else await Task.Delay(10);
        });
        var set = new HashSet<string>(tags);
        var resources = ResourceFactories
            .Where(sf => sf.Resource == null && set.Contains(sf.Tag))
            .Select(sf => new ResourceLoad(sf.ResourcePath, sf.Load))
            .ToList();
        await LoadTools.LoadThreaded(resources, awaiter, (rp) => {
            OnLoadResourceProgress(rp);
            progressAction?.Invoke(rp);
        });
    }

    public void UnloadResources() {
        UnloadResources(new [] { ResourceFactory.DefaultTag });
    }

    public void UnloadResources(string tag) {
        UnloadResources(new [] { tag });
    }

    public void UnloadResources(string[] tags) {
        var set = new HashSet<string>(tags);
        ResourceFactories
            .Where(sf => sf.Resource != null && set.Contains(sf.Tag))
            .ForEach(sf => sf.Unload());
    }
}