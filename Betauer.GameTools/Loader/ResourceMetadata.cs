using System;
using System.Linq;
using System.Reflection;
using Godot;

namespace Betauer.Loader {
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

        public void Dispose() {
            if (Resource == null) return;
            Resource.Dispose();
            Resource = null;
        }
    }

    public class ResourceMetadata : ResourceMetadata<Resource> {
        public ResourceMetadata(string path, int size) : base(path, size) {
        }

        public bool IsScene => Resource is PackedScene;

        public static object CreateGenericResourceMetadata(ResourceMetadata resource, Type genericType) {
            var type = typeof(ResourceMetadata<>).MakeGenericType(genericType);
            var ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(info => info.GetParameters().Length == 1 &&
                               info.GetParameters()[0].ParameterType == typeof(ResourceMetadata));
            return ctor.Invoke(new object[] { resource });
        }
    }
}