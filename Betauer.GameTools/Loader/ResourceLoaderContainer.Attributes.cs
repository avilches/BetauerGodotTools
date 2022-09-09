using System;

namespace Betauer.Loader {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LoadAttribute : Attribute {
        public readonly string? ResourceName;
        public readonly string Path;

        public LoadAttribute(string path) {
            Path = path;
        }
        
        public LoadAttribute(string resourceName, string path) {
            ResourceName = resourceName;
            Path = path;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ResourceAttribute : Attribute {
        public readonly string ResourceName;
        public ResourceAttribute(string resourceName) {
            ResourceName = resourceName;
        }
    }
}