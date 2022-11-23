using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.Loader {
    public static class Loader {
        private class ResourceProgress {
            internal string Path;
            internal Resource? Resource;
            internal float Progress = 0;

            public ResourceProgress(string path) {
                Path = path;
            }
        }

        public static async Task<IEnumerable<Resource>> Load(IEnumerable<string> resourcesToLoad, Func<Task> awaiter,
            Action<float>? progressAction = null) {
            return resourcesToLoad.Select(r => ResourceLoader.Load(r));
        }

        public static async Task<IEnumerable<Resource>> Load2(IEnumerable<string> resourcesToLoad, Func<Task> awaiter, 
            Action<float>? progressAction = null) {
            if (awaiter == null) throw new ArgumentNullException(nameof(awaiter));
            progressAction?.Invoke(0f);
            var resources = resourcesToLoad.Select(resourcePath => {
                ResourceLoader.LoadThreadedRequest(resourcePath);
                return new ResourceProgress(resourcePath);
            }).ToArray();
            var progressArray = new Godot.Collections.Array {
                0f
            };
            var pending = true;
            while (pending) {
                foreach (var resource in resources) {
                    if (resource.Resource == null) {
                        var status = ResourceLoader.LoadThreadedGetStatus(resource.Path, progressArray);
                        if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
                            resource.Progress = 1f;
                            resource.Resource = ResourceLoader.LoadThreadedGet(resource.Path);
                        } else if (status == ResourceLoader.ThreadLoadStatus.InProgress) {
                            resource.Progress = progressArray[0].AsSingle();
                        } else {
                            throw new ResourceLoaderException($"Error loading {resource.Path}: {status}");
                        } 
                    }
                }
                pending = resources.Any(r => r.Resource == null);
                if (pending) {
                    progressAction?.Invoke(resources.Sum(r => r.Progress));
                    await awaiter();
                }
            }
            progressAction?.Invoke(1f);
            return resources.Select(r => r.Resource!);
        }
    }
}