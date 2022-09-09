using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Loader {
    public static class Loader {
        public static async Task<IEnumerable<ResourceMetadata>> Load(IEnumerable<string> resourcesToLoad,
            Action<LoadingProgress>? progress = null, Func<Task>? awaiter = null, float maxTime = 0.100f) {
            var (resources, totalSize) = CreateQueue(resourcesToLoad);
            var totalLoadedSize = 0;
            var stopwatch = Stopwatch.StartNew();
            foreach (var resource in resources) {
                await Load(stopwatch, resource, totalLoadedSize, totalSize, progress, awaiter, maxTime);
                totalLoadedSize += resource.Size;
            }
            return resources;
        }

        private static (IEnumerable<ResourceMetadata>, int) CreateQueue(IEnumerable<string> resourcesToLoad) {
            var totalSizeToLoad = 0;
            var godotFileHandler = new File();
            var queue = new List<ResourceMetadata>();
            foreach (var resourcePath in resourcesToLoad) {
                Error error = godotFileHandler.Open(resourcePath, File.ModeFlags.Read);
                if (error != Error.Ok) {
                    throw new Exception($"Error getting resource size {resourcePath}: {error}");
                }
                var resourceSize = (int)godotFileHandler.GetLen();
                totalSizeToLoad += resourceSize;
                queue.Add(new ResourceMetadata(resourcePath, resourceSize));
            }
            godotFileHandler.Close();
            godotFileHandler.Dispose();
            return (queue, totalSizeToLoad);
        }

        private static async Task Load(Stopwatch stopwatch, ResourceMetadata resourceMetadata, 
            int totalLoadedSize, int totalSize, Action<LoadingProgress>? progress, Func<Task>? awaiter, float maxTime) {
            var resourcePath = resourceMetadata.Path;
            var resourceSize = resourceMetadata.Size;
            LoadingProgress loadingProgress = null;
            Resource resource = null;
            if (progress != null) {
                if (awaiter != null) await awaiter(); // Ensure the progress is executed on idle time
                loadingProgress = new LoadingProgress();
                progress(loadingProgress.Update(totalSize, totalLoadedSize, resourcePath, resourceSize, 0));
            }

            using (var loader = ResourceLoader.LoadInteractive(resourcePath)) {
                var stages = loader.GetStageCount();
                // Resource resource = null;
                Error pollResult = Error.Ok;
                var stage = 0;
                while (pollResult != Error.FileEof) {
                    pollResult = loader.Poll();
                    if (pollResult == Error.Ok) {
                        stage++;
                        var resourceLoadedSize = (int)((float)stage / stages * resourceSize);
                        if (progress != null && loadingProgress != null) {
                            progress(loadingProgress.Update(totalSize, totalLoadedSize + resourceLoadedSize,
                                resourcePath, resourceSize, resourceLoadedSize));
                        }
                        if (awaiter != null && stopwatch.Elapsed.Seconds > maxTime) {
                            await awaiter();
                            stopwatch.Restart();
                        }
                    } else if (pollResult != Error.FileEof) {
                        throw new Exception($"Error loading resource {resourcePath}: {pollResult}");
                    }
                }
                resource = loader.GetResource();
            }
            if (progress != null && loadingProgress != null) {
                progress(loadingProgress.Update(totalSize, totalSize, resourcePath, resourceSize, resourceSize));
                if (awaiter != null) await awaiter(); // At least one to update the final status
            }
            resourceMetadata.Resource = resource;
        }
    }
}