using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Betauer.Application.Lifecycle;

public static class LoadTools {
    public static async Task LoadMonoThread(List<ResourceLoadProgress> resources, Func<Task> awaiter, Action<LoadProgress>? progressAction = null) {
        var globalProgress = new LoadProgress(progressAction);
        globalProgress.Update(0f, 0f, null);
        for (var i = 0; i < resources.Count; i++) {
            await awaiter();
            var resourceProgress = resources[i];
            var totalProgress = (float)(i+1) / resources.Count;

            if (ResourceLoader.Exists(resourceProgress.Path)) {
                var resource = ResourceLoader.Load(resourceProgress.Path);
                resourceProgress.Load(resource);
                globalProgress.Update(totalProgress, 1f, resourceProgress.Path);

            } else if (FileAccess.FileExists(resourceProgress.Path)) {
                var bytes = await LoadResourceAsync(resourceProgress.Path, async (progress) => {
                    resourceProgress.Progress = progress;
                    globalProgress.Update(totalProgress, progress, resourceProgress.Path);
                    await awaiter();
                });
                resourceProgress.Load(bytes);
                globalProgress.Update(totalProgress, 1f, resourceProgress.Path);

            } else {
                throw new ResourceLoaderException($"Resource doesn't exist {resourceProgress.Path}");
            }
        }
    }

    public static async Task<Dictionary<string, Resource>> LoadThreaded(List<string> resourcesPaths,
        Func<Task> awaiter, Action<LoadProgress>? progressAction = null) {
        var resources = resourcesPaths.Select(path => new ResourceLoadProgress(path)).ToList();
        await LoadThreaded(resources, awaiter, progressAction);
        return resources.Select(r => r.Resource!).ToDictionary(r => r.ResourcePath);
    }

    public static async Task LoadThreaded(List<ResourceLoadProgress> resources, Func<Task> awaiter, Action<LoadProgress>? progressAction = null) {
        ArgumentNullException.ThrowIfNull(awaiter);
        var globalProgress = new LoadProgress(progressAction);
        globalProgress.Update(0f, 0f, null);
        resources.ForEach(resourceProgress => {
            if (ResourceLoader.Exists(resourceProgress.Path)) {
                var error = ResourceLoader.LoadThreadedRequest(resourceProgress.Path);
                if (error != Error.Ok) throw new ResourceLoaderException($"Error requesting load {resourceProgress.Path}: {error}");

            } else if (!FileAccess.FileExists(resourceProgress.Path)) {
                throw new ResourceLoaderException($"Resource doesn't exist {resourceProgress.Path}");
            }
        });

        while (true) {
            foreach (var resourceProgress in resources.Where(r => r.Resource == null)) {
                if (!ResourceLoader.Exists(resourceProgress.Path)) {
                    var bytes = await LoadResourceAsync(resourceProgress.Path, async (progress) => {
                        resourceProgress.Progress = progress;
                        globalProgress.Update(TotalProgress(), progress, resourceProgress.Path);
                        await awaiter();
                    });
                    resourceProgress.Load(bytes);
                    globalProgress.Update(TotalProgress(), 1f, resourceProgress.Path);

                } else {
                    var (status, progress) = ThreadLoadStatus(resourceProgress);
                    if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
                        resourceProgress.Load(ResourceLoader.LoadThreadedGet(resourceProgress.Path));
                        globalProgress.Update(TotalProgress(), 1f, resourceProgress.Path);
                    } else if (status == ResourceLoader.ThreadLoadStatus.InProgress) {
                        resourceProgress.Progress = progress;
                        globalProgress.Update(TotalProgress(), progress, resourceProgress.Path);
                    } else {
                        throw new ResourceLoaderException($"Error getting load status {resourceProgress.Path}: {status}");
                    }
                }
            }
            globalProgress.Update(TotalProgress(), 0, null);
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
        var bytes = await ReadResourceAsync(path, progressCallback, bufferSize, progressIntervalMs);
        if (!TextFileExtensions.Contains(Path.GetExtension(path).ToLowerInvariant())) {
            return new BinaryResource(bytes);
        }
        var text = System.Text.Encoding.UTF8.GetString(bytes);
        return new TextResource(text);
    }

    public static async Task<byte[]> ReadResourceAsync(string path, Func<float, Task>? progressCallback = null, int bufferSize = 4096, int progressIntervalMs = 250) {
        // Check if the resource exists
        if (!FileAccess.FileExists(path)) {
            throw new FileNotFoundException($"Resource not found: {path}");
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var fileLength = file.GetLength();
        var result = new byte[fileLength];
        ulong bytesRead = 0L;
        var lastProgressUpdate = DateTime.UtcNow;

        while (bytesRead < fileLength) {
            // Calculate how many bytes to read in this iteration
            var remainingBytes = fileLength - bytesRead;
            var bytesToRead = (int)Math.Min((ulong)bufferSize, remainingBytes);

            // Read the chunk
            var chunk = file.GetBuffer(bytesToRead);

            // Copy to the result array
            Array.Copy(chunk, 0, result, (long)bytesRead, chunk.Length);
            bytesRead += (ulong)chunk.Length;

            // Check if we should notify progress
            var now = DateTime.UtcNow;
            if (progressCallback != null && (now - lastProgressUpdate).TotalMilliseconds >= progressIntervalMs) {
                var progress = (float)bytesRead / fileLength;
                await progressCallback(progress);
                lastProgressUpdate = now;
            }
        }

        return result;
    }
}