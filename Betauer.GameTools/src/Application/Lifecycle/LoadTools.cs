using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Betauer.Application.Lifecycle;

public static class LoadTools {
    public static async Task<Dictionary<string, Resource>> LoadMonoThread(
        List<string> resourcesPaths, Func<Task> awaiter, Action<LoadProgress>? progressAction = null) {
        var resources = resourcesPaths.Select(path => new ResourceLoadProgress(path)).ToList();
        await LoadMonoThread(resources, awaiter, progressAction);
        return resources.Select(r => r.Resource!).ToDictionary(r => r.ResourcePath);
    }

    public static async Task<Dictionary<string, Resource>> LoadThreaded(
        List<string> resourcesPaths, Func<Task> awaiter, Action<LoadProgress>? progressAction = null) {
        var resources = resourcesPaths.Select(path => new ResourceLoadProgress(path)).ToList();
        await LoadThreaded(resources, awaiter, progressAction);
        return resources.Select(r => r.Resource!).ToDictionary(r => r.ResourcePath);
    }

    public static async Task LoadMonoThread(List<ResourceLoadProgress> resources, Func<Task> awaiter, Action<LoadProgress>? progressAction = null) {
        ArgumentNullException.ThrowIfNull(awaiter);

        // Start the progress at 0%
        var progress = new LoadProgress(progressAction);
        progress.Update(0f, 0f, null);

        for (var i = 0; i < resources.Count; i++) {

            await awaiter();

            var resourceLoad = resources[i];
            var totalProgress = (float)(i + 1) / resources.Count;

            if (ResourceLoader.Exists(resourceLoad.Path)) {
                var resource = ResourceLoader.Load(resourceLoad.Path);
                resourceLoad.Load(resource);
                progress.Update(totalProgress, 1f, resourceLoad.Path);
            } else if (FileAccess.FileExists(resourceLoad.Path)) {
                var bytes = await LoadResourceAsync(resourceLoad.Path, percent => {
                    resourceLoad.Progress = percent;
                    progress.Update(totalProgress, percent, resourceLoad.Path);

                    return awaiter();
                });
                resourceLoad.Load(bytes);
                progress.Update(totalProgress, 1f, resourceLoad.Path);
            } else {
                throw new ResourceLoaderException($"Resource doesn't exist {resourceLoad.Path}");
            }
        }
    }

    public static async Task LoadThreaded(List<ResourceLoadProgress> resources, Func<Task> awaiter, Action<LoadProgress>? progressAction = null) {
        ArgumentNullException.ThrowIfNull(awaiter);

        // Start the progress at 0%
        var progress = new LoadProgress(progressAction);
        progress.Update(0f, 0f, null);

        // Request all resources or validate native resources
        resources.ForEach(resourceProgress => {
            if (ResourceLoader.Exists(resourceProgress.Path)) {
                var error = ResourceLoader.LoadThreadedRequest(resourceProgress.Path);
                if (error != Error.Ok) throw new ResourceLoaderException($"Error requesting load {resourceProgress.Path}: {error}");
            } else if (!FileAccess.FileExists(resourceProgress.Path)) {
                throw new ResourceLoaderException($"Resource doesn't exist {resourceProgress.Path}");
            }
        });

        while (true) {

            await awaiter();

            foreach (var resourceProgress in resources.Where(r => r.Resource == null)) {
                if (!ResourceLoader.Exists(resourceProgress.Path)) {
                    var bytes = await LoadResourceAsync(resourceProgress.Path, percent => {
                        resourceProgress.Progress = percent;
                        progress.Update(TotalProgress(), percent, resourceProgress.Path);

                        return awaiter();
                    });
                    resourceProgress.Load(bytes);
                    progress.Update(TotalProgress(), 1f, resourceProgress.Path);
                } else {
                    var (status, percent) = ThreadLoadStatus(resourceProgress);
                    if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
                        resourceProgress.Load(ResourceLoader.LoadThreadedGet(resourceProgress.Path));
                        progress.Update(TotalProgress(), 1f, resourceProgress.Path);
                    } else if (status == ResourceLoader.ThreadLoadStatus.InProgress) {
                        resourceProgress.Progress = percent;
                        progress.Update(TotalProgress(), percent, resourceProgress.Path);
                    } else {
                        throw new ResourceLoaderException($"Error getting load status {resourceProgress.Path}: {status}");
                    }
                }
            }
            progress.Update(TotalProgress(), 0, null);
            if (resources.All(r => r.Resource != null)) { // pending
                // No more pending
                break;
            }
        }
        return;

        float TotalProgress() => resources.Sum(r => r.Progress) / resources.Count;
    }

    private static (ResourceLoader.ThreadLoadStatus, float) ThreadLoadStatus(ResourceLoadProgress resource) {
        var progressArray = new Godot.Collections.Array();
        var status = ResourceLoader.LoadThreadedGetStatus(resource.Path, progressArray);
        return (status, (float)progressArray[0]);
    }

    public static readonly HashSet<string> TextFileExtensions = new(StringComparer.OrdinalIgnoreCase) {
        ".txt", ".json", ".csv", ".xml", ".md", ".cfg", ".ini", ".log"
    };

    public static async Task<Resource> LoadResourceAsync(string path, Func<float, Task>? progressCallback = null, int bufferSize = 4096, int progressIntervalMs = 250) {
        if (!TextFileExtensions.Contains(Path.GetExtension(path).ToLowerInvariant())) {
            return await BinaryResource.ReadBinaryResourceAsync(path, progressCallback, bufferSize, progressIntervalMs);
        }
        return await TextResource.ReadTextResourceAsync(path, progressCallback, bufferSize, progressIntervalMs);
    }
}