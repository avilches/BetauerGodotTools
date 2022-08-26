using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using Object = Godot.Object;

namespace Betauer.Loader {
    public class LoadingContext {
        public readonly int TotalSize;
        public readonly int LoadedSize;
        public float LoadPercent => (float)LoadedSize / TotalSize;

        public readonly string ResourcePath;
        public readonly int ResourceSize;
        public readonly int ResourceLoadedSize;
        public float ResourceLoadedPercent => (float)ResourceLoadedSize / ResourceSize;

        public LoadingContext(int totalSize, int loadedSize, string resourcePath, int resourceSize,
            int resourceLoadedSize) {
            TotalSize = totalSize;
            LoadedSize = loadedSize;
            ResourcePath = resourcePath;
            ResourceSize = resourceSize;
            ResourceLoadedSize = resourceLoadedSize;
        }
    }
    
    public class ResourceMetadata<T> where T : Resource {
        public readonly string Path;
        public readonly int Size;

        internal ResourceMetadata(string path, int size) {
            Path = path;
            Size = size;
        }

        internal ResourceMetadata(ResourceMetadata resource) {
            Path = resource.Path;
            Size = resource.Size;
            Resource = resource.Resource as T;
        }

        public T Resource { get; internal set; }
    }

    public class ResourceMetadata : ResourceMetadata<Resource> {
        public ResourceMetadata(string path, int size) : base(path, size) {
        }

        public static object CreateResourceMetadataWithGeneric(ResourceMetadata resource, Type genericType) {
            var type = typeof(ResourceMetadata<>).MakeGenericType(genericType);
            var ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(info => info.GetParameters().Length == 1 &&
                               info.GetParameters()[0].ParameterType == typeof(ResourceMetadata));
            return ctor.Invoke(new object[] { resource });
        }
    }

    public static class Loader {
        public static async Task<Dictionary<string, ResourceMetadata>> Load(IEnumerable<string> resourcesToLoad,
            Action<LoadingContext>? progress = null, Func<Task>? awaiter = null, int maxTime = 100) {
            var queue = new List<ResourceMetadata>();
            var totalSize = CreateQueue(resourcesToLoad, queue);
            var totalLoadedSize = 0;
            var resources = new Dictionary<string, ResourceMetadata>();
            var stopwatch = Stopwatch.StartNew();
            foreach (var resource in queue) {
                await Load(stopwatch, resource, totalLoadedSize, totalSize, progress, awaiter, maxTime);
                resources[resource.Path] = resource;
                totalLoadedSize += resource.Size;
            }
            return resources;
        }

        private static int CreateQueue(IEnumerable<string> resourcesToLoad, ICollection<ResourceMetadata> queue) {
            var totalSizeToLoad = 0;
            var f = new File();
            foreach (var resourcePath in resourcesToLoad) {
                Error error = f.Open(resourcePath, File.ModeFlags.Read);
                if (error != Error.Ok) {
                    throw new Exception($"Error getting resource size {resourcePath}: {error}");
                }
                var resourceSize = (int)f.GetLen();
                totalSizeToLoad += resourceSize;
                queue.Add(new ResourceMetadata(resourcePath, resourceSize));
            }
            f.Close();
            return totalSizeToLoad;
        }

        private static async Task Load(Stopwatch stopwatch, ResourceMetadata resourceMetadata, 
            int totalLoadedSize, int totalSize, Action<LoadingContext>? progress, Func<Task>? awaiter, int maxTime) {
            var resourcePath = resourceMetadata.Path;
            var resourceSize = resourceMetadata.Size;
            
            Resource resource = null;

            if (progress != null) {
                if (awaiter != null) await awaiter(); // Ensure the progress is executed on idle time
                progress(new LoadingContext(totalSize, totalLoadedSize, resourcePath, resourceSize, 0));
            }

            using (var loader = ResourceLoader.LoadInteractive(resourcePath)) {
                var stages = loader.GetStageCount();
                // Resource resource = null;
                Error pollResult = Error.Ok;
                var stage = 0;
                while (pollResult != Error.FileEof) {
                    pollResult = loader.Poll();
                    if (awaiter != null && stopwatch.ElapsedMilliseconds > maxTime) {
                        await awaiter();
                        stopwatch.Restart();
                    }
                    if (pollResult == Error.Ok) {
                        stage++;
                        var resourceLoadedSize = (int)((float)stage / stages * resourceSize);
                        if (progress != null) progress(new LoadingContext(totalSize, totalLoadedSize + resourceLoadedSize,
                            resourcePath, resourceSize, resourceLoadedSize));
                    } else if (pollResult != Error.FileEof) {
                        throw new Exception($"Error loading resource {resourcePath}: {pollResult}");
                    }
                }
                resource = loader.GetResource();
            }
            if (awaiter != null) await awaiter(); // At least one
            if (progress != null) progress(new LoadingContext(totalSize, totalLoadedSize + resourceSize,
                resourcePath, resourceSize, resourceSize));
            resourceMetadata.Resource = resource;
        }
    }
}