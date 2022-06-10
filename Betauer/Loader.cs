using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Collections;
using Godot;

namespace Betauer {
    public readonly struct LoadingContext {
        public readonly int TotalSize;
        public readonly int TotalLoadedSize;
        public float TotalLoadedPercent => (float)TotalLoadedSize / TotalSize;

        public readonly string ResourcePath;
        public readonly int ResourceSize;
        public readonly int ResourceLoadedSize;
        public float ResourceLoadedPercent => (float)ResourceLoadedSize / ResourceSize;

        public LoadingContext(int totalSize, int totalLoadedSize, string resourcePath, int resourceSize,
            int resourceLoadedSize) {
            TotalSize = totalSize;
            TotalLoadedSize = totalLoadedSize;
            ResourcePath = resourcePath;
            ResourceSize = resourceSize;
            ResourceLoadedSize = resourceLoadedSize;
        }
    }

    public static class Loader {
        private struct ResourceToLoad {
            internal readonly string Path;
            internal readonly int Size;

            public ResourceToLoad(string path, int size) {
                Path = path;
                Size = size;
            }
        }

        public static async Task<Dictionary<string, Resource>> Load(IEnumerable<string> resourcesToLoad,
            Action<LoadingContext>? progress = null, Func<Task>? awaiter = null) {
            var queue = new FastUnsafeLinkedList<ResourceToLoad>();
            var totalSize = CreateQueue(resourcesToLoad, queue);
            var totalLoadedSize = 0;
            var resources = new Dictionary<string, Resource>();
            foreach (var resource in queue) {
                Resource godotResource = await Load(
                    resource.Path, resource.Size, totalLoadedSize, totalSize, progress, awaiter);
                resources[resource.Path] = godotResource;
                totalLoadedSize += resource.Size;
            }
            return resources;
        }

        private static int CreateQueue(IEnumerable<string> resourcesToLoad, ICollection<ResourceToLoad> queue) {
            var totalSizeToLoad = 0;
            var f = new File();
            foreach (var resourceName in resourcesToLoad) {
                f.Open(resourceName, File.ModeFlags.Read);
                var resourceSize = (int)f.GetLen();
                totalSizeToLoad += resourceSize;
                queue.Add(new ResourceToLoad(resourceName, resourceSize));
            }
            f.Close();
            return totalSizeToLoad;
        }

        private static async Task<Resource> Load(string resourcePath, int resourceSize,
            int totalLoadedSize, int totalSize, Action<LoadingContext>? progress = null, Func<Task>? awaiter = null) {
            Resource resource = null;

            progress?.Invoke(new LoadingContext(totalSize, totalLoadedSize, resourcePath, resourceSize, 0));

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
                        progress?.Invoke(new LoadingContext(totalSize, totalLoadedSize + resourceLoadedSize,
                            resourcePath, resourceSize, resourceLoadedSize));
                        if (awaiter != null) await awaiter();
                    }
                }
                resource = loader.GetResource();
            }
            progress?.Invoke(new LoadingContext(totalSize, totalLoadedSize + resourceSize,
                resourcePath, resourceSize, resourceSize));
            return resource;
        }
    }
}