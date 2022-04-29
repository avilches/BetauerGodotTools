using System;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttribute : Attribute {
        public bool Nullable { get; set; } = false;
        public string? Name { get; set; }
    }

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