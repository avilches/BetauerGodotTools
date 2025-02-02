using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.Core.Signal;
using Betauer.Tools.Logging;
using Godot;

namespace Betauer.Application.Lifecycle;

public class ResourceLoaderContainer {
    private static readonly Logger Logger = LoggerFactory.GetLogger<ResourceLoaderContainer>();

    public List<ResourceLoad> ResourceFactories { get; } = [];

    public Func<Task>? Awaiter { get; private set; }
    public event Action<ResourceProgress>? OnLoadResourceProgress;
    // public event Action? OnLoadFinished;

    // Use ResourceFactory.SetResourceLoaderContainer() instead
    internal void Add(ResourceLoad resourceLoad) {
        if (ResourceFactories.Contains(resourceLoad)) return; // avoid duplicates
        ResourceFactories.Add(resourceLoad);
    }

    // Use ResourceFactory.SetResourceLoaderContainer() instead
    internal void Remove(ResourceLoad resourceLoad) {
        ResourceFactories.Remove(resourceLoad);
    }
                  
    public Task<TimeSpan> LoadResources(Action<ResourceProgress>? progressAction = null) {
        return LoadResources([ResourceLoad.DefaultTag], progressAction);
    }

    public Task<TimeSpan> LoadResources(string tag, Action<ResourceProgress>? progressAction = null) {
        return LoadResources([tag], progressAction);
    }

    public async Task<TimeSpan> LoadResources(string[] tags, Action<ResourceProgress>? progressAction = null) {
        var x = Stopwatch.StartNew();
        Func<Task> awaiter = Awaiter ?? (async () => {
            await ((SceneTree)Engine.GetMainLoop()).AwaitProcessFrame();
        });
        var resources = GetResourceFactories(tags)
            .Where(sf => !sf.IsLoaded())
            .Select(sf => new ResourceLoadingState(sf.Path, sf.Load))
            .ToList();
        
        Logger.Debug("Loading {0}", tags.Join(", "));
        await LoadTools.Load(resources, awaiter, (rp) => {
            Logger.Debug("{0:0.00}% | {1} ({2:0.00}%)", rp.TotalPercent * 100, rp.Resource, rp.ResourcePercent * 100);
            OnLoadResourceProgress?.Invoke(rp);
            progressAction?.Invoke(rp);
        });
        // OnLoadFinished?.Invoke();
        var timeSpan = x.Elapsed;
        Logger.Debug("Total load time: {0}s {1:D3}ms", timeSpan.Seconds, timeSpan.Milliseconds);
        return x.Elapsed;
    }

    public void UnloadResources() {
        GetResourceFactories(ResourceLoad.DefaultTag).ForEach(sf => sf.Unload());
    }

    public void UnloadResources(string tag) {
        GetResourceFactories(tag).ForEach(sf => sf.Unload());
    }

    public void UnloadResources(string[] tags) {
        GetResourceFactories(tags).ForEach(sf => sf.Unload());
    }

    public IEnumerable<ResourceLoad> GetResourceFactories(string tag) {
        return ResourceFactories.Where(sf => sf.Tag == tag);
    }

    public IEnumerable<ResourceLoad> GetResourceFactories(string[] tags) {
        var set = new HashSet<string>(tags);
        return ResourceFactories.Where(sf => set.Contains(sf.Tag));
    }
}