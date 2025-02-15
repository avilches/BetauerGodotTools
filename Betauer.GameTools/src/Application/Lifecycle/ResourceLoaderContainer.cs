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
    public event Action<LoadProgress>? OnLoadResourceProgress;
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
                  
    public Task<TimeSpan> LoadResources(bool multiThread, Action<LoadProgress>? progressAction = null) {
        return LoadResources([ResourceLoad.DefaultTag], multiThread, progressAction);
    }

    public Task<TimeSpan> LoadResources(string tag, bool multiThread, Action<LoadProgress>? progressAction = null) {
        return LoadResources([tag], multiThread, progressAction);
    }

    public async Task<TimeSpan> LoadResources(string[] tags,  bool multiThread, Action<LoadProgress>? progressAction = null) {
        var x = Stopwatch.StartNew();
        Func<Task> awaiter = Awaiter ?? (async () => {
            await ((SceneTree)Engine.GetMainLoop()).AwaitProcessFrame();
        });
        var resources = GetResourceFactories(tags)
            .Where(sf => !sf.IsLoaded())
            .Select(sf => new ResourceLoadProgress(sf.Path, sf.Load))
            .ToList();
        
        Logger.Debug("Loading {0}", tags.Join(", "));

        if (multiThread) {
            await LoadTools.LoadThreaded(resources, awaiter, OnLoad);
        } else {
            await LoadTools.LoadMonoThread(resources, awaiter, OnLoad);
        }

        var timeSpan = x.Elapsed;
        Logger.Debug("Total load time: {0}s {1:D3}ms", timeSpan.Seconds, timeSpan.Milliseconds);
        return x.Elapsed;

        void OnLoad(LoadProgress rp) {
            Logger.Debug("{0:0.00}% | {1} ({2:0.00}%)", rp.TotalPercent * 100, rp.Resource, rp.ResourcePercent * 100);
            OnLoadResourceProgress?.Invoke(rp);
            progressAction?.Invoke(rp);
        }
    }

    public void UnloadResources() {
        foreach (var sf in GetResourceFactories(ResourceLoad.DefaultTag)) {
            sf.Unload();
        }
    }

    public void UnloadResources(string tag) {
        foreach (var sf in GetResourceFactories(tag)) {
            sf.Unload();
        }
    }

    public void UnloadResources(string[] tags) {
        foreach (var sf in GetResourceFactories(tags)) {
            sf.Unload();
        }
    }
    public IEnumerable<ResourceLoad> GetResourceFactories(string tag) {
        return ResourceFactories.Where(sf => sf.Tag == tag);
    }

    public IEnumerable<ResourceLoad> GetResourceFactories(string[] tags) {
        var set = new HashSet<string>(tags);
        return ResourceFactories.Where(sf => set.Contains(sf.Tag));
    }
}