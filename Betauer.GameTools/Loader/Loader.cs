using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Loader {
    // TODO: Exported binaries doesn't include the binaries only the ".import" version of resources, so it's not trivial
    // to get the file size to figure out the total size.
    // Godot 4 will replace the interactive loader by a new one, so fix this will not be useful
    public static class Loader {
        private const int FakeSize = 100;
        public static async Task<IEnumerable<ResourceMetadata>> Load(IEnumerable<string> resourcesToLoad,
            Action<LoadingProgress>? progress = null, Func<Task>? awaiter = null, float maxTime = 0.100f) {
            var resources = resourcesToLoad.Select(resourcePath => new ResourceMetadata(resourcePath, FakeSize)).ToList();
            var totalSize = FakeSize * resources.Count;
            var totalLoadedSize = 0;
            var stopwatch = Stopwatch.StartNew();
            foreach (var resource in resources) {
                await Load(stopwatch, resource, totalLoadedSize, totalSize, progress, awaiter, maxTime);
                totalLoadedSize += resource.Size;
            }
            return resources;
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