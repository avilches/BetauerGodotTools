using System;

namespace Betauer.OnReady {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OnReadyAttribute : Attribute {
        public bool Nullable { get; set; } = false;
        public readonly string? Path;

        public OnReadyAttribute() {
        }

        public OnReadyAttribute(string path) {
            Path = path;
        }
    }
}