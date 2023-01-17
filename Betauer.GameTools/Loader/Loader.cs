using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Loader;

public static class Loader {
    private class ResourceProgress {
        internal readonly string Path;
        internal Resource? Resource;
        internal float Progress = 0;

        public ResourceProgress(string path) {
            Path = path;
        }
    }
    public static PackedScene PackedScene(string sceneResource) => ResourceLoader.Load<PackedScene>(sceneResource);
    public static T Instantiate<T>(string sceneResource) where T : Node => PackedScene(sceneResource).Instantiate<T>();
    
    public static async Task<IEnumerable<Resource>> Load(IEnumerable<string> resourcePathsToLoadEnum, Func<Task> awaiter,
        Action<float>? progressAction = null) {
        progressAction?.Invoke(0f);
        var resourcePaths = resourcePathsToLoadEnum.ToArray();
        var count = 0f;
        return resourcePaths.Select(path => {
            count++;
            var resource = ResourceLoader.Load(path);
            if (resource == null) throw new ResourceLoaderException($"Resource {path} not found");
            progressAction?.Invoke(count / resourcePaths.Length);
            return resource;
        });
    }

    public static async Task<IEnumerable<Resource>> LoadThreaded(IEnumerable<string> resourcesToLoad, Func<Task> awaiter, 
        Action<float>? progressAction = null) {
        if (awaiter == null) throw new ArgumentNullException(nameof(awaiter));
        progressAction?.Invoke(0f);
        var resources = resourcesToLoad.Select(resourcePath => {
            var error = ResourceLoader.LoadThreadedRequest(resourcePath);
            if (error != Error.Ok) throw new ResourceLoaderException($"Error request loading {resourcePath}: {error}");
            // GD.Print($"Request loading {resourcePath}");
            return new ResourceProgress(resourcePath);
        }).ToArray();
        var progressArray = new Godot.Collections.Array { 0f };
        var pending = true;
        while (pending) {
            foreach (var resource in resources) {
                if (resource.Resource == null) {
                    var status = ResourceLoader.LoadThreadedGetStatus(resource.Path, progressArray);
                    var progress = (float)progressArray[0];
                    if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
                        // GD.Print($"DONE {resource.Path}: {progress}");
                        resource.Progress = 1f;
                        resource.Resource = ResourceLoader.LoadThreadedGet(resource.Path);
                    } else if (status == ResourceLoader.ThreadLoadStatus.InProgress) {
                        // GD.Print($"... {resource.Path}: {progress}");
                        resource.Progress = progress;
                    } else {
                        throw new ResourceLoaderException($"Error get status {resource.Path}: {status}");
                    } 
                }
            }
            pending = resources.Any(r => r.Resource == null);
            if (pending) {
                var totalProgress = resources.Sum(r => r.Progress) / resources.Length;
                progressAction?.Invoke(totalProgress);
                await awaiter();
            }
        }
        progressAction?.Invoke(1f);
        return resources.Select(r => r.Resource!);
    }
}